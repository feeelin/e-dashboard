from sqlalchemy import ForeignKey, Integer, String, Column

from src.database import Base


class ProjectConfiguration(Base):
    __tablename__ = "projects_configurations"

    id = Column(Integer, primary_key=True, autoincrement=True)
    api_token = Column(String, unique=True, nullable=False)
    project_key = Column(String, unique=True, nullable=False)
    project = Column(ForeignKey("projects.id"), nullable=False)
