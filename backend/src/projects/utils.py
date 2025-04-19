from sqlalchemy.orm import Session

from src.projects.models.project import Project
from src.projects.models.project_configuration import ProjectConfiguration
from src.projects.schemas.project_with_config import ProjectWithConfig


def get_all_projects(db: Session):
    return db.query(Project).all()


def get_project_with_config(db: Session, project_id: int):
    return db.query(
        Project,
        ProjectConfiguration
    ).join(
        ProjectConfiguration,
        Project.id == ProjectConfiguration.id
    ).filter(
        Project.id == project_id
    ).first()


def add_project(db: Session, project: ProjectWithConfig) -> None:
    db_project = Project(name=project.name, description=project.description)
    db.add(db_project)
    db.refresh(db_project)

    db_project_config = ProjectConfiguration(
        api_token=project.api_token,
        project_key=project.project_key,
        project=db_project.id
    )

    db.add(db_project_config)

    db.commit()
