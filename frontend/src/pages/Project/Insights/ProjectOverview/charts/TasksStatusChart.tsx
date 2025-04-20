import PieChart from "../../../../../components/PieChart/PieChart.tsx";

export const tasksStatus = [
  {
    id: 1,
    task: 2016,
    status: 'To do',
    number: 3
  },{
    id: 1,
    task: 2016,
    status: 'Completed',
    number: 4
  },{
    id: 1,
    task: 2016,
    status: 'In Progress',
    number: 7
  },
];

const labels =  tasksStatus.map((task: any) => task?.status)
const data = tasksStatus.map((task: any) => task?.number)
const pieChartData = {
  labels: labels,
  datasets: [{
    data: data,
    backgroundColor: [
      '#1A3396',
      '#89CFF0',
      '#D9D9D9'
    ],
    hoverOffset: 4
  }]
};


const TasksStatusChart = () => {
  return (
    <div style={{width: '200px'}}>
      <PieChart chartData={pieChartData}/>
    </div>
  );
};

export default TasksStatusChart;
