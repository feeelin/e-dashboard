import {Header} from "./components/Header/Header.tsx";
import logo from '../public/logo.svg';
import {
  BrowserRouter as Router,
  Route,
  Link, Routes
} from "react-router-dom";
import PageContent from "./components/PageContent/PageContent.tsx";
import Homepage from "./pages/Homepage/Homepage.tsx";
import ProjectOverview from "./pages/Project/Insights/ProjectOverview/ProjectOverview.tsx";

const App = () => {
  return (
    <div>
      <Header />
      <PageContent />

      {/*<Homepage/>*/}
      {/*<ProjectOverview />*/}
    </div>
  );
};

export default App;
