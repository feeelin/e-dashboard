
import logging
import pandas as pd
from datetime import datetime, timedelta

from src.utils.config import load_config
from src.utils.logging_config import setup_logging
from src.inference.predict import predict_velocity, load_historical_data

# Setup logging
setup_logging()
logger = logging.getLogger(__name__)

def main():
    logger.info("Starting inference script...")
    cfg = load_config()
    if not cfg:
        logger.error("Failed to load configuration. Exiting.")
        return

    # --- Simulate Input Data for Prediction ---
    # Find the start date for the next sprint based on historical data
    historical_data = load_historical_data(cfg)
    if historical_data is None or historical_data.empty:
         logger.error("Cannot run inference without historical data. Exiting.")
         return

    last_sprint_start = historical_data['start_date'].max()
    # Assuming 14 day sprints, next starts immediately after
    # A more robust approach would use sprint end dates or look at future sprints in data
    next_sprint_start_date = last_sprint_start + timedelta(days=14)
    logger.info(f"Last historical sprint started: {last_sprint_start.date()}. Predicting for sprint starting: {next_sprint_start_date.date()}")

    # Define the input data for the sprint to predict
    # These values should ideally come from Jira for the actual upcoming sprint
    sprint_input = {
        "start_date": next_sprint_start_date.strftime("%Y-%m-%d"),
        "planned_story_points": 55.0,  # Planned points
        "planned_issue_count": 18      # Planned count
    }
    logger.info(f"Input data for prediction: {sprint_input}")


    # --- Run Prediction ---
    predicted_value = predict_velocity(sprint_input, model_name="velocity_gbr")


    # --- Display Result ---
    if predicted_value is not None:
        logger.info("-" * 30)
        logger.info(f"Inference Result:")
        logger.info(f"  Input Sprint Start Date: {sprint_input['start_date']}")
        logger.info(f"  Predicted Velocity: {predicted_value:.2f}")
        logger.info("-" * 30)
    else:
        logger.error("Inference failed.")

    logger.info("Inference script finished.")

if __name__ == "__main__":
    main()
