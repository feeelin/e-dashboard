import { Bar } from "react-chartjs-2";
import { Chart as ChartJS, Tooltip, Legend } from "chart.js/auto";

export const capacityData = [
  {
    id: 1,
    task: 2016,
    status: 'Planned',
    number: 3000
  },{
    id: 1,
    task: 2016,
    status: 'Actual',
    number: 4000
  }
];

const labels =  capacityData.map((task: any) => task?.status)
const data = capacityData.map((task: any) => task?.number)

const progressBarData = {
    labels: labels,
    datasets: [{
      data: data,
      backgroundColor: 'rgba(75, 192, 192, 0.2)',
      borderColor: 'rgba(75, 192, 192, 1)',
      borderWidth: 1
    }]
  }

ChartJS.register(
  Tooltip, Legend
)

function ProgressBar({ chartData }) {
  const options = {
    indexAxis: 'y', // Это делает диаграмму горизонтальной
    scales: {
      x: {
        beginAtZero: true
      }
    }
  }
  return <Bar data={progressBarData} options={options} />;
}

export default ProgressBar;


