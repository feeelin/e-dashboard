import logging
from pathlib import Path
from src.utils.config import load_config
from src.utils.logging_config import setup_logging
from src.data_processing.preprocessing import (
    load_data,
    preprocess_sprints,
    preprocess_issues,
    calculate_actual_velocity
)

# Setup logging
setup_logging()
logger = logging.getLogger(__name__)

def main():
    logger.info("Starting data preprocessing script...")
    cfg = load_config()
    if not cfg:
        logger.error("Failed to load configuration. Exiting.")
        return

    # Define output paths
    output_dir = Path(cfg['paths']['processed_data_dir'])
    output_dir.mkdir(parents=True, exist_ok=True)
    sprints_out_path = output_dir / "processed_sprints.parquet"
    issues_out_path = output_dir / "processed_issues.parquet"
    closed_sprints_out_path = output_dir / "closed_sprints_with_velocity.parquet"


    # 1. Load Data
    s_df, i_df, st_df = load_data(use_mock=True, num_sprints=30, issues_per_sprint=15) # Use mock data
    if s_df.empty or i_df.empty:
         logger.error("Failed to load data. Exiting.")
         return

    # 2. Preprocess Sprints
    processed_sprints_df = preprocess_sprints(s_df)

    # 3. Preprocess Issues
    processed_issues_df = preprocess_issues(i_df, st_df, cfg) # Pass config for status mapping etc

    # 4. Calculate Actual Velocity for CLOSED sprints
    closed_sprints_df = processed_sprints_df[processed_sprints_df['state'] == 'closed'].copy()
    if not closed_sprints_df.empty:
        sprints_with_velocity_df = calculate_actual_velocity(
            processed_issues_df, closed_sprints_df, cfg
        )
        logger.info(f"Saving {len(sprints_with_velocity_df)} closed sprints with velocity to {closed_sprints_out_path}")
        sprints_with_velocity_df.to_parquet(closed_sprints_out_path, index=False)
    else:
        logger.warning("No closed sprints found. Skipping velocity calculation and saving.")
        # Create an empty file maybe? Or handle downstream. For now, just warn.

    # 5. Save Processed Dataframes
    logger.info(f"Saving processed sprints to {sprints_out_path}")
    processed_sprints_df.to_parquet(sprints_out_path, index=False)

    logger.info(f"Saving processed issues to {issues_out_path}")
    processed_issues_df.to_parquet(issues_out_path, index=False)


    logger.info("Data preprocessing script finished.")

if __name__ == "__main__":
    main()
