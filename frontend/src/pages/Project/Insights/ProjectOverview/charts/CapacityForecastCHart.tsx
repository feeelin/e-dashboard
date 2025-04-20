import LineChart from "../../../../../components/LineChart/LineChart.tsx";

export const capacityData = [
  {
    id: 1,
    task: 2016,
    status: 'To do',
    number: 3000
  },{
    id: 1,
    task: 2016,
    status: 'Completed',
    number: 4000
  },{
    id: 1,
    task: 2016,
    status: 'In Progress',
    number: 7000
  },
];

const labels =  capacityData.map((task: any) => task?.status)
const data = capacityData.map((task: any) => task?.number)
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
    <div style={{height: '200px'}}>
      <LineChart chartData={pieChartData}/>
    </div>
  );
};

export default TasksStatusChart;
