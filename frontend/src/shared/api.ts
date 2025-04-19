
export const getProjectData = async (id) => {
  const data = await fetch(`http://localhost:3000/${id}`);
  return data.json()
}

export const getAllProjects = async () => {

  const link = 'http://localhost:3000/projectsData';
  const data = await fetch(link);
  return data.json()

}
