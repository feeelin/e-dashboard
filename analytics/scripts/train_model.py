import logging
from pathlib import Path
import pandas as pd
import joblib

from src.utils.config import load_config
from src.utils.logging_config import setup_logging
from src.training.train_velocity import train_velocity_model

setup_logging()
logger = logging.getLogger(__name__)

def main():
    logger.info("Starting model training script...")
    cfg = load_config()
    if not cfg:
        logger.error("Failed to load configuration. Exiting.")
        return

    # --- Paths ---
    features_path = Path(cfg['paths']['features_dir']) / "model_input_features.parquet"
    models_dir = Path(cfg['paths']['models_dir'])
    models_dir.mkdir(parents=True, exist_ok=True)
    # Specific model path naming convention
    model_output_path = models_dir / "velocity_gbr_model.joblib"
    features_list_path = models_dir / "velocity_gbr_features.joblib"


    # --- Load Features ---
    if not features_path.exists():
        logger.error(f"Features file not found at {features_path}. Please run feature engineering first. Exiting.")
        return
    logger.info(f"Loading features from {features_path}")
    features_data = pd.read_parquet(features_path)

    # --- Train Model ---
    # Currently hardcoded to velocity model, expand later if needed
    logger.info("Training Velocity Model...")
    trained_model, features_used, validation_mae = train_velocity_model(features_data, cfg)

    # --- Save Model ---
    if trained_model and features_used:
        logger.info(f"Saving trained model to {model_output_path}")
        joblib.dump(trained_model, model_output_path)

        logger.info(f"Saving feature list used for training to {features_list_path}")
        joblib.dump(features_used, features_list_path)

        logger.info("-" * 30)
        logger.info(f"Training Summary:")
        logger.info(f"  Model Type: Gradient Boosting Regressor (Scikit-learn)")
        logger.info(f"  Features Used: {features_used}")
        logger.info(f"  Avg Validation MAE: {validation_mae:.4f}")
        logger.info(f"  Model saved to: {model_output_path}")
        logger.info(f"  Features list saved to: {features_list_path}")
        logger.info("-" * 30)
        logger.info("Model training script finished successfully.")
    else:
        logger.error("Model training failed. Model or features list not generated.")
        logger.info("Model training script finished with errors.")


if __name__ == "__main__":
    main()
