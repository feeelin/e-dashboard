import numpy as np
from sklearn.metrics import mean_absolute_error, mean_squared_error

def calculate_regression_metrics(y_true, y_pred):
    """Calculates standard regression metrics."""
    y_true = np.array(y_true).flatten()
    y_pred = np.array(y_pred).flatten()

    # Handle potential NaNs or infinite values if necessary
    valid_indices = np.isfinite(y_true) & np.isfinite(y_pred)
    if not np.all(valid_indices):
        print(f"Warning: Found {len(y_true) - valid_indices.sum()} non-finite values. Excluding them from metrics calculation.")
        y_true = y_true[valid_indices]
        y_pred = y_pred[valid_indices]

    if len(y_true) == 0:
        print("Warning: No valid samples to calculate metrics.")
        return {'mae': np.nan, 'rmse': np.nan, 'mape': np.nan}

    mae = mean_absolute_error(y_true, y_pred)
    rmse = np.sqrt(mean_squared_error(y_true, y_pred))

    # Calculate carefully to avoid division by zero
    # Replace true zeros with a small epsilon or exclude them
    mask = y_true != 0
    if np.any(mask):
        mape = np.mean(np.abs((y_true[mask] - y_pred[mask]) / y_true[mask])) * 100
    else:
        mape = np.nan # Or infinity, depending on convention

    return {'mae': mae, 'rmse': rmse, 'mape': mape}

if __name__ == '__main__':
    true = [10, 20, 30, 40, 50]
    pred = [12, 18, 33, 38, 55]
    metrics = calculate_regression_metrics(true, pred)
    print(f"Sample Metrics: {metrics}")

    true_with_zero = [0, 10, 20]
    pred_with_zero = [1, 11, 19]
    metrics_zero = calculate_regression_metrics(true_with_zero, pred_with_zero)
    print(f"Sample Metrics with Zero: {metrics_zero}")
