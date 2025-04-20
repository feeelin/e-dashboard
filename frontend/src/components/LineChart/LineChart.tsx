import { Line } from "react-chartjs-2";
import { Chart as ChartJS, Tooltip, Legend } from "chart.js/auto";

ChartJS.register(
  Tooltip, Legend
)

function LineChart({ chartData }) {
  const options = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom',
      },
      tooltip: Tooltip
    }
  }
  return <Line data={chartData} options={options} />;
}

export default LineChart;


