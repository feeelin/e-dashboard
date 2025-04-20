import yaml
from pathlib import Path

CONFIG_PATH = Path(__file__).parent.parent.parent / "config/config.yaml"

def load_config(config_path=CONFIG_PATH):
    """Loads the YAML configuration file."""
    try:
        with open(config_path, 'r') as f:
            config = yaml.safe_load(f)
        print(f"Configuration loaded successfully from {config_path}")
        return config
    except FileNotFoundError:
        print(f"Error: Configuration file not found at {config_path}")
        return None
    except Exception as e:
        print(f"Error loading configuration from {config_path}: {e}")
        return None

# Load config globally on import if needed often, or load explicitly
# config = load_config()

if __name__ == '__main__':
    cfg = load_config()
    if cfg:
        print("\nConfig Content Example:")
        print(f"Story Point Field: {cfg.get('jira_field_names', {}).get('story_points')}")
        print(f"Done Statuses: {cfg.get('status_mapping', {}).get('done')}")
        print(f"Model Output Path: {Path(cfg.get('paths', {}).get('models_dir', 'models'))}")
