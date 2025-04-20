import pandas as pd
import numpy as np
from datetime import datetime, timedelta

def generate_mock_data(num_sprints=30, issues_per_sprint=15, start_date="2023-01-09"):
    """Generates mock Jira-like sprint and issue data."""
    print(f"Generating mock data for {num_sprints} sprints...")
    sprints = []
    issues = []
    status_transitions = []
    current_sprint_id = 1
    current_issue_id = 1
    sprint_start_date = datetime.strptime(start_date, "%Y-%m-%d")

    # Define status progression times (simplistic)
    time_to_inprogress = timedelta(days=1, hours=4)
    time_to_done = timedelta(days=5, hours=2) # Relative to 'In Progress' start

    for i in range(num_sprints):
        sprint_end_date = sprint_start_date + timedelta(days=13) # 2 week sprints
        is_closed = (sprint_end_date < datetime.now())
        sprint_state = 'closed' if is_closed else ('active' if sprint_start_date <= datetime.now() <= sprint_end_date else 'future')
        completion_date = sprint_end_date + timedelta(days=np.random.randint(0,2)) if is_closed else pd.NaT # Add slight variance

        sprints.append({
            "sprint_id": current_sprint_id,
            "name": f"Sprint {current_sprint_id}",
            "start_date": sprint_start_date,
            "end_date": sprint_end_date,
            "completed_date": completion_date,
            "state": sprint_state,
            "goal": f"Goal for Sprint {current_sprint_id}"
        })

        num_actual_issues = np.random.randint(int(issues_per_sprint * 0.8), int(issues_per_sprint * 1.2))
        for j in range(num_actual_issues):
            issue_type = np.random.choice(["Story", "Task", "Bug"], p=[0.6, 0.3, 0.1])
            created_date = sprint_start_date - timedelta(days=np.random.randint(1, 20)) # Created before sprint start
            story_points = np.random.choice([1, 2, 3, 5, 8, 13, None], p=[0.1, 0.2, 0.3, 0.2, 0.1, 0.05, 0.05]) if issue_type != "Bug" else None

            # Simulate status and resolution based on whether sprint is closed
            final_status = "To Do"
            resolved_date = pd.NaT
            status_times = {"created": created_date}

            if is_closed and np.random.rand() < 0.9: # 90% chance issues in closed sprints are done
                final_status = np.random.choice(["Done", "Resolved", "Closed"], p=[0.8, 0.1, 0.1])
                in_progress_start = sprint_start_date + timedelta(days=np.random.randint(0, 5), hours=np.random.randint(0, 8))
                resolved_date = in_progress_start + timedelta(days=np.random.randint(1, 10), hours=np.random.randint(0, 8))
                # Ensure resolved date is within reasonable bounds
                resolved_date = min(resolved_date, sprint_end_date + timedelta(days=2))
                resolved_date = max(resolved_date, in_progress_start + timedelta(hours=1))

                status_times["inprogress_start"] = in_progress_start
                status_times["done_start"] = resolved_date

            elif is_closed: # 10% chance issues in closed sprints are not done
                final_status = np.random.choice(["To Do", "In Progress"])
                if final_status == "In Progress":
                     status_times["inprogress_start"] = sprint_start_date + timedelta(days=np.random.randint(0, 10), hours=np.random.randint(0, 8))

            elif sprint_state == 'active' and np.random.rand() < 0.5: # Issues in active sprint
                 final_status = np.random.choice(["In Progress", "In Review"])
                 status_times["inprogress_start"] = sprint_start_date + timedelta(days=np.random.randint(0, (datetime.now() - sprint_start_date).days + 1 ), hours=np.random.randint(0, 8))

            issues.append({
                "issue_id": current_issue_id,
                "issue_key": f"PROJ-{current_issue_id}",
                "project_key": "PROJ",
                "issuetype": issue_type,
                "status": final_status,
                "resolution": "Done" if final_status in ["Done", "Resolved", "Closed"] else None,
                "summary": f"Summary for issue {current_issue_id}",
                "assignee": f"user_{np.random.randint(1, 6)}",
                "reporter": f"user_{np.random.randint(1, 6)}",
                "created_date": created_date,
                "resolved_date": resolved_date,
                "story_points": story_points,
                "sprint_id": current_sprint_id, # Simple association for mock data
            })

            # Generate simplified status transitions
            status_transitions.append({"issue_id": current_issue_id, "field": "status", "from_status": None, "to_status": "To Do", "timestamp": created_date})
            if "inprogress_start" in status_times:
                status_transitions.append({"issue_id": current_issue_id, "field": "status", "from_status": "To Do", "to_status": "In Progress", "timestamp": status_times["inprogress_start"]})
            if "done_start" in status_times:
                 status_transitions.append({"issue_id": current_issue_id, "field": "status", "from_status": "In Progress", "to_status": final_status, "timestamp": status_times["done_start"]})

            current_issue_id += 1

        # Move to next sprint
        sprint_start_date = sprint_end_date + timedelta(days=1) # Start next day
        current_sprint_id += 1

    sprints_df = pd.DataFrame(sprints)
    issues_df = pd.DataFrame(issues)
    status_transitions_df = pd.DataFrame(status_transitions)

    # Convert types
    for df in [sprints_df, issues_df, status_transitions_df]:
        for col in df.columns:
            if 'date' in col or 'timestamp' in col:
                df[col] = pd.to_datetime(df[col], errors='coerce')

    print(f"Generated {len(sprints_df)} sprints and {len(issues_df)} issues.")
    return sprints_df, issues_df, status_transitions_df

if __name__ == '__main__':
    s_df, i_df, st_df = generate_mock_data()
    print("\nSprints DataFrame Head:")
    print(s_df.head())
    print("\nIssues DataFrame Head:")
    print(i_df.head())
    print("\nStatus Transitions DataFrame Head:")
    print(st_df.head())
    print("\nSprints DataFrame Info:")
    s_df.info()
    print("\nIssues DataFrame Info:")
    i_df.info()
    print("\nStatus Transitions DataFrame Info:")
    st_df.info()
