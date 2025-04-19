import {Flex, Tabs, TabsProps} from "antd";
import TaskList from "./TaskList/TaskList.tsx";
import Insights from "./Insights/Insights.tsx";
import './styles.scss'
import {ArrowLeftOutlined} from "@ant-design/icons";
import {useNavigate} from "react-router-dom";

const items: TabsProps['items'] = [
  {
    key: '1',
    label: 'Task List',
    children: <TaskList />,
  },
  {
    key: '2',
    label: 'Insights',
    children: <Insights />,
  },
];

const Project = () => {
  const navigateTo = useNavigate();

  return (
    <div>
      <div className={'wrap'}>
        <ArrowLeftOutlined onClick={() => navigateTo('/')}/>
        <p className={'titleProject'}>Eureka startup</p>
      </div>

      <div className='tabs'>
        <Tabs defaultActiveKey="2" items={items}></Tabs>
      </div>
    </div>
  );
};

export default Project;
