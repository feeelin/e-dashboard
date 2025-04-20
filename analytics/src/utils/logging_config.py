import logging
import sys
from pathlib import Path
from src.utils.config import load_config

def setup_logging():
    """Sets up basic logging."""
    cfg = load_config()
    log_path = Path(cfg.get('paths', {}).get('log_file', 'logs/forecast.log'))
    log_path.parent.mkdir(parents=True, exist_ok=True)

    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s [%(levelname)s] %(name)s: %(message)s",
        handlers=[
            logging.FileHandler(log_path),
            logging.StreamHandler(sys.stdout)
        ]
    )
    logging.info("Logging setup complete.")

if __name__ == '__main__':
    setup_logging()
    logging.getLogger().info("Testing logging setup.")
