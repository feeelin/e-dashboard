import pandas as pd
import numpy as np
import logging
import joblib
from pathlib import Path
from sklearn.model_selection import train_test_split
from sklearn.ensemble import GradientBoostingRegressor
from sklearn.metrics import mean_absolute_error

from src.utils.config import load_config
from src.evaluation.metrics import calculate_regression_metrics

logger = logging.getLogger(__name__)

def time_series_split(df, date_column='start_date', n_splits=5):
    """Generates indices for time series cross-validation.
       Yields (train_indices, val_indices) for each split.
       Simplistic approach: splits data into n_splits chunks by date.
    """
    df_sorted = df.sort_values(date_column)
    total_samples = len(df_sorted)
    fold_size = total_samples // (n_splits + 1) # Approximate size, ensure > 0

    if fold_size == 0:
        logger.warning(f"Not enough data for {n_splits} splits. Using leave-one-out.")
        for i in range(1, total_samples):
            train_indices = df_sorted.index[:i]
            val_indices = df_sorted.index[i:i+1]
            if not val_indices.empty:
                yield train_indices, val_indices
        return

    current_end_idx = 0
    for i in range(n_splits):
        start_idx = 0 # Always train from the beginning
        end_idx = (i + 1) * fold_size
        val_start_idx = end_idx
        val_end_idx = end_idx + fold_size if i < n_splits - 1 else total_samples # Last fold takes remainder

        if val_start_idx >= total_samples: break # No more validation data

        train_indices = df_sorted.index[start_idx:end_idx]
        val_indices = df_sorted.index[val_start_idx:val_end_idx]

        if not train_indices.empty and not val_indices.empty:
             logger.info(f"Split {i+1}: Train size={len(train_indices)}, Val size={len(val_indices)}")
             yield train_indices, val_indices

def train_velocity_model(features_df, config):
    """Trains the velocity forecasting model."""
    model_cfg = config.get('velocity_model', {})
    target_metric = model_cfg.get('target_metric', 'actual_velocity')
    # Use features defined in config OR define a default list
    feature_cols = model_cfg.get('features', [
        'avg_velocity_last_1_sprints', 'avg_velocity_last_3_sprints',
        'planned_story_points', 'planned_issue_count'
    ])

    # Filter for relevant data: closed sprints with non-null target and features
    # Only train on sprints where we HAVE the actual velocity
    train_data = features_df[features_df['state'] == 'closed'].copy()
    train_data = train_data.dropna(subset=[target_metric] + feature_cols)

    if train_data.empty:
        logger.error("No valid training data found after filtering NaNs. Cannot train model.")
        return None, None, []

    logger.info(f"Training data shape after filtering: {train_data.shape}")
    logger.info(f"Using features: {feature_cols}")
    logger.info(f"Using target: {target_metric}")


    X = train_data[feature_cols]
    y = train_data[target_metric]

    # --- Time Series Cross-Validation ---
    logger.info("Performing Time Series Cross-Validation...")
    val_scores = []
    # Use the custom time series split
    for split_num, (train_idx, val_idx) in enumerate(time_series_split(train_data, date_column='start_date', n_splits=5)): # Use 5 splits or configure
        X_train, X_val = X.loc[train_idx], X.loc[val_idx]
        y_train, y_val = y.loc[train_idx], y.loc[val_idx]

        if X_train.empty or X_val.empty:
            logger.warning(f"Skipping split {split_num+1} due to empty train/val set.")
            continue

        # Initialize model (using params from config)
        model_params = model_cfg.get('model_params', {})
        model = GradientBoostingRegressor(**model_params)

        logger.info(f"Split {split_num+1}: Training on {len(X_train)}, Validating on {len(X_val)}")
        model.fit(X_train, y_train)
        y_pred = model.predict(X_val)

        metrics = calculate_regression_metrics(y_val, y_pred)
        val_scores.append(metrics['mae']) # Store MAE for averaging
        logger.info(f"Split {split_num+1} Val Metrics: {metrics}")

    avg_mae = np.mean(val_scores) if val_scores else np.nan
    logger.info(f"Average Validation MAE across splits: {avg_mae:.4f}")

    # --- Final Model Training ---
    logger.info("Training final model on all available closed sprint data...")
    final_model = GradientBoostingRegressor(**model_params)
    final_model.fit(X, y) # Train on the whole filtered dataset
    logger.info("Final model trained.")

    # Return the trained model and the list of features used
    return final_model, feature_cols, avg_mae


if __name__ == '__main__':
    from src.utils.logging_config import setup_logging
    setup_logging()

    cfg = load_config()
    if cfg:
        features_path = Path(cfg['paths']['features_dir']) / "model_input_features.parquet"
        models_dir = Path(cfg['paths']['models_dir'])
        models_dir.mkdir(parents=True, exist_ok=True)
        model_path = models_dir / "velocity_model.joblib"
        features_list_path = models_dir / "velocity_model_features.joblib"

        if features_path.exists():
            features_data = pd.read_parquet(features_path)
            trained_model, features_used, validation_mae = train_velocity_model(features_data, cfg)

            if trained_model:
                logger.info(f"Saving trained model to {model_path}")
                joblib.dump(trained_model, model_path)
                logger.info(f"Saving feature list to {features_list_path}")
                joblib.dump(features_used, features_list_path)
                logger.info(f"Training complete. Average Validation MAE: {validation_mae:.4f}")
            else:
                logger.error("Model training failed.")
        else:
            logger.error(f"Features file not found at {features_path}. Cannot train model.")
    else:
        logger.error("Could not load configuration.")
