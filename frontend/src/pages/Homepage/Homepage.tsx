import {Table} from 'antd'
import'./styles.scss'
import {Link} from "react-router-dom";
import {mockProjectsData} from "../../mockData/mockProjectsData.ts";
import {getAllProjects} from "../../shared/api.ts";

const projects = getAllProjects()
  .then(r => r.estimationSummary)


const columns = [
  {
    title: 'Name',
    dataIndex: 'name',
    key: 'name',
    render: (text, {name}) => (
      <>
      <Link to={`:${name}`}>{name}</Link>
      </>
    )
  },
  {
    title: 'Planned vs Actual',
    dataIndex: 'estimationSummary',
    key: 'estimationSummary',
  },
  {
    title: 'Progress',
    dataIndex: 'progress',
    key: 'progress',
  },
  {
    title: 'Last Activity',
    dataIndex: 'lastActivity',
    key: 'lastActivity',
  },
  {
    title: 'Status',
    dataIndex: 'projectStatus',
    key: 'projectStatus',
  },
]

const projectsCount = mockProjectsData.length;

const Homepage = () => {
  return (
    <div>
      <h3 className={'title'}>{projectsCount} {projectsCount > 1 ? 'Projects': 'Project' }</h3>
      <div className={'table'}>
        <Table dataSource={mockProjectsData} columns={columns}></Table>
      </div>
    </div>
  );
};

export default Homepage;
