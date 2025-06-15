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
using System.Configuration;

namespace kursovoy
{
    public partial class Orders : Form
    {
        private FormWindowState _previousWindowState; // Сохраняем предыдущее состояние окна
        private FormBorderStyle _previousBorderStyle; // Сохраняем стиль границы
        private int currentPage1 = 0;
        private int rowsPerPage1 = 20;
        private int totalRows1 = 0;
        private int totalRecords; //кол-во строк всего
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();

        public Orders()
        {
            InitializeComponent();
            dateTimePickerStart.Value = DateTime.Now;
            dateTimePickerEnd.Value = DateTime.Now;
        }
        /// <summary>
        /// Класс для хранения данных о заказе
        /// </summary>
        private class OrderData
        {
            public int OrderID { get; set; }
            public string OrderStatus { get; set; }
            public DateTime OrderDate { get; set; }

            // Хранение количества товаров в заказе
            public Dictionary<string, int> OrderItems { get; set; } = new Dictionary<string, int>();
        }
        // Список для хранения данных о заказах
        private List<OrderData> orderDataList = new List<OrderData>();

        /// <summary>
        /// Загрузка данных в datagridview
        /// </summary>
        /// <param name="strCmd"></param>
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
                        allRows1.Clear();
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        orderDataList.Clear();

                        dataGridView1.AllowUserToDeleteRows = false;
                        dataGridView1.AllowUserToOrderColumns = false;
                        dataGridView1.AllowUserToResizeColumns = false;
                        dataGridView1.AllowUserToResizeRows = false;
                        dataGridView1.ReadOnly = false;
                        dataGridView1.AllowUserToAddRows = false;

                        dataGridView1.Columns.Add("OrderID", "Номер заказа");
                        dataGridView1.Columns.Add("OrderDate", "Дата заказа");
                        dataGridView1.Columns.Add("OrderUser", "Сотрудник");
                        dataGridView1.Columns.Add("OrderPrice", "Сумма заказа");
                        dataGridView1.Columns["OrderUser"].Width = 250;
                        dataGridView1.Columns["OrderDate"].Width = 150;
                        dataGridView1.Columns["OrderID"].Width = 80;
                        dataGridView1.Columns["OrderUser"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                        dataGridView1.Columns["OrderID"].ReadOnly = true;
                        dataGridView1.Columns["OrderDate"].ReadOnly = true;
                        dataGridView1.Columns["OrderUser"].ReadOnly = true;
                        dataGridView1.Columns["OrderPrice"].ReadOnly = true;

                        if (Authorization.User2.Role == 2)
                        {
                            DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn();
                            statusColumn.Name = "OrderStatus";
                            statusColumn.HeaderText = "Статус заказа";
                            statusColumn.Items.AddRange(new string[] { "Завершен", "Отменён" });
                            dataGridView1.Columns.Add(statusColumn);
                        }
                        if (Authorization.User2.Role == 3)
                        {
                            dataGridView1.Columns.Add("OrderStatus", "Статус заказа");
                            dataGridView1.Columns["OrderStatus"].ReadOnly = true;
                            dateTimePickerEnd.Visible = false;
                            dateTimePickerStart.Visible = false;
                            button2.Visible = false;
                            label1.Visible = false;
                            panel1.Visible = false;
                        }

                        while (rdr.Read())
                        {
                            int orderID = Convert.ToInt32(rdr["Номер заказа"]);
                            DateTime orderDate = Convert.ToDateTime(rdr["Дата заказа"]);

                            // Создаем объект OrderData и заполняем его данными
                            OrderData orderData = new OrderData
                            {
                                OrderID = orderID,
                                OrderStatus = rdr["Статус заказа"].ToString(),
                                OrderDate = orderDate
                            };
                            // Загружаем информацию о товарах в заказе
                            LoadOrderItems(orderID, orderData);

                            int rowIndex = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowIndex];
                            row.Cells["OrderID"].Value = rdr[0];
                            row.Cells["OrderDate"].Value = rdr[1];
                            row.Cells["OrderUser"].Value = string.Format("{0} {1} {2}", rdr[3], rdr[4], rdr[5]);
                            row.Cells["OrderPrice"].Value = rdr[6];
                            row.Cells["OrderStatus"].Value = rdr[2];
                            allRows1.Add(row);
                            orderDataList.Add(orderData);
                        }
                        totalRows1 = allRows1.Count;
                        con.Close();
                    }
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
            if (Authorization.User2.Role == 2)
            {
                СommoditySpecialist CS = new СommoditySpecialist();
                CS.Show();
                this.Close();
            }
            else if (Authorization.User2.Role == 3)
            {
                Seller sl = new Seller();
                sl.Show();
                this.Close();
            }
        }
        /// <summary>
        /// Событие при загрузке формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Orders_Load(object sender, EventArgs e)
        {
            // Масштабирование для роли Продавец
            if (Authorization.User2.Role == 3)
            {
                this.Size = new Size(905, 608);
                buttonPag1.Location = new System.Drawing.Point(75, 524);
                buttonPag2.Location = new System.Drawing.Point(106, 524);
                labelCount.Location = new System.Drawing.Point(75, 491);
                labelVSE.Location = new System.Drawing.Point(301, 491);
                label4.Location = new System.Drawing.Point(628, 492);
                button5.Location = new System.Drawing.Point(768, 495);
                dataGridView1.Size = new Size(740, 375);
            }
            UpdateDataGrid();
            UpdatePag();
            FillCount();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            labelVSE.Visible = false;
            label7.Text = Authorization.User2.RoleName + ": " + Authorization.User2.FIO;
        }

        /// <summary>
        /// Выбор страницы пагинации, скрытие ненужных строк
        /// </summary>
        private void LinkLabel_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;
            // узнаём какая страница выбрана
            LinkLabel l = sender as LinkLabel;
            if (l != null)
            {
                currentPage1 = Convert.ToInt32(l.Text) - 1;
                UpdatePag();
                FillCount();
            }
        }

        /// <summary>
        /// Пагинация
        /// </summary>
        private void UpdatePag()
        {
            dataGridView1.Rows.Clear();
            int startIndex = currentPage1 * rowsPerPage1;
            int endIndex = Math.Min(startIndex + rowsPerPage1, totalRows1);
            // Добавляем строки для текущей страницы
            for (int i = startIndex; i < endIndex; i++)
            {
                dataGridView1.Rows.Add(allRows1[i].Cells.Cast<DataGridViewCell>().Select(cell => cell.Value).ToArray());
            }
            // Удаляем старые LinkLabel страниц
            foreach (var control in this.Controls.OfType<LinkLabel>().Where(c => c.Name?.StartsWith("page") == true).ToList())
            {
                this.Controls.Remove(control);
            }
            // Рассчитываем общее количество страниц
            int totalPages = (int)Math.Ceiling((double)totalRows1 / rowsPerPage1);
            int step = 15;
            int x = labelCount.Location.X + 68;
            int y = labelCount.Location.Y + 38;
            // Создаем новые LinkLabel для страниц
            for (int i = 0; i < totalPages; i++)
            {
                var linkLabel = new LinkLabel();
                linkLabel.Text = (i + 1).ToString();
                linkLabel.Name = "page" + i;

                // Устанавливаем цвет ссылки
                linkLabel.LinkColor = Color.Black;      
                linkLabel.VisitedLinkColor = Color.Black; 
                linkLabel.ActiveLinkColor = Color.Black; 

                linkLabel.Font = new System.Drawing.Font(linkLabel.Font.FontFamily, 14);
                linkLabel.AutoSize = true;
                linkLabel.Location = new System.Drawing.Point(x, y);
                linkLabel.Click += LinkLabel_Click;
                linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;// Убираем подчеркивание

                // Добавляем фон только у текущей страницы
                if (i == currentPage1)
                {
                    linkLabel.BackColor = Color.LightGreen;
                }
                else
                {
                    linkLabel.BackColor = Color.Honeydew;
                }
                this.Controls.Add(linkLabel);
                x += step;
            }
            // Обновляем состояние кнопок
            buttonPag1.Enabled = currentPage1 > 0;
            buttonPag2.Enabled = currentPage1 < totalPages - 1;

            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
        }
        /// <summary>
        /// Количество строк всего
        /// </summary>
        public void FillCount()
        {
            string conStr = Authorization.Program.ConnectionString;
            using (MySqlConnection con = new MySqlConnection(conStr))
            {
                con.Open();
                using (MySqlCommand totalCommand = new MySqlCommand("SELECT COUNT(*) FROM `order`", con))
                {
                    totalRecords = Convert.ToInt32(totalCommand.ExecuteScalar());
                }
                labelVSE.Text = $"/{totalRecords}";
            }
        }
        /// <summary>
        /// Поиск, сортировка, фильтрация
        /// </summary>
        private void UpdateDataGrid()
        {
            string searchStr = SearchText.Text;
            string sortOrder = comboBox1.SelectedItem?.ToString();
            string orderDirection = (sortOrder == "сначало новые") ? "DESC" : "ASC";
            string strCmd = "SELECT OrderID AS 'Номер заказа',OrderDate AS 'Дата заказа'," +
                " OrderStatus AS 'Статус заказа', e.EmployeeF AS 'Фамилия', e.EmployeeI AS 'Имя', e.EmployeeO AS 'Отчеcтво', OrderPrice AS 'Сумма заказа'" +
                " FROM `order`" +
                "INNER JOIN `employeeee` e ON `order`.OrderUser = e.EmployeeID WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(searchStr))
            {
                strCmd += $" AND OrderID LIKE '%{searchStr}%'";
            }
            strCmd += $" ORDER BY `OrderDate` {orderDirection}";
            FillDataGrid(strCmd);
            currentPage1 = 0; // Сбрасываем на первую страницу
            UpdatePag(); // Обновляем пагинацию
        }
        /// <summary>
        /// Обработчик события изменения текста в поле поиска.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            FillCount();
        }
        /// <summary>
        /// Обработчик события изменения выбранного элемента в ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            FillCount();
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
                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = $"Отчет по заказам_{startDate2}_{endDate2}.xlsx";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel Files|*.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        con.Open();
                        string query = @"SELECT
                                        OrderID AS 'Номер заказа',
                                        OrderDate AS 'Дата заказа',
                                        OrderStatus AS 'Статус заказа',
                                        e.EmployeeF AS 'Фамилия',
                                        e.EmployeeI AS 'Имя',
                                        e.EmployeeO AS 'Отчество',
                                        OrderPrice AS 'Сумма заказа'
                                    FROM `order`
                                    INNER JOIN employeeee e ON `order`.OrderUser = e.EmployeeID
                                    WHERE OrderDate BETWEEN @startDate AND @endDate AND OrderStatus = 'Завершен'";
                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@startDate", startDate);
                            cmd.Parameters.AddWithValue("@endDate", endDate);
                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                            {
                                System.Data.DataTable dataTable = new System.Data.DataTable();
                                adapter.Fill(dataTable);

                                // Подсчет общего дохода
                                decimal totalRevenue = dataTable.AsEnumerable().Sum(row => Convert.ToDecimal(row["Сумма заказа"]));
                                int totalOrders = dataTable.Rows.Count;
                                double averagePrice = totalOrders > 0 ? (double)totalRevenue / totalOrders : 0;

                                // Создание Excel-приложения и книги
                                Excel.Application excelApp = new Excel.Application();
                                Excel.Workbook workbook = excelApp.Workbooks.Add();
                                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1]; 

                                var periodHeader = $"Отчет за период: {startDate.ToShortDateString()} - {endDate.ToShortDateString()}";
                                worksheet.Range["A1:G1"].Merge(); 
                                worksheet.Cells[1, 1] = periodHeader;
                                worksheet.Cells[1, 1].Font.Bold = true;
                                worksheet.Cells[1, 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                worksheet.Cells[1, 1].Font.Size = 14;

                                for (int i = 0; i < dataTable.Columns.Count; i++)
                                {
                                    worksheet.Cells[2, i + 1] = dataTable.Columns[i].ColumnName;
                                }

                                for (int i = 0; i < dataTable.Rows.Count; i++)
                                {
                                    for (int j = 0; j < dataTable.Columns.Count; j++)
                                    {
                                        worksheet.Cells[i + 3, j + 1] = dataTable.Rows[i][j].ToString();
                                    }
                                }
                                // Итоговая информация
                                int lastRow = dataTable.Rows.Count + 3;
                                worksheet.Cells[lastRow, 1] = "Общий доход:";
                                worksheet.Cells[lastRow, 7] = totalRevenue.ToString("F2");
                                worksheet.Cells[lastRow + 1, 1] = "Количество заказов:";
                                worksheet.Cells[lastRow + 1, 2] = totalOrders.ToString();
                                // Распределение по сотрудникам
                                var employeeDistribution = dataTable.AsEnumerable()
                                    .GroupBy(row => $"{row.Field<string>("Фамилия")} {row.Field<string>("Имя")} {row.Field<string>("Отчество")}")
                                    .Select(g => new
                                    {
                                        Employee = g.Key,
                                        Count = g.Count(),
                                        TotalPrice = g.Sum(row => Convert.ToDecimal(row["Сумма заказа"]))
                                    });
                                worksheet.Cells[lastRow + 3, 1] = "Сотрудник";
                                worksheet.Cells[lastRow + 3, 2] = "Количество заказов";
                                worksheet.Cells[lastRow + 3, 3] = "Сумма заказов";

                                int currentRow = lastRow + 4;
                                foreach (var employee in employeeDistribution)
                                {
                                    worksheet.Cells[currentRow, 1] = employee.Employee;
                                    worksheet.Cells[currentRow, 2] = employee.Count.ToString();
                                    worksheet.Cells[currentRow, 3] = employee.TotalPrice.ToString("F2");
                                    currentRow++;
                                }
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
                                con.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Устанавливаем минимальную дату для dateTimePickerEnd.
        /// Предотвращает выбор даты окончания раньше даты начала.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerEnd.MinDate = dateTimePickerStart.Value;
        }
        /// <summary>
        /// Устанавливаем максимальную дату для dateTimePickerStart.
        /// Предотвращает выбор даты начала позже даты окончания.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerStart.MaxDate = dateTimePickerEnd.Value;
        }
        /// <summary>
        /// Изменение статуса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "OrderStatus")
            {
                int orderID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["OrderID"].Value);
                string newStatus = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

                // Находим данные заказа в списке
                OrderData orderData = orderDataList.FirstOrDefault(o => o.OrderID == orderID);

                if (orderData != null)
                {
                    // Получаем статус заказа
                    string oldStatus = orderData.OrderStatus;

                    // Попытка изменения статуса с "Отменён" на "Завершен"
                    if (oldStatus == "Отменён" && newStatus == "Завершен")
                    {
                        MessageBox.Show("Изменение статуса заказа с 'Отменён' на 'Завершен' запрещено.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dataGridView1.Rows[e.RowIndex].Cells["OrderStatus"].Value = oldStatus; 
                        return;
                    }

                    // Проверяем, прошло ли больше 14 дней с даты заказа
                    if (DateTime.Now > orderData.OrderDate.AddDays(14) && newStatus == "Отменён")
                    {
                        MessageBox.Show("Нельзя отменить заказ, так как прошло больше 14 дней с даты заказа.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dataGridView1.Rows[e.RowIndex].Cells["OrderStatus"].Value = orderData.OrderStatus; 
                        return;
                    }
                    // Проверяем, что статус изменился
                    if (oldStatus != newStatus)
                    {
                        UpdateOrderStatus(orderID, newStatus);
                        // возвращаем товар на склад
                        if (oldStatus == "Завершен" && newStatus == "Отменён")
                        {
                            ReturnProductsToStock(orderID);
                        }
                        orderData.OrderStatus = newStatus;

                        UpdateDataGrid();
                        UpdatePag();
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
                            return false;
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
                            string query = "UPDATE Product SET ProductQuantityInStock = ProductQuantityInStock + @Quantity WHERE ProductArticul = @ProductArticul"; 
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
        /// Проверка на ввод только чисел в поиске
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Кнопка перехода на предыдущую страницу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPag1_Click(object sender, EventArgs e)
        {
            if (currentPage1 > 0)
            {
                currentPage1--;
                UpdatePag(); // Обновляем пагинацию
                FillCount();
            }
        }
        /// <summary>
        /// Кнопка перехода на следующую страницу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPag2_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRows1 / rowsPerPage1);
            if (currentPage1 < totalPages - 1)
            {
                currentPage1++;
                UpdatePag();
                FillCount();
            }
        }
        /// <summary>
        /// Обновляет навигацию по страницам при изменении размера формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Orders_SizeChanged(object sender, EventArgs e)
        {
            UpdatePag();
        }
        /// <summary>
        /// Просмотр состава заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewOrder_Click(object sender, EventArgs e)
        {
            int ed = dataGridView1.CurrentCell.RowIndex;
            int id = Convert.ToInt32(dataGridView1.Rows[ed].Cells["OrderID"].Value);
            ViewOrder vo = new ViewOrder(id);
            vo.ShowDialog();
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
        }
        /// <summary>
        /// Условное форматирование данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["OrderStatus"].Index && e.Value != null)
            {
                string OrderStatus = Convert.ToString(e.Value);

                if (OrderStatus == "Отменён")
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                }
                else if (OrderStatus == "Завершен")
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }
        /// <summary>
        /// Масштабирование формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleFullscreenButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // Восстанавливаем предыдущее состояние
                this.WindowState = _previousWindowState;
            }
            else
            {
                // Переходим в полноэкранный режим
                _previousWindowState = this.WindowState;  // Сохраняем текущее состояние
                _previousBorderStyle = this.FormBorderStyle; // Сохраняем стиль границы
                this.WindowState = FormWindowState.Maximized;
            }
        }
        /// <summary>
        /// Событие для выделение всей строки при нажатие ПКМ для просмотра состава заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[hit.RowIndex].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[hit.RowIndex].Cells[0];
                    contextMenuStrip1.Show(dataGridView1, e.Location);
                }
            }
        }
    }
}