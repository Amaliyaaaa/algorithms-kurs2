using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace asd_kursach
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                int maxWeight = (int)numericUpDown2.Value;

                List<(string name, int weight, int value)> items = new List<(string, int, int)>();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null && row.Cells[2].Value != null)
                    {
                        string name = row.Cells[0].Value.ToString();
                        int weight = int.Parse(row.Cells[1].Value.ToString());
                        int value = int.Parse(row.Cells[2].Value.ToString());
                        items.Add((name, weight, value));
                    }
                }

                // Засекаем время начала работы алгоритма
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Замер памяти до создания таблицы
                long memoryBefore = GC.GetTotalMemory(true);

                // Запускаем алгоритм динамического программирования
                var result = SolveKnapsack(items, maxWeight);

                // Замер памяти после создания таблицы
                long memoryAfter = GC.GetTotalMemory(true);

                // Останавливаем таймер
                stopwatch.Stop();

                // Вычисляем объём используемой памяти
                long memoryUsed = memoryAfter - memoryBefore;
                if (memoryUsed < 0) memoryUsed = Math.Abs(memoryUsed);

                label3.Text = "Максимальная ценность: ";
                listBox1.Items.Clear();
                label3.Text += $"\n{result.totalValue}\n" + $"Выбранные предметы: ";
                foreach (var item in result.selectedItems)
                {
                    listBox1.Items.Add(item);
                }

                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                MessageBox.Show(
                    $"Время выполнения: {elapsedSeconds:F6} секунд\nОбъём дополнительно используемой памяти: {memoryUsed / 1024.0:F2} КБ",
                    "Результаты выполнения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        // Метод для решения задачи о рюкзаке
        private (int totalValue, List<string> selectedItems) SolveKnapsack(
            List<(string name, int weight, int value)> items, int maxWeight)
        {
            int n = items.Count;
            int[,] dp = new int[n + 1, maxWeight + 1];

            // Заполняем таблицу dp
            for (int i = 1; i <= n; i++)
            {
                for (int w = 0; w <= maxWeight; w++)
                {
                    if (items[i - 1].weight <= w)
                    {
                        dp[i, w] = Math.Max(dp[i - 1, w],
                                            dp[i - 1, w - items[i - 1].weight] + items[i - 1].value);
                    }
                    else
                    {
                        dp[i, w] = dp[i - 1, w];
                    }
                }
            }

            // Определяем выбранные предметы
            List<string> selectedItems = new List<string>();
            int totalValue = dp[n, maxWeight];
            int weightLeft = maxWeight;

            for (int i = n; i > 0 && weightLeft > 0; i--)
            {
                if (dp[i, weightLeft] != dp[i - 1, weightLeft])
                {
                    selectedItems.Add(items[i - 1].name);
                    weightLeft -= items[i - 1].weight;
                }
            }

            selectedItems.Reverse(); // Чтобы вывод был в порядке добавления
            return (totalValue, selectedItems);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            if (numericUpDown1.Value >= 5 && numericUpDown2.Value >= 5)
            {
                int things_quantity = Convert.ToInt32(numericUpDown1.Value);
                if (things_quantity <= 50)
                {
                    for (int i = 0; i < things_quantity; i++)
                    {
                        dataGridView1.Rows.Add(Global_Data_Things.Names[i], Global_Data_Things.Weights[i], Global_Data_Things.Values[i]);
                    }
                }
                else
                {
                    Random random1 = new Random();
                    for (int i = 0; i < things_quantity; i++)
                    {
                        string name = "Предмет " + (i + 1).ToString();
                        int weight = random1.Next(1, 100);
                        int value = random1.Next(1, 100000);
                        dataGridView1.Rows.Add(name, weight.ToString(), value.ToString());
                    }
                }
            }
        }
    }
}
