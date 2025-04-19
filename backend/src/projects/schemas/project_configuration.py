from pydantic import BaseModel


class ProjectConfiguration(BaseModel):
    api_token: str
    project_key: str

    class Config:
        orm_mode = True
