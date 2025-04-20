import logging
import pandas as pd
from pathlib import Path
from src.utils.config import load_config
from src.utils.logging_config import setup_logging
from src.feature_engineering.features import (
    generate_historical_velocity_features,
    generate_planned_features,
    combine_features
)

# Setup logging
setup_logging()
logger = logging.getLogger(__name__)

def main():
    logger.info("Starting feature engineering script...")
    cfg = load_config()
    if not cfg:
        logger.error("Failed to load configuration. Exiting.")
        return

    # Define input and output paths
    processed_dir = Path(cfg['paths']['processed_data_dir'])
    features_dir = Path(cfg['paths']['features_dir'])
    features_dir.mkdir(parents=True, exist_ok=True)

    closed_sprints_path = processed_dir / "closed_sprints_with_velocity.parquet"
    all_sprints_path = processed_dir / "processed_sprints.parquet"
    issues_path = processed_dir / "processed_issues.parquet"
    output_path = features_dir / "model_input_features.parquet"

    # Check if input files exist
    if not all([p.exists() for p in [closed_sprints_path, all_sprints_path, issues_path]]):
        logger.error(f"One or more input files not found in {processed_dir}. "
                     "Please run the preprocessing script first. Exiting.")
        return

    # 1. Load Processed Data
    logger.info("Loading processed data...")
    closed_sprints_df = pd.read_parquet(closed_sprints_path)
    all_sprints_df = pd.read_parquet(all_sprints_path)
    issues_df = pd.read_parquet(issues_path)

    # 2. Generate Historical Features (using closed sprints with velocity)
    # Ensure sorted by date for rolling calculations
    closed_sprints_df = closed_sprints_df.sort_values(by='start_date')
    hist_feat_df = generate_historical_velocity_features(closed_sprints_df, windows=[1, 3, 5])

    # 3. Generate Planned Features (using all sprints and issues)
    plan_feat_df = generate_planned_features(all_sprints_df, issues_df)

    # 4. Combine Features
    # We need to combine based on all sprints but keep the target variable (actual_velocity)
    # which only exists for closed sprints.
    final_features_df = combine_features(hist_feat_df, plan_feat_df)

    # Make sure 'actual_velocity' is present for the rows corresponding to closed sprints
    # It might get lost if combine_features doesn't explicitly merge it back. Let's ensure it's there.
    if 'actual_velocity' not in final_features_df.columns and 'actual_velocity' in hist_feat_df.columns:
         final_features_df = final_features_df.merge(
             hist_feat_df[['sprint_id', 'actual_velocity']], on='sprint_id', how='left'
         )

    # 5. Save Features
    logger.info(f"Saving final features to {output_path}")
    final_features_df.to_parquet(output_path, index=False)

    logger.info(f"Feature engineering script finished. Output shape: {final_features_df.shape}")
    logger.info(f"Columns: {final_features_df.columns.tolist()}")

if __name__ == "__main__":
    main()
