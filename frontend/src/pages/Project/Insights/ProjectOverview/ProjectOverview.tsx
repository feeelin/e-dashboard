import './styles.scss'
import {Card, Flex, Space} from "antd";
import CheckCircleOutlined from "@ant-design/icons/CheckCircleOutlined";
import ExclamationCircleOutlined from "@ant-design/icons/ExclamationCircleOutlined";
import FireOutlined from "@ant-design/icons/FireOutlined";
import WarningOutlined from "@ant-design/icons/WarningOutlined";
import {getAllProjects} from "../../../../shared/api.ts";
import PieChart from "../../../../components/PieChart/PieChart.tsx";
import {useState} from "react";
import TasksStatusChart from "./charts/TasksStatusChart.tsx";
import CapacityForecastCHart from "./charts/CapacityForecastCHart.tsx";
import ProgressBar from "./charts/ProgressBar.tsx";


const ProjectOverview = () => {

  console.log(getAllProjects())
  const completed = 3;


  return (
    <div>
      <Flex justify="flex-start" gap={20}>
        <Card className={'card-top'} title={'Completed tasks'} >
          <Space>
            <CheckCircleOutlined />
            <span className={'digit'}>{completed}</span>
          </Space>
        </Card>

        <Card className={'card-top'} title={'Incomplete tasks'} >
          <Space>
            <ExclamationCircleOutlined />
            <span className={'digit'}>{completed}</span>
          </Space>
        </Card>

        <Card className={'card-top'} title={'Overdue tasks'} >
          <Space>
            <FireOutlined />
            <span className={'digit'}>{completed}</span>
          </Space>
        </Card>
      </Flex>


      <Flex justify="space-between" gap={20}>
      <Card className={'card'} title={'Time to Market'} >
        <p className={'digit'}>50h</p>
        <ProgressBar />
      </Card>

      <Card className={'card'} title={'Task Lifecycle'} >
        <p className={'digit'}>10h</p>
        <ProgressBar />
      </Card>

        <Card className={'card'} title={'Capacity Forecast'} >
          <CapacityForecastCHart/>
        </Card>
      </Flex>

      <Flex justify="space-between" gap={20}>
        <Card className={'card'} title={'Bugs after release'}>
          <p className={'digit'}>10 bugs</p>
        </Card>

        <Card className={'card'} title={'Status Overview'}>
          <p className={'digit'}>10 bugs</p>
          <TasksStatusChart />

        </Card>

        <Card className={'card'} title={'Project Deadline'}>
          <Flex justify="space-between" gap={20}>
            <Card className={'card-top'} >
              <Space>
                <WarningOutlined />
                <span>Overdue time</span>
                <p className={'digit'}>10h</p>
              </Space>
            </Card>
            <Card className={'card-top'} >
              <Space>
                <WarningOutlined />
                <span>Overdue tasks</span>
                <p className={'digit'}>7</p>
              </Space>
            </Card>
          </Flex>
        </Card>
      </Flex>


    </div>
  );
};

export default ProjectOverview;
