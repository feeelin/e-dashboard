import pandas as pd
import numpy as np
import logging
from pathlib import Path
from src.utils.config import load_config
from src.data_processing.mock_data_generator import generate_mock_data # Use mock data

logger = logging.getLogger(__name__)

def load_data(use_mock=True, **kwargs):
    """Loads data, currently using mock generator."""
    if use_mock:
        logger.info("Loading data using mock data generator...")
        sprints_df, issues_df, status_transitions_df = generate_mock_data(**kwargs)
        return sprints_df, issues_df, status_transitions_df
    else:
        # Placeholder for real data extraction logic from DB
        logger.error("Real data extraction not implemented yet.")
        # Example:
        ###
        # config = load_config()
        # extractor = JiraExtractor(config) # Assume JiraExtractor class exists
        # sprints_df = extractor.get_sprints()
        # issues_df = extractor.get_issues()
        # status_transitions_df = extractor.get_status_transitions()
        return pd.DataFrame(), pd.DataFrame(), pd.DataFrame()

def map_statuses(status_series, status_mapping):
    """Maps detailed statuses to broader categories (todo, inprogress, done)."""
    reverse_map = {}
    for key, values in status_mapping.items():
        for value in values:
            reverse_map[value.lower()] = key # Use lowercase for robust matching
    # Apply mapping, keep original if not found (or map to 'other')
    return status_series.str.lower().map(reverse_map).fillna('other')

def calculate_cycle_time(issue_id, transitions_df):
    """Calculates cycle time (In Progress -> Done) for a single issue."""
    issue_transitions = transitions_df[transitions_df['issue_id'] == issue_id].sort_values('timestamp')
    # Find first entry into an 'inprogress' state
    inprogress_entry = issue_transitions[issue_transitions['to_status'].str.lower().isin(['in progress', 'in development', 'in review'])].iloc[0] if not issue_transitions[issue_transitions['to_status'].str.lower().isin(['in progress', 'in development', 'in review'])].empty else None
    # Find first entry into a 'done' state *after* entering inprogress
    done_entry = None
    if inprogress_entry is not None:
       done_transitions = issue_transitions[
           (issue_transitions['timestamp'] >= inprogress_entry['timestamp']) &
           (issue_transitions['to_status'].str.lower().isin(['done', 'resolved', 'closed']))
       ]
       if not done_transitions.empty:
           done_entry = done_transitions.iloc[0]

    if inprogress_entry is not None and done_entry is not None:
        cycle_time = done_entry['timestamp'] - inprogress_entry['timestamp']
        return cycle_time.total_seconds() / (60 * 60 * 24) # Return in days
    return np.nan

def preprocess_issues(issues_df, status_transitions_df, config):
    """Preprocesses the issues DataFrame."""
    logger.info(f"Preprocessing {len(issues_df)} issues...")
    df = issues_df.copy()

    # Type Conversion (already done in mock data gen, but good practice)
    df['created_date'] = pd.to_datetime(df['created_date'], errors='coerce')
    df['resolved_date'] = pd.to_datetime(df['resolved_date'], errors='coerce')
    df['story_points'] = pd.to_numeric(df['story_points'], errors='coerce') # Coerce errors to NaN

    # Handle missing story points (simple mean imputation for now)
    # A better approach might be per issue type or using a simple model
    mean_sp = df['story_points'].mean()
    df['story_points_imputed'] = df['story_points'].isnull() # Flag imputed values
    df['story_points'].fillna(mean_sp, inplace=True)
    logger.info(f"Imputed story points for {df['story_points_imputed'].sum()} issues with mean {mean_sp:.2f}")

    # Map Statuses
    status_mapping = config.get('status_mapping', {})
    df['status_category'] = map_statuses(df['status'], status_mapping)
    logger.info(f"Status categories mapped: {df['status_category'].value_counts().to_dict()}")

    # Calculate Cycle Time (This can be slow on large datasets)
    # Optimize if necessary (e.g., pre-grouping transitions)
    # Temporarily disable/simplify if too slow during initial dev
    logger.info("Calculating cycle times...")
    if status_transitions_df is not None and not status_transitions_df.empty:
         # Ensure transitions 'to_status' is string type before applying .str accessor
         status_transitions_df['to_status'] = status_transitions_df['to_status'].astype(str)
         # Group transitions by issue_id for faster lookup if needed
         ### 
         # grouped_transitions = status_transitions_df.groupby('issue_id')
         # df['cycle_time_days'] = df['issue_id'].apply(lambda x: calculate_cycle_time(x, grouped_transitions.get_group(x) if x in grouped_transitions.groups else pd.DataFrame()))
         df['cycle_time_days'] = df['issue_id'].apply(lambda x: calculate_cycle_time(x, status_transitions_df))
         logger.info(f"Calculated cycle time for {df['cycle_time_days'].notna().sum()} issues.")
    else:
         logger.warning("Status transitions data is missing, cannot calculate cycle time.")
         df['cycle_time_days'] = np.nan


    # Add more cleaning steps as needed (e.g., outlier handling)

    logger.info("Issue preprocessing complete.")
    return df

def preprocess_sprints(sprints_df):
    """Preprocesses the sprints DataFrame."""
    logger.info(f"Preprocessing {len(sprints_df)} sprints...")
    df = sprints_df.copy()
    df['start_date'] = pd.to_datetime(df['start_date'], errors='coerce')
    df['end_date'] = pd.to_datetime(df['end_date'], errors='coerce')
    df['completed_date'] = pd.to_datetime(df['completed_date'], errors='coerce')
    df['sprint_duration_days'] = (df['end_date'] - df['start_date']).dt.days + 1 # Inclusive
    logger.info("Sprint preprocessing complete.")
    return df

def calculate_actual_velocity(processed_issues_df, closed_sprints_df, config):
    """Calculates actual velocity for closed sprints."""
    logger.info("Calculating actual velocity for closed sprints...")
    done_statuses = config.get('status_mapping', {}).get('done', [])
    done_statuses_lower = [s.lower() for s in done_statuses]

    # Filter issues that are considered 'Done' and belong to a sprint
    # Using status_category ensures consistency
    done_issues = processed_issues_df[
        (processed_issues_df['status_category'] == 'done') &
        (processed_issues_df['sprint_id'].notna()) # Make sure issue is associated with a sprint
    ]

    # Group by sprint_id and sum story points
    velocity = done_issues.groupby('sprint_id')['story_points'].sum().reset_index()
    velocity.rename(columns={'story_points': 'actual_velocity'}, inplace=True)

    # Merge with closed sprints data
    # We only care about velocity for sprints that actually finished
    sprints_with_velocity = closed_sprints_df.merge(velocity, on='sprint_id', how='left')
    sprints_with_velocity['actual_velocity'].fillna(0, inplace=True) # Sprints with 0 completed points

    logger.info(f"Actual velocity calculated for {len(sprints_with_velocity)} closed sprints.")
    return sprints_with_velocity


if __name__ == '__main__':
    from src.utils.logging_config import setup_logging
    setup_logging()

    cfg = load_config()
    if cfg:
        s_df, i_df, st_df = load_data(use_mock=True, num_sprints=5, issues_per_sprint=10) # Load small mock data

        processed_sprints_df = preprocess_sprints(s_df)
        processed_issues_df = preprocess_issues(i_df, st_df, cfg)

        # Filter only closed sprints before calculating velocity
        closed_sprints = processed_sprints_df[processed_sprints_df['state'] == 'closed'].copy()

        if not closed_sprints.empty:
            sprints_with_velocity_df = calculate_actual_velocity(processed_issues_df, closed_sprints, cfg)
            print("\nProcessed Sprints with Actual Velocity (Head):")
            print(sprints_with_velocity_df.head())
            sprints_with_velocity_df.info()
        else:
            print("\nNo closed sprints found in the mock data generated.")

        print("\nProcessed Issues (Head):")
        print(processed_issues_df.head())
        processed_issues_df.info()

        # Save processed data
        ###
        # output_dir = Path(cfg['paths']['processed_data_dir'])
        # output_dir.mkdir(parents=True, exist_ok=True)
        # processed_sprints_df.to_parquet(output_dir / "processed_sprints.parquet")
        # processed_issues_df.to_parquet(output_dir / "processed_issues.parquet")
        # if sprints_with_velocity_df is not None:
        #     sprints_with_velocity_df.to_parquet(output_dir / "closed_sprints_with_velocity.parquet")
    else:
        logger.error("Could not load configuration.")
