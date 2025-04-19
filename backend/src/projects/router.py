from fastapi import APIRouter, HTTPException

import src.projects.utils as utils
from src.database import SessionLocal

from src.projects.schemas.project import Project
from src.projects.schemas.project_with_config import ProjectWithConfig

router = APIRouter(prefix="/projects", tags=["projects"])


@router.get("/", response_model=list[Project])
def get_all():
    with SessionLocal() as session:
        projects = utils.get_all_projects(session)
    return projects


@router.get("/{project_id}", response_model=ProjectWithConfig)
def get_by_id(project_id: int):
    with SessionLocal() as session:
        project = utils.get_project_with_config(session, project_id)

    if project is None:
        raise HTTPException(status_code=404, detail="Project not found")

    return project


@router.post("/", response_model=ProjectWithConfig)
def add_project(project: ProjectWithConfig):
    with SessionLocal() as session:
        session.expire_on_commit = False
        utils.add_project(session, project)

    return project
