import { Pie } from "react-chartjs-2";
import { Chart as ChartJS, Tooltip, Legend, ArcElement } from "chart.js/auto";

ChartJS.register(
  Tooltip, Legend, ArcElement
)

function PieChart({ chartData }) {
  const options = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom',
      },
      tooltip: Tooltip
    }
  }
  return <Pie data={chartData} options={options} />;
}

export default PieChart;


