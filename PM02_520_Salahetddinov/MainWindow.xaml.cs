using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PM02_520_Salahetddinov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для проверки, является ли строка числом
        private bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        // Метод для проверки, заполнено ли поле
        private bool IsFilled(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        // Метод для проверки валидности всех введенных значений
        private bool ValidateInput(string[] inputs)
        {
            foreach (string input in inputs)
            {
                if (!IsFilled(input) || !IsNumeric(input))
                {
                    return false;
                }
            }
            return true;
        }

        // Обработчик события для решения задачи методом минимальной стоимости
        private void SolveByMinimumCost(object sender, RoutedEventArgs e)
        {
            string[] demandInputs = txtConsumerDemand.Text.Split(',');
            string[] supplyInputs = txtSupplierSupply.Text.Split(',');
            string[] costRows = txtCostMatrix.Text.Split(';');

            if (!ValidateInput(demandInputs) || !ValidateInput(supplyInputs) || costRows.Any(row => !ValidateInput(row.Split(','))))
            {
                if (!ValidateInput(demandInputs) || !ValidateInput(supplyInputs))
                {
                    MessageBox.Show("Заполните все поля.");
                }
                else
                {
                    MessageBox.Show("Убедитесь, что все поля матрицы стоимостей заполнены числовыми значениями.");
                }
                return;
            }

            int[] demand = Array.ConvertAll(demandInputs, int.Parse);
            int[] supply = Array.ConvertAll(supplyInputs, int.Parse);

            int[][] costMatrix = new int[costRows.Length][];
            for (int i = 0; i < costRows.Length; i++)
            {
                costMatrix[i] = costRows[i].Split(',').Select(int.Parse).ToArray();
            }

            var (allocation, totalCost) = MinimumCostMethod(supply, demand, costMatrix);
            DisplayResult(allocation, totalCost);
        }

        // Метод для решения задачи методом минимальной стоимости
        static (int[][], int) MinimumCostMethod(int[] supply, int[] demand, int[][] costs)
        {
            int[][] allocation = new int[supply.Length][];
            for (int i = 0; i < supply.Length; i++)
            {
                allocation[i] = new int[demand.Length];
            }

            int[] supplyCopy = supply.ToArray();
            int[] demandCopy = demand.ToArray();
            int totalCost = 0;

            while (true)
            {
                int minCost = int.MaxValue;
                int minRow = -1, minCol = -1;

                for (int row = 0; row < supply.Length; row++)
                {
                    for (int col = 0; col < demand.Length; col++)
                    {
                        if (supplyCopy[row] > 0 && demandCopy[col] > 0)
                        {
                            if (costs[row][col] < minCost)
                            {
                                minCost = costs[row][col];
                                minRow = row;
                                minCol = col;
                            }
                        }
                    }
                }

                if (minRow == -1 || minCol == -1)
                {
                    break;
                }

                int x = Math.Min(supplyCopy[minRow], demandCopy[minCol]);
                allocation[minRow][minCol] = x;
                supplyCopy[minRow] -= x;
                demandCopy[minCol] -= x;
                totalCost += x * minCost;
            }

            return (allocation, totalCost);
        }

        // Метод для отображения результатов
        private void DisplayResult(int[][] allocation, int totalCost)
        {
            StringBuilder resultBuilder = new StringBuilder();

            resultBuilder.AppendLine("Опорный план:");
            for (int i = 0; i < allocation.Length; i++)
            {
                for (int j = 0; j < allocation[i].Length; j++)
                {
                    resultBuilder.Append(allocation[i][j]);
                    resultBuilder.Append("\t");
                }
                resultBuilder.AppendLine();
            }

            resultBuilder.AppendLine($"Общая стоимость: {totalCost}");

            txtSolution.Text = resultBuilder.ToString();
        }

        // Обработчик события для очистки полей ввода
        private void ClearFields(object sender, RoutedEventArgs e)
        {
            txtSupplierSupply.Clear();
            txtConsumerDemand.Clear();
            txtCostMatrix.Clear();
        }

        // Обработчик события для очистки поля с результатами
        private void ClearSolution(object sender, RoutedEventArgs e)
        {
            txtSolution.Clear();
        }
    }
}