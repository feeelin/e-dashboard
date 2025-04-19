from pydantic import BaseModel


class Project(BaseModel):
    name: str
    description: str

    class Config:
        orm_mode = True
