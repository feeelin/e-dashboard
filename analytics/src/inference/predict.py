import pandas as pd
import numpy as np
import joblib
import logging
from pathlib import Path

from src.utils.config import load_config
# Need feature engineering functions to recreate features for new data
from src.feature_engineering.features import generate_historical_velocity_features

logger = logging.getLogger(__name__)

# Global cache for models/features to avoid reloading repeatedly if running server
# In a script, this doesn't help much unless called multiple times
_model_cache = {}
_features_cache = {}
_historical_data_cache = None

def load_model_and_features(model_name="velocity_gbr", config=None):
    """Loads a trained model and its corresponding feature list."""
    if model_name in _model_cache and model_name in _features_cache:
         return _model_cache[model_name], _features_cache[model_name]

    if config is None:
        config = load_config()
    if not config:
        logger.error("Configuration not loaded, cannot find model path.")
        return None, None

    models_dir = Path(config['paths']['models_dir'])
    model_path = models_dir / f"{model_name}_model.joblib"
    features_list_path = models_dir / f"{model_name}_features.joblib"

    if not model_path.exists() or not features_list_path.exists():
        logger.error(f"Model '{model_path}' or features list '{features_list_path}' not found.")
        return None, None

    try:
        model = joblib.load(model_path)
        features_list = joblib.load(features_list_path)
        logger.info(f"Loaded model from {model_path} and features list ({len(features_list)} features).")
        _model_cache[model_name] = model
        _features_cache[model_name] = features_list
        return model, features_list
    except Exception as e:
        logger.error(f"Error loading model or features list: {e}")
        return None, None

def load_historical_data(config=None):
    """Loads historical data needed for feature calculation during inference."""
    global _historical_data_cache
    if _historical_data_cache is not None:
        return _historical_data_cache

    if config is None:
        config = load_config()
    if not config:
         logger.error("Configuration not loaded, cannot find historical data path.")
         return None

    processed_dir = Path(config['paths']['processed_data_dir'])
    closed_sprints_path = processed_dir / "closed_sprints_with_velocity.parquet" # Needs actual velocity

    if not closed_sprints_path.exists():
         logger.error(f"Historical closed sprints data not found at {closed_sprints_path}.")
         return None

    try:
         # Only load columns needed for historical feature generation
         hist_df = pd.read_parquet(closed_sprints_path, columns=['sprint_id', 'start_date', 'actual_velocity'])
         hist_df = hist_df.sort_values(by='start_date')
         _historical_data_cache = hist_df
         logger.info(f"Loaded historical velocity data: {len(hist_df)} sprints.")
         return hist_df
    except Exception as e:
         logger.error(f"Error loading historical data: {e}")
         return None


def prepare_inference_features(sprint_input_data, historical_data, features_list):
    """
    Prepares the feature vector for a single sprint prediction.

    Args:
        sprint_input_data (dict): Dict containing planned features for the sprint
                                   (e.g., 'planned_story_points', 'planned_issue_count', 'start_date').
        historical_data (pd.DataFrame): DataFrame of historical sprints with actual velocity, sorted by date.
        features_list (list): The ordered list of feature names the model expects.

    Returns:
        pd.DataFrame: A single-row DataFrame with features ready for prediction, or None if error.
    """
    logger.debug(f"Preparing features for input: {sprint_input_data}")

    # Calculate historical features based on data *before* the target sprint start date
    target_start_date = pd.to_datetime(sprint_input_data.get('start_date'))
    if target_start_date is None:
         logger.error("Missing 'start_date' in sprint_input_data.")
         return None

    # Simulate adding the new sprint to calculate rolling features correctly
    # We need the LATEST historical features available BEFORE this sprint starts
    relevant_history = historical_data[historical_data['start_date'] < target_start_date]
    if relevant_history.empty:
        logger.warning("No historical data available before the target sprint start date. Historical features will be NaN.")
        # Create a placeholder row with NaN velocity to calculate rolling features if needed
        # This might result in NaN features if min_periods isn't met
        last_known_velocity = np.nan
    else:
        # Use the velocity of the most recent historical sprint
        last_known_velocity = relevant_history['actual_velocity'].iloc[-1]


    # Construct the feature row - start with planned features
    feature_values = {
        'planned_story_points': sprint_input_data.get('planned_story_points', 0),
        'planned_issue_count': sprint_input_data.get('planned_issue_count', 0),
         # Initialize historical features potentially with NaN
        'avg_velocity_last_1_sprints': np.nan,
        'avg_velocity_last_3_sprints': np.nan,
        'avg_velocity_last_5_sprints': np.nan,
        # Add other features if the model uses them
    }

    # Calculate rolling averages using relevant history
    # This simplified approach takes the latest values. A more robust way might
    # involve temporarily adding a row for the prediction sprint and recalculating.
    if not relevant_history.empty:
         hist_vel = relevant_history['actual_velocity']
         if len(hist_vel) >= 1: feature_values['avg_velocity_last_1_sprints'] = hist_vel.iloc[-1] # Last actual velocity
         if len(hist_vel) >= 1: feature_values['avg_velocity_last_3_sprints'] = hist_vel.rolling(window=3, min_periods=1).mean().iloc[-1]
         if len(hist_vel) >= 1: feature_values['avg_velocity_last_5_sprints'] = hist_vel.rolling(window=5, min_periods=1).mean().iloc[-1]


    # Create DataFrame with the exact columns expected by the model
    try:
        inference_df = pd.DataFrame([feature_values], columns=features_list)
        # Check for missing columns (shouldn't happen if features_list is correct)
        missing_cols = set(features_list) - set(inference_df.columns)
        if missing_cols:
             logger.error(f"Missing required features during inference prep: {missing_cols}")
             return None
        # Ensure correct order - already done by specifying columns in DataFrame constructor

        # Handle potential NaNs - model might need imputation strategy learned during training
        # For GBR, it can sometimes handle NaNs, but explicit handling is safer.
        # Simple strategy: fill with 0 or median/mean *learned from training data*
        # For now, let's assume the model or preprocessing handles it, or fill with 0
        inference_df.fillna(0, inplace=True) # Caution: Use a more principled imputation if needed

        logger.debug(f"Prepared inference DataFrame:\n{inference_df}")
        return inference_df

    except Exception as e:
         logger.error(f"Error creating inference DataFrame: {e}")
         return None


def predict_velocity(sprint_input_data, model_name="velocity_gbr"):
    """Makes a velocity prediction for a given sprint's planned data."""
    logger.info(f"Received prediction request for sprint starting: {sprint_input_data.get('start_date')}")

    model, features_list = load_model_and_features(model_name=model_name)
    if model is None or features_list is None:
        return None # Error already logged

    historical_data = load_historical_data()
    if historical_data is None:
        return None # Error already logged

    inference_features_df = prepare_inference_features(sprint_input_data, historical_data, features_list)
    if inference_features_df is None:
        logger.error("Failed to prepare features for inference.")
        return None

    try:
        prediction = model.predict(inference_features_df)
        predicted_velocity = prediction[0] # Get the scalar value
        logger.info(f"Predicted velocity: {predicted_velocity:.2f}")
        return predicted_velocity
    except Exception as e:
        logger.error(f"Error during model prediction: {e}")
        return None

if __name__ == '__main__':
    from src.utils.logging_config import setup_logging
    setup_logging()

    # --- Example Usage ---
    # Simulate input for a sprint we want to predict
    # Needs plausible planned points/count and a start date AFTER the last historical sprint
    hist_data = load_historical_data()
    if hist_data is not None and not hist_data.empty:
        last_sprint_start = hist_data['start_date'].max()
        next_sprint_start = last_sprint_start + timedelta(days=14) # Assume next sprint starts right after

        sprint_to_predict = {
            "start_date": next_sprint_start.strftime("%Y-%m-%d"), # Crucial for historical feature calculation
            "planned_story_points": 45.0, # Example value
            "planned_issue_count": 12     # Example value
            # Add other potential features if needed by the model
        }

        predicted_vel = predict_velocity(sprint_to_predict)

        if predicted_vel is not None:
            print(f"\n--- Inference Example ---")
            print(f"Input Sprint Data: {sprint_to_predict}")
            print(f"Predicted Velocity: {predicted_vel:.2f}")
            print(f"--- End Inference Example ---")
        else:
            print("\n--- Inference Example Failed ---")
    else:
         print("\nCould not load historical data for inference example.")
