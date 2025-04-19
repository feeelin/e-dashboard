import {Card, Flex, Space} from "antd";
import TeamOutlined from "@ant-design/icons/TeamOutlined";
import ExclamationCircleOutlined from "@ant-design/icons/ExclamationCircleOutlined";
import MoonOutlined from "@ant-design/icons/MoonOutlined";

const Team = () => {
  const completed = 4;
  return (
    <div>
      <Flex justify="flex-start" gap={20}>
        <Card className={'card-top'} title={'Project Members'} >
          <Space>
            <TeamOutlined />
            <span className={'digit'}>{completed}</span>
          </Space>
        </Card>

        <Card className={'card-top'} title={'Assignees with overdue tasks'} >
          <Space>
            <ExclamationCircleOutlined />
            <span className={'digit'}>{completed}</span>
          </Space>
        </Card>

        <Card className={'card-top'} title={'Unassigned Members'} >
          <Space>
            <MoonOutlined />
            <span className={'digit'}>{completed}</span>
          </Space>
        </Card>
      </Flex>

      <Flex justify="flex-start" gap={40}>
        <Card className={'card'} title={'Team efficiency'} >
          <span className={'digit'}>{completed}h</span>
        </Card>
        <Card className={'card'} title={'Team capacity'} >
          <span className={'digit'}>{completed}</span>
        </Card>

        <Card className={'card'} title={'Workload'} >
          <span className={'digit'}>{completed} tasks/per member</span>
        </Card>
      </Flex>

    </div>
  );
};

export default Team;
