using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
namespace kursovoy
{
    public partial class Orders : Form
    {
        public Orders()
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; // Запрет на ввод в comboBox
        }

        /// <summary>
        /// Класс для хранения данных о заказе
        /// </summary>
        private class OrderData
        {
            public int OrderID { get; set; }
            public string OrderStatus { get; set; }

            // Хранение количества товаров в заказе
            public Dictionary<string, int> OrderItems { get; set; } = new Dictionary<string, int>();
        }

        // Список для хранения данных о заказах
        private List<OrderData> orderDataList = new List<OrderData>();

        public void FillDataGrid(string strCmd)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand command = new MySqlCommand(strCmd, con))
                    {
                        MySqlDataReader rdr = command.ExecuteReader();
                        for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                        {
                            dataGridView1.Rows[i].Visible = true;
                        }
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        orderDataList.Clear();
                        dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dataGridView1.AllowUserToResizeColumns = false;
                        dataGridView1.ReadOnly = false;
                        dataGridView1.AllowUserToAddRows = false;

                        dataGridView1.Columns.Add("OrderID", "Номер заказа");
                        dataGridView1.Columns.Add("OrderDate", "Дата заказа");
                        dataGridView1.Columns.Add("OrderUser", "Сотрудник");
                        dataGridView1.Columns.Add("OrderPrice", "Цена заказа");

                        // Создаем ComboBoxColumn для статуса заказа
                        DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn();
                        statusColumn.Name = "OrderStatus";
                        statusColumn.HeaderText = "Статус заказа";
                        statusColumn.Items.AddRange(new string[] { "Завершен", "Отменён" });
                        dataGridView1.Columns.Add(statusColumn);

                        dataGridView1.Columns["OrderID"].ReadOnly = true;
                        dataGridView1.Columns["OrderDate"].ReadOnly = true;
                        dataGridView1.Columns["OrderUser"].ReadOnly = true;
                        dataGridView1.Columns["OrderPrice"].ReadOnly = true;

                        while (rdr.Read())
                        {
                            int orderID = Convert.ToInt32(rdr["Номер заказа"]);
                            // Создаем объект OrderData и заполняем его данными
                            OrderData orderData = new OrderData
                            {
                                OrderID = orderID,
                                OrderStatus = rdr["Статус заказа"].ToString()
                            };
                            // Загружаем информацию о товарах в заказе
                            LoadOrderItems(orderID, orderData);

                            int rowIndex = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowIndex];
                            row.Cells["OrderID"].Value = rdr[0];
                            row.Cells["OrderDate"].Value = rdr[1];
                            row.Cells["OrderUser"].Value = rdr[3];
                            row.Cells["OrderPrice"].Value = rdr[4];
                            // Устанавливаем выбранное значение в ComboBoxColumn
                            row.Cells["OrderStatus"].Value = rdr[2];

                            orderDataList.Add(orderData);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }

        /// <summary>
        /// Загрузка информации о товарах в заказе
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="orderData"></param>
        private void LoadOrderItems(int orderID, OrderData orderData)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    con.Open();
                    string query = "SELECT ProductID, ProductCount FROM ProductOrder WHERE OrderID = @OrderID";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderID);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string productArticleNumber = reader["ProductID"].ToString();
                            int quantity = Convert.ToInt32(reader["ProductCount"]);
                            orderData.OrderItems.Add(productArticleNumber, quantity); 
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров для заказа {orderID}: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Кнопка "Назад"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            СommoditySpecialist CS = new СommoditySpecialist();
            CS.Show();
            this.Hide();
        }

        private void Orders_Load(object sender, EventArgs e)
        {
            FillDataGrid("SELECT OrderID AS 'Номер заказа',OrderDate AS 'Дата заказа'," +
                "OrderStatus AS 'Статус заказа',e.EmployeeFIO AS 'Продавец', OrderPrice AS 'Сумма заказа'" +
                " FROM `order`" +
                "INNER JOIN `Employee` e ON `order`.OrderUser = e.EmployeeID");
        }
        /// <summary>
        /// Поиск, сортировка, фильтр
        /// </summary>
        private void UpdateDataGrid() 
        {
            string searchStr = SearchText.Text;
            string sortOrder = comboBox1.SelectedItem?.ToString();
            string orderDirection = (sortOrder == "от нового к старому") ? "DESC" : "ASC";

            string strCmd = "SELECT OrderID AS 'Номер заказа',OrderDate AS 'Дата заказа'," +
                "OrderStatus AS 'Статус заказа',e.EmployeeFIO AS 'Продавец', OrderPrice AS 'Сумма заказа'" +
                " FROM `order`" +
                "INNER JOIN `Employee` e ON `order`.OrderUser = e.EmployeeID WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(searchStr))
            {
                strCmd += $" AND OrderID LIKE '%{searchStr}%'";
            }
            strCmd += $" ORDER BY `OrderDate` {orderDirection}";
            FillDataGrid(strCmd);
        }

        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Формирование отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePickerStart.Value;
            DateTime endDate = dateTimePickerEnd.Value;

            string startDate2 = dateTimePickerStart.Value.ToString("yyyy-MM-dd");
            string endDate2 = dateTimePickerEnd.Value.ToString("yyyy-MM-dd");
            // Проверка дат
            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала периода не может быть больше даты окончания периода.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    // Сохранение файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = $"Отчет по заказам{"_" + startDate2 + "_" + endDate2 + "_"}.xlsx";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel Files|*.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        connection.Open();

                        // Запрос для получения данных о заказах за указанный период
                        string query = @"SELECT OrderID AS 'Номер заказа',OrderDate AS 'Дата заказа'," +
                        "OrderStatus AS 'Статус заказа',e.EmployeeFIO AS 'Продавец', OrderPrice AS 'Сумма заказа'" +
                        " FROM `order`" +
                        "INNER JOIN `Employee` e ON `order`.OrderUser = e.EmployeeID WHERE OrderDate BETWEEN @startDate AND @endDate AND OrderStatus = 'Завершен'";

                        MySqlCommand command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@startDate", startDate);
                        command.Parameters.AddWithValue("@endDate", endDate);

                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        adapter.Fill(dataTable);

                        // Подсчет общего дохода
                        int totalRevenue = dataTable.AsEnumerable().Sum(row => row.Field<int>("Сумма заказа"));

                        // Создание Excel-приложения и книги
                        Excel.Application excelApp = new Excel.Application();
                        Excel.Workbook workbook = excelApp.Workbooks.Add();
                        Excel.Worksheet worksheet = workbook.Sheets[1];

                        // Заголовки столбцов
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                        }

                        // Данные
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataTable.Columns.Count; j++)
                            {
                                worksheet.Cells[i + 2, j + 1] = dataTable.Rows[i][j].ToString();
                            }
                        }

                        // Итоговая сумма
                        int lastRow = dataTable.Rows.Count + 2;
                        worksheet.Cells[lastRow, 1] = "Общий доход:";
                        worksheet.Cells[lastRow, 2] = totalRevenue.ToString("F2");

                        // Авторазмер столбцов
                        worksheet.Columns.AutoFit();

                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сформирован и сохранен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Закрытие Excel
                        workbook.Close();
                        excelApp.Quit();

                        // Очистка COM-объектов
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                        excelApp = null;
                        workbook = null;
                        worksheet = null;
                        GC.Collect();
                        connection.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerEnd.MinDate = dateTimePickerStart.Value;
        }

        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerStart.MaxDate = dateTimePickerEnd.Value;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "OrderStatus")
            {
                int orderID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["OrderID"].Value);
                string newStatus = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                // Получаем старый статус заказа из списка данных
                string oldStatus = orderDataList.FirstOrDefault(o => o.OrderID == orderID)?.OrderStatus;

                // Проверяем, что статус изменился
                if (oldStatus != newStatus)
                {
                    // Проверяем, достаточно ли товара на складе, если меняем статус с "Отменён" на "Выполнен"
                    if (oldStatus == "Отменён" && newStatus == "Завершен")
                    {
                        if (!CheckStockAvailability(orderID))
                        {
                            MessageBox.Show("Недостаточно товара на складе для выполнения заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dataGridView1.Rows[e.RowIndex].Cells["OrderStatus"].Value = oldStatus; // Возвращаем старый статус
                            return; // Прекращаем выполнение метода
                        }
                    }

                    // Обновляем статус заказа в базе данных
                    UpdateOrderStatus(orderID, newStatus);

                    // Если статус изменился с "Выполнен" на "Отменён", возвращаем товар на склад
                    if (oldStatus == "Завершен" && newStatus == "Отменён")
                    {
                        ReturnProductsToStock(orderID);
                    }
                    // Если статус изменился с "Отменён" на "Выполнен", списываем товар со склада
                    else if (oldStatus == "Отменён" && newStatus == "Завершен")
                    {
                        DeductProductsFromStock(orderID);
                    }

                    // Обновляем статус заказа в списке данных
                    OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);
                    if (orderData != null)
                    {
                        orderData.OrderStatus = newStatus;
                    }

                }
            }
        }

        /// <summary>
        /// Проверка наличия товара на складе перед выполнением заказа
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        private bool CheckStockAvailability(int orderID)
        {
            OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);

            if (orderData == null)
            {
                MessageBox.Show($"Не найден заказ с ID {orderID}.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    con.Open();
                    foreach (var item in orderData.OrderItems)
                    {
                        string productArticleNumber = item.Key;
                        int quantity = item.Value;
                        // Проверяем, достаточно ли товара на складе
                        string query = "SELECT ProductQuantityInStock FROM Product WHERE ProductArticul = @ProductArticul";
                        MySqlCommand command = new MySqlCommand(query, con);
                        command.Parameters.AddWithValue("@ProductArticul", productArticleNumber);
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int stockQuantity = Convert.ToInt32(result);
                            if (stockQuantity < quantity)
                            {
                                return false; // Недостаточно товара на складе
                            }
                        }

                        else
                        {
                            MessageBox.Show($"Товар с артикулом {productArticleNumber} не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false; // Товар не найден
                        }
                    }
                    con.Close();
                }
                return true; // Достаточно товара на складе
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке наличия товара на складе: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Обновление статуса заказа в базе данных
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="newStatus"></param>
        private void UpdateOrderStatus(int orderID, string newStatus)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    con.Open();
                    string query = "UPDATE `Order` SET OrderStatus = @OrderStatus WHERE OrderID = @OrderID";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrderID", orderID);
                    cmd.Parameters.AddWithValue("@OrderStatus", newStatus);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Возврат товара на склад
        /// </summary>
        /// <param name="orderID"></param>
        private void ReturnProductsToStock(int orderID)
        {
            try
            {
                OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);
                if (orderData != null)
                {
                    using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        con.Open();
                        // Перебираем товары в заказе и возвращаем их на склад
                        foreach (var item in orderData.OrderItems)
                        {
                            string productArticleNumber = item.Key; 
                            int quantity = item.Value;
                            // Увеличиваем количество товара на складе
                            string query = "UPDATE Product SET ProductQuantityInStock = ProductQuantityInStock + @Quantity WHERE ProductArticul = @ProductArticul"; // Используем ProductArticleNumber
                            MySqlCommand command = new MySqlCommand(query, con);
                            command.Parameters.AddWithValue("@ProductArticul", productArticleNumber); 
                            command.Parameters.AddWithValue("@Quantity", quantity);
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при возврате товара на склад: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 

        /// <summary>
        /// Списание товара со склада (при изменении статуса с "Отменён" на "Выполнен")
        /// </summary>
        /// <param name="orderID"></param>
        private void DeductProductsFromStock(int orderID)
        {
            try
            {
                OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);
                if (orderData != null)
                {
                    using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        con.Open();
                        // Перебираем товары в заказе и списываем их со склада
                        foreach (var item in orderData.OrderItems)
                        {
                            string productArticleNumber = item.Key; 
                            int quantity = item.Value;
                            // Уменьшаем количество товара на складе
                            string query = "UPDATE Product SET ProductQuantityInStock = ProductQuantityInStock - @Quantity WHERE ProductArticul = @ProductArticul"; // Используем ProductArticleNumber
                            MySqlCommand command = new MySqlCommand(query, con);
                            command.Parameters.AddWithValue("@ProductArticul", productArticleNumber); 
                            command.Parameters.AddWithValue("@Quantity", quantity);
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при списании товара со склада: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверка на ввод только чисел
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Отменить ввод, если символ не является цифрой
            }
        }
    }
}

