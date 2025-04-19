import {BrowserRouter, useParams} from "react-router-dom";
import App from "../App.tsx";
import {
  BrowserRouter as Router,
  Route,
  Link, Routes
} from "react-router-dom";
import Homepage from "../pages/Homepage/Homepage.tsx";
import ProjectOverview from "../pages/Project/Insights/ProjectOverview/ProjectOverview.tsx";
import TaskStatus from "../pages/Project/Insights/TaskStatus/TaskStatus.tsx";
import Team from "../pages/Project/Insights/Team/Team.tsx";
import TaskList from "../pages/Project/TaskList/TaskList.tsx";
import Project from "../pages/Project/Project.tsx";


// get data source for a clicked project by id? name?

const AppRoutes = () => {
  let {name} = useParams()
  return (
    <Routes>
      <Route path="/" element={<Homepage />} />
      {/*<Route path="/projects" component={<Homepage />} />*/}
      <Route path="/:id" element={<Project />} />
      <Route path="/taskstatus" element={<TaskStatus />} />
      <Route path="/team" element={<Team />} />
      <Route path="/tasklist" element={<TaskList />} />
    </Routes>
  );
};

export default AppRoutes;
