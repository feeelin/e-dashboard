import {Tabs, TabsProps} from "antd";
import TaskList from "../TaskList/TaskList.tsx";
import ProjectOverview from "./ProjectOverview/ProjectOverview.tsx";
import TaskStatus from "./TaskStatus/TaskStatus.tsx";
import Team from "./Team/Team.tsx";
import './styles.scss'

const items: TabsProps['items'] = [
  {
    key: '1',
    label: 'Project Overview',
    children: <ProjectOverview />,
  },
  {
    key: '2',
    label: 'Task Status',
    children: <TaskStatus />,
  },
  {
    key: '3',
    label: 'Team',
    children: <Team />,
  },
];

const Insights = () => {
  return (
    <div className={'menu'}>
      <Tabs defaultActiveKey="1" type={'card'} items={items}></Tabs>
    </div>
  );
};

export default Insights;
