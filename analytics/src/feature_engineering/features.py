import pandas as pd
import numpy as np
import logging

logger = logging.getLogger(__name__)

def generate_historical_velocity_features(sprints_with_velocity_df, windows=[1, 3, 5]):
    """Generates historical velocity features (e.g., avg velocity of last N sprints)."""
    logger.info(f"Generating historical velocity features for windows: {windows}")
    df = sprints_with_velocity_df.sort_values('start_date').copy()

    if 'actual_velocity' not in df.columns:
         logger.error("Input DataFrame must contain 'actual_velocity' column.")
         return df # Return original df or raise error

    # Shift by 1 to ensure we use past data only (velocity of sprint k uses data up to k-1)
    df_shifted = df['actual_velocity'].shift(1)

    for w in windows:
        col_name = f"avg_velocity_last_{w}_sprints"
        df[col_name] = df_shifted.rolling(window=w, min_periods=1).mean()
        # Handle potential NaNs at the beginning
        # df[col_name].fillna(df[col_name].median(), inplace=True) # Or use 0 or ffill

    logger.info(f"Generated features: {', '.join([f'avg_velocity_last_{w}_sprints' for w in windows])}")
    return df


def generate_planned_features(sprints_df, processed_issues_df):
    """Generates features based on issues planned for each sprint."""
    logger.info("Generating planned features (story points, issue count) for sprints...")
    # We need *all* sprints here, not just closed ones, as we might predict for future/active ones
    df_sprints = sprints_df.copy()
    df_issues = processed_issues_df.copy()

    # Group issues by their assigned sprint_id
    # Use the non-imputed story points for planning features if desired, or the imputed one
    planned_agg = df_issues.groupby('sprint_id').agg(
        planned_story_points=('story_points', 'sum'),
        planned_issue_count=('issue_id', 'count')
    ).reset_index()

    # Merge planned features into the sprints DataFrame
    df_sprints = df_sprints.merge(planned_agg, on='sprint_id', how='left')

    # Fill NaNs for sprints with potentially no issues planned (or if merge fails)
    df_sprints['planned_story_points'].fillna(0, inplace=True)
    df_sprints['planned_issue_count'].fillna(0, inplace=True)

    logger.info("Generated features: planned_story_points, planned_issue_count")
    return df_sprints


def combine_features(historical_sprint_features_df, planned_sprint_features_df):
    """Combines historical and planned features."""
    logger.info("Combining historical and planned sprint features.")
    # Both dataframes have 'sprint_id' as a key
    # Use the planned features df as base, merge historical (which only exist for closed sprints + 1 future one potentially)
    combined_df = planned_sprint_features_df.merge(
        historical_sprint_features_df[['sprint_id'] + [col for col in historical_sprint_features_df.columns if 'avg_velocity' in col]],
        on='sprint_id',
        how='left' # Keep all sprints, historical features will be NaN for earliest sprints
    )
    logger.info(f"Combined DataFrame shape: {combined_df.shape}")
    return combined_df

if __name__ == '__main__':
    # Usage (requires processed data files)
    from src.utils.logging_config import setup_logging
    from src.utils.config import load_config
    from pathlib import Path
    setup_logging()
    cfg = load_config()

    processed_dir = Path(cfg['paths']['processed_data_dir'])
    closed_sprints_path = processed_dir / "closed_sprints_with_velocity.parquet"
    all_sprints_path = processed_dir / "processed_sprints.parquet"
    issues_path = processed_dir / "processed_issues.parquet"

    if closed_sprints_path.exists() and all_sprints_path.exists() and issues_path.exists():
        closed_sprints_df = pd.read_parquet(closed_sprints_path)
        all_sprints_df = pd.read_parquet(all_sprints_path)
        issues_df = pd.read_parquet(issues_path)

        hist_feat_df = generate_historical_velocity_features(closed_sprints_df, windows=[1, 3])
        plan_feat_df = generate_planned_features(all_sprints_df, issues_df)
        final_features_df = combine_features(hist_feat_df, plan_feat_df)

        print("\nFinal Combined Features DataFrame (Head):")
        print(final_features_df.head())
        final_features_df.info()
        print("\nCheck for NaNs in historical features (expected at start):")
        print(final_features_df[['avg_velocity_last_1_sprints', 'avg_velocity_last_3_sprints']].isnull().sum())
    else:
        logger.error("Could not find processed data files needed for feature engineering example.")
