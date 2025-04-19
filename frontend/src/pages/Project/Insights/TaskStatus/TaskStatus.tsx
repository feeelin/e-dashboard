import {Card, Flex, Space, Table} from "antd";
import CheckCircleOutlined from "@ant-design/icons/CheckCircleOutlined";
import ExclamationCircleOutlined from "@ant-design/icons/ExclamationCircleOutlined";
import FireOutlined from "@ant-design/icons/FireOutlined";
import {Link} from "react-router-dom";
import {mockOverdueTasks} from "../../../../mockData/mockOverdueTasks.ts";

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
    title: 'Status',
    dataIndex: 'status',
    key: 'status',
  },
  {
    title: 'End Date',
    dataIndex: 'endDate',
    key: 'endDate',
  }
]

const TaskStatus = () => {
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
        <Card className={'card'} title={'Overdue tasks'} >
          <Table columns={columns} dataSource={mockOverdueTasks} pagination={false}/>
        </Card>
      </Flex>

      <Flex justify="space-between" gap={20}>
      <Card className={'card'} title={'Tasks completed early'} >
          <Table columns={columns} dataSource={mockOverdueTasks} pagination={false}/>
        </Card>

        <Card className={'card'} title={'Tasks completed late'} >
          <Table columns={columns} dataSource={mockOverdueTasks} pagination={false}/>
        </Card>

        <Card className={'card'} title={'Tasks completed in time'} >
          <Table columns={columns} dataSource={mockOverdueTasks} pagination={false}/>
        </Card>
      </Flex>
    </div>
  );
};

export default TaskStatus;
