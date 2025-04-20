import {Table} from "antd";
import {mockOverdueTasks} from "../../../mockData/mockOverdueTasks.ts";
import {Link} from "react-router-dom";

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

const TaskList = () => {
  return (
    <div>
      <Table columns={columns} dataSource={mockOverdueTasks} pagination={false}/>

    </div>
  );
};

export default TaskList;
