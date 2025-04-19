from pydantic import BaseModel


class ProjectWithConfig(BaseModel):
    name: str
    description: str
    api_token: str
    project_key: str
