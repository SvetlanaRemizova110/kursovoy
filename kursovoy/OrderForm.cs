using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace kursovoy
{
    public partial class OrderForm : Form
    {
        private Dictionary<string, int> order;
        public Dictionary<string, int> UpdatedOrder { get; private set; }
        private int orderId;

        public OrderForm(Dictionary<string, int> order)
        {
            InitializeComponent();
            this.order = new Dictionary<string, int>(order);
            this.UpdatedOrder = order;
            Products.Value.clearOrder = false;
            InitializeOrderFormUI();
            PopulateOrderDetails();
            UpdateConfirmButtonState(); 

            this.orderId = -1;
            UpdateOrderLabels();
        }
        /// <summary>
        /// Обновляет информацию о заказе (дату и номер).
        /// </summary>
        private void UpdateOrderLabels()
        {
            orderDateLabel.Text = $"Дата: {DateTime.Now.ToShortDateString()}";
            if (orderId == -1)
            {
                orderNumberLabel.Text = $"Номер заказа: {GetNextOrderId()}";
            }
            else
            {
                orderNumberLabel.Text = $"Номер заказа: {orderId}";
            }
        }
        /// <summary>
        /// Вывод товаров, добавленных в корзину
        /// </summary>
        private void InitializeOrderFormUI()
        {
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Name = "CartProductPhoto";
            imageColumn.HeaderText = "Фото";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            dataGridViewOrder.Columns.Add(imageColumn); 

            dataGridViewOrder.Columns.Add("Name", "Товар");
            dataGridViewOrder.Columns["Name"].Width = 150;
            dataGridViewOrder.Columns["Name"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridViewOrder.Columns.Add("ProductQuantityInStock", "Количество");
            dataGridViewOrder.Columns.Add("Cost", "Цена на ед.");
            dataGridViewOrder.Columns.Add("Total", "Итоговая стоимость");

            dataGridViewOrder.AutoResizeColumns();
            dataGridViewOrder.AutoResizeRows();
            dataGridViewOrder.AllowUserToDeleteRows = false;
            dataGridViewOrder.AllowUserToOrderColumns = false;
            dataGridViewOrder.AllowUserToResizeColumns = false;
            dataGridViewOrder.AllowUserToResizeRows = false;
            dataGridViewOrder.RowTemplate.Height = 80;
            dataGridViewOrder.ReadOnly = true;
            dataGridViewOrder.AllowUserToAddRows = false;

            // Кнопка удаления товара из корзины
            DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
            buttonDel.Name = "Удалить";
            buttonDel.HeaderText = "Удалить";
            buttonDel.Text = "Удалить";
            buttonDel.UseColumnTextForButtonValue = true;
            dataGridViewOrder.Columns.Add(buttonDel);

            // Кнопка увеличения количества
            DataGridViewButtonColumn buttonIncrease = new DataGridViewButtonColumn();
            buttonIncrease.Name = "IncreaseButton";
            buttonIncrease.HeaderText = "+";
            buttonIncrease.Text = "+";
            buttonIncrease.UseColumnTextForButtonValue = true;
            dataGridViewOrder.Columns.Add(buttonIncrease);
            dataGridViewOrder.Columns["IncreaseButton"].Width = 50;

            // Кнопка уменьшения количества
            DataGridViewButtonColumn buttonDecrease = new DataGridViewButtonColumn();
            buttonDecrease.Name = "DecreaseButton";
            buttonDecrease.HeaderText = "-";
            buttonDecrease.Text = "-";
            buttonDecrease.UseColumnTextForButtonValue = true;
            dataGridViewOrder.Columns.Add(buttonDecrease);
            dataGridViewOrder.Columns["DecreaseButton"].Width = 50;
        }

        /// <summary>
        /// Метод для заполнения DataGridView данными о заказе и расчета общей стоимости заказа.
        /// </summary>
        private void PopulateOrderDetails()
        {
            dataGridViewOrder.Rows.Clear();
            decimal totalOrderPrice = 0;
            foreach (var product in order)
            {
                string productART = product.Key;
                string productName = GetProductNameFromDB(productART);
                decimal productCost = GetProductCostFromDB(productART);
                decimal finalPrice = productCost * product.Value;

                // Получаем имя файла изображения из базы
                string imageFileName = GetProductImageStringFromDB(productART);
                if (string.IsNullOrEmpty(imageFileName))
                {
                    imageFileName = "picture.png"; // фото-заглушка
                }

                // Формируем путь к изображению
                string imagePath = Path.Combine(System.Windows.Forms.Application.StartupPath, "photo", imageFileName);
                Image productImage = null;

                if (File.Exists(imagePath))
                {
                    productImage = Image.FromFile(imagePath);
                }
                else
                {
                    MessageBox.Show($"Файл изображения не найден: {imagePath}");
                }
                dataGridViewOrder.Rows.Add(productImage, productName, product.Value, productCost, finalPrice);
                totalOrderPrice += finalPrice;
            }
            label1.Text = $"Итоговая сумма: {totalOrderPrice:F2} руб.";
            label3.Visible = false;
            label4.Visible = false;

            if (totalOrderPrice >= 2000)
            {
                label4.Visible = true;
                label4.Text = $"Cумма без скидки: {totalOrderPrice}";
                label3.Visible = true;
                decimal discount = 0.15m;
                label3.Text = $"Скидка 15% от 2000р: {(totalOrderPrice * discount):F2} руб.";
                decimal discountedPrice = totalOrderPrice * (1 - discount);
                label1.Text = $"Итоговая сумма: {discountedPrice:F2} руб.";
            }
            UpdateConfirmButtonState();
        }
        /// <summary>
        /// Проверка состояния заказа
        /// </summary>
        private void UpdateConfirmButtonState()
        {
            AddOrder.Enabled = order.Count > 0; // Кнопка активна при наличии в заказе товаров.
        }
        /// <summary>
        /// Получение информации о компании из базы данных.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetCompanyInfo()
        {
            Dictionary<string, string> companyInfo = new Dictionary<string, string>();

            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT company_name, company_adress, company_phone, company_inn, company_ogrn, company_bick, company_ip FROM companyinfo LIMIT 1";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            companyInfo["company_name"] = reader["company_name"].ToString();
                            companyInfo["company_adress"] = reader["company_adress"].ToString();
                            companyInfo["company_phone"] = reader["company_phone"].ToString();
                            companyInfo["company_inn"] = reader["company_inn"].ToString();
                            companyInfo["company_ogrn"] = reader["company_ogrn"].ToString();
                            companyInfo["company_bick"] = reader["company_bick"].ToString();
                            companyInfo["company_ip"] = reader["company_ip"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении информации о компании: {ex.Message}");
                }
            }
            return companyInfo;
        }
        /// <summary>
        /// Функция для получения изображения из базы данных
        /// </summary>
        /// <param name="productART"></param>
        /// <returns></returns>
        private string GetProductImageStringFromDB(string productART)
        {
            string imageString = null;
            string connectionString = Authorization.Program.ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProductPhoto FROM product WHERE ProductArticul = @ProductArticul";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductArticul", productART);
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            imageString = result.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении изображения продукта: " + ex.Message);
                }
            }
            return imageString;
        }
        /// <summary>
        /// Генерация документа заказа в формате Word.
        /// </summary>
        /// <param name="orderId">ID заказа.</param>
        private void GenerateOrderDocument(int orderId)
        {
            try
            {
                var companyInfo = GetCompanyInfo();
                var wordApp = new Microsoft.Office.Interop.Word.Application();
                var doc = wordApp.Documents.Add();

                // Настройка ориентации страницы на альбомную
                doc.PageSetup.Orientation = WdOrientation.wdOrientLandscape;

                // Установка полей
                doc.PageSetup.LeftMargin = wordApp.CentimetersToPoints(1);
                doc.PageSetup.RightMargin = wordApp.CentimetersToPoints(1);
                doc.PageSetup.TopMargin = wordApp.CentimetersToPoints(1);
                doc.PageSetup.BottomMargin = wordApp.CentimetersToPoints(1);

                // Установка междустрочного интервала 1.5
                var paragraphFormat = doc.Content.ParagraphFormat;
                paragraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpace1pt5;
                paragraphFormat.SpaceBefore = 0;
                paragraphFormat.SpaceAfter = 0;

                // Стиль для заголовков таблицы
                object styleHeading = WdBuiltinStyle.wdStyleHeading1;

                Paragraph paragraph = doc.Content.Paragraphs.Add();
                Range range = paragraph.Range;

                // Настройка шрифта для заголовка
                range.Font.Size = 20;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                range.Text = "ТОВАРНЫЙ ЧЕК";

                // Сброс форматирования для остальной информации о компании
                range = doc.Content.Paragraphs.Add().Range;
                range.Font.Size = 14;
                range.Font.Bold = 0;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                range.Text += $"===============================================================================================================\n";

                range.Font.Size = 14;
                range.Font.Bold = 1;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                range.Text += $"{companyInfo["company_name"]}\n";

                range.Font.Size = 12;
                range.Font.Bold = 0;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                range.Text += $"Адрес: {companyInfo["company_adress"]}\n";
                range.Text += $"Телефон: {companyInfo["company_phone"]}\n";
                range.Text += $"ИНН: {companyInfo["company_inn"]}  ОГРН: {companyInfo["company_ogrn"]}\n";
                range.Text += $"БИК: {companyInfo["company_bick"]}  ИП: {companyInfo["company_ip"]}\n";
                range.Text += $"===============================================================================================================\n";
                range.Text += $"Дата заказа: {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}\n";
                range.Text += $"Номер заказа: {orderId}\n";
                range.Text += $"===============================================================================================================\n";

                Range tableRange = doc.Content.Paragraphs.Add().Range;
                // Создание таблицы
                Table orderTable = doc.Tables.Add(tableRange, 1, 5);
                orderTable.Borders.Enable = 1;
                orderTable.AllowAutoFit = true;
                orderTable.PreferredWidthType = WdPreferredWidthType.wdPreferredWidthPercent;
                orderTable.PreferredWidth = 100;

                // Заголовки таблицы
                orderTable.Cell(1, 1).Range.Text = "Товар";
                orderTable.Cell(1, 2).Range.Text = "Количество";
                orderTable.Cell(1, 3).Range.Text = "Цена";
                orderTable.Cell(1, 4).Range.Text = "Скидка (%)";
                orderTable.Cell(1, 5).Range.Text = "Итого";

                // Применяем стиль к заголовкам
                for (int i = 1; i <= 5; i++)
                {
                    orderTable.Cell(1, i).Range.Bold = 1;
                    orderTable.Cell(1, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                }
                decimal totalOrderAmount = 0;
                foreach (var product in UpdatedOrder)
                {
                    decimal productCost = GetProductCostFromDB(product.Key);
                    int quantity = product.Value;
                    totalOrderAmount += productCost * quantity; // Считаем полную сумму без скидки
                }

                // Определяем процент скидки, исходя из всей суммы заказа
                decimal discountPercentage = (totalOrderAmount >= 2000) ? 15 : 0;

                totalOrderAmount = 0;
                int row = 2;
                foreach (var product in UpdatedOrder)
                {
                    string productName = GetProductNameFromDB(product.Key);
                    decimal productCost = GetProductCostFromDB(product.Key);
                    int quantity = product.Value;
                    decimal discountedPrice = productCost * (1 - discountPercentage / 100);
                    decimal subtotal = discountedPrice * quantity;
                    totalOrderAmount += subtotal;
                    row++;
                    orderTable.Rows.Add();

                    // Заполнение ячеек
                    orderTable.Cell(row, 1).Range.Text = productName;
                    orderTable.Cell(row, 2).Range.Text = quantity.ToString();
                    orderTable.Cell(row, 3).Range.Text = productCost.ToString("F2");
                    orderTable.Cell(row, 4).Range.Text = discountPercentage.ToString("F2");
                    orderTable.Cell(row, 5).Range.Text = subtotal.ToString("F2");
                }
                // Добавление строки с итоговой суммой
                orderTable.Rows.Add();
                orderTable.Cell(row, 1).Range.Text = "Итоговая сумма:";
                orderTable.Cell(row, 5).Range.Text = totalOrderAmount.ToString("F2") + " руб.";
                orderTable.Cell(row, 5).Range.Bold = 1;

                range = doc.Content.Paragraphs.Add().Range;
                range.Font.Size = 14;
                range.Font.Bold = 0;
                range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                range.Text += $"===============================================================================================================\n";
                range.Text += $"Спасибо за покупку!\n";
                range.Text += $"Продавец: {Authorization.User2.FIO} \n";
                range.Text += $"===============================================================================================================";

                // Определение пути для сохранения
                string checksFolder = Path.Combine(Environment.CurrentDirectory, "checks");
                if (!Directory.Exists(checksFolder))
                {
                    Directory.CreateDirectory(checksFolder); // Если папка не существует, создаем её
                }

                string fileName = $"OrderTicket_{orderId}.docx";
                string filePath = Path.Combine(checksFolder, fileName); // Полный путь к файлу

                // Сохранение документа
                doc.SaveAs(filePath);
                doc.Close();
                wordApp.Quit();

                // Открытие документа после сохранения
                try
                {
                    Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть документ: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть документ: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Кнопка для оформления заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddOrder_Click(object sender, EventArgs e)
        {
            if (order.Count == 0)
            {
                MessageBox.Show("Ваш заказ пуст!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                // Сохранение заказа в базе данных и получение его ID
                orderId = SaveOrderToDB();
                if (orderId > 0)
                {
                    MessageBox.Show("Заказ успешно оформлен!", "Оформление заказа", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button3.Enabled = true;
                    AddOrder.Enabled = false;
                    button2.Enabled = false;
                    button1.Enabled = false;

                    UpdateOrderLabels();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранение заказа в базе данных
        /// </summary>
        /// <returns></returns>
        private int SaveOrderToDB()
        {
            int newOrderId; 
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    // Расчет итоговой суммы заказа
                    decimal totalAmount = 0;
                    decimal total = 0;
                    decimal discountPercentage = 0.15m;

                    foreach (var product in UpdatedOrder)
                    {
                        string productName = GetProductNameFromDB(product.Key);
                        decimal productCost = GetProductCostFromDB(product.Key);
                        decimal subtotal = product.Value * productCost;
                        total += subtotal;
                    }

                    foreach (var product in order)
                    {
                        decimal unitPrice = GetProductCostFromDB(product.Key);
                        totalAmount += product.Value;
                    }

                    string insertOrderQuery = @"INSERT INTO `order` (OrderDate, OrderStatus, OrderUser, OrderPrice)
                        VALUES (@OrderDate, @OrderStatus, @OrderUser, @OrderPrice);
                        SELECT LAST_INSERT_ID();";

                    MySqlCommand cmd = new MySqlCommand(insertOrderQuery, connection, transaction);
                    cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@OrderStatus", "Завершен");
                    cmd.Parameters.AddWithValue("@OrderUser", Authorization.User2.EmployeeID);


                    if (total >= 2000m) //Проверка на сумму заказа
                    {
                        decimal discountAmount = total * discountPercentage;
                        decimal discountedTotal = total * (1 - discountPercentage);
                        cmd.Parameters.AddWithValue("@OrderPrice", discountedTotal);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@OrderPrice", total);
                    }
                    
                    newOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                    string insertProductQuery = @"INSERT INTO `ProductOrder` (OrderID, ProductID, ProductCount)
                        VALUES (@OrderID, @ProductID, @ProductCount);";
                    foreach (var product in order)
                    {
                        MySqlCommand productCmd = new MySqlCommand(insertProductQuery, connection, transaction);

                        // Параметры для добавления товаров в заказ
                        productCmd.Parameters.AddWithValue("@OrderID", newOrderId); // ID созданного заказа
                        productCmd.Parameters.AddWithValue("@ProductID", product.Key); // Артикул товара
                        productCmd.Parameters.AddWithValue("@ProductCount", product.Value); // Количество товара
                        productCmd.ExecuteNonQuery();

                        //Уменьшение кол-ва товаров на складе
                        string updateStockQuery = "UPDATE product SET ProductQuantityInStock = ProductQuantityInStock - @ProductCount WHERE ProductArticul = @ProductID";
                        MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, connection, transaction);
                        updateStockCmd.Parameters.AddWithValue("@ProductCount", product.Value);
                        updateStockCmd.Parameters.AddWithValue("@ProductID", product.Key);
                        updateStockCmd.ExecuteNonQuery();
                    }
                    transaction.Commit(); 
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка при получении информации о компании: {ex.Message}");
                    return -1;
                }
                connection.Close();
                return newOrderId;
            }
        }
        /// <summary>
        /// Метод обрабатывает изменение количества товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dataGridOrder = sender as DataGridView;

                // При изменении количества пересчет значения в таблице
                if (e.ColumnIndex == dataGridOrder.Columns["ProductQuantityInStock"].Index && e.RowIndex >= 0)
                {
                    string productName = dataGridOrder.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                    string productId = GetProductIdByName(productName);
                    if (!int.TryParse(dataGridViewOrder.Rows[e.RowIndex].Cells["ProductQuantityInStock"].Value?.ToString(), out int newQuantity))
                    {
                        PopulateOrderDetails();
                        return;
                    }
                    int maxQuantity = GetProductQuantityInStock(productId);

                    // Проверка, что введенное количество не превышает количество на складе
                    if (newQuantity > 0 && newQuantity <= maxQuantity && order.ContainsKey(productId))
                    {
                        order[productId] = newQuantity;
                        UpdatedOrder[productId] = newQuantity;
                    }
                    else
                    {
                        if (newQuantity <= 0)
                        {
                            MessageBox.Show("Количество должно быть больше нуля!");
                            dataGridOrder.Rows[e.RowIndex].Cells["ProductQuantityInStock"].Value = order[productId];
                        }
                        else
                        {
                            MessageBox.Show($"Количество товара не может превышать {maxQuantity}!", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            dataGridOrder.Rows[e.RowIndex].Cells["ProductQuantityInStock"].Value = order[productId];
                        }
                    }
                    PopulateOrderDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Метод для получения артикула товара
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        private string GetProductIdByName(string productName)
        {
            string productId = "";
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                try
                {
                    connection.Open();
                string query = "SELECT ProductArticul FROM product WHERE Name = @Name";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Name", productName);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    productId = result.ToString();
                }
                connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении информации: {ex.Message}");
                }
            }
            return productId;
        }

        /// <summary>
        /// Метод для получения названия товара
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private string GetProductNameFromDB(string productId)
        {
            string productName = string.Empty;
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Name FROM product WHERE ProductArticul = @ProductArticul";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticul", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        productName = result.ToString();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении названия товара: {ex.Message}");
                }
            }
            return productName;
        }

        /// <summary>
        /// Метод для получения цены товара
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private decimal GetProductCostFromDB(string productId)
        {
            decimal productCost = 0;
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Cost FROM product WHERE ProductArticul = @ProductArticul";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticul", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && decimal.TryParse(result.ToString(), out decimal cost))
                    {
                        productCost = cost;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении стоимости товара: {ex.Message}");
                }
            }
            return productCost;
        }

        /// <summary>
        /// Метод для получения количества товара на складе
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        private int GetProductQuantityInStock(string productId)
        {
            int quantityInStock = 0;
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ProductQuantityInStock FROM product WHERE ProductArticul = @ProductArticul";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductArticul", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int quantity))
                    {
                        quantityInStock = quantity;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении количества товара на складе: {ex.Message}");
                }
            }
            return quantityInStock;
        }

        /// <summary>
        /// Для работы кнопок "Удалить", "+","-"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridOrder = sender as DataGridView;

            if (e.RowIndex < 0)
                return;

            string productName = dataGridOrder.Rows[e.RowIndex].Cells["Name"].Value.ToString();
            string productId = GetProductIdByName(productName);
            int quantityInStock = GetProductQuantityInStock(productId); // Получение количества товаров на складе

            // Кнопка "Удалить"
            if (e.ColumnIndex == dataGridOrder.Columns["Удалить"].Index)
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить этот товар из заказа?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (order.ContainsKey(productId))
                    {
                        order.Remove(productId);
                        UpdatedOrder.Remove(productId);
                    }
                    PopulateOrderDetails(); 
                }
            }
            // Кнопка "+"
            else if (e.ColumnIndex == dataGridOrder.Columns["IncreaseButton"].Index)
            {
                if (order.ContainsKey(productId))
                {
                    if (order[productId] < quantityInStock)
                    {
                        order[productId]++;
                        UpdatedOrder[productId]++;
                        PopulateOrderDetails();
                    }
                    else
                    {
                        MessageBox.Show("Невозможно добавить больше товара, чем есть на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            // Кнопка "-"
            else if (e.ColumnIndex == dataGridOrder.Columns["DecreaseButton"].Index)
            {
                if (order.ContainsKey(productId))
                {
                    if (order[productId] > 1)
                    {
                        order[productId]--;
                        UpdatedOrder[productId]--;
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить этот товар из заказа?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (order.ContainsKey(productId))
                            {
                                order.Remove(productId);
                                UpdatedOrder.Remove(productId);
                            }
                        }
                    }
                    PopulateOrderDetails(); 
                }
            }
            label5.Text = "Количество записей: ";
            label5.Text += " " + dataGridViewOrder.Rows.Count;
        }
        /// <summary>
        /// Очистка корзины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            order.Clear();
            Products.currentOrder.Clear();
            PopulateOrderDetails();
        }
        /// <summary>
        /// Кнопка для выхода в каталог
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrder.Rows.Count == 0 || (dataGridViewOrder.Rows.Count == 1 && dataGridViewOrder.Rows[0].IsNewRow))
            {
                order.Clear();
                Products.currentOrder.Clear();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        /// <summary>
        /// Cобытие при загрузки формы. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderForm_Load(object sender, EventArgs e)
        {
            label5.Text += " " + dataGridViewOrder.Rows.Count;
            label7.Text = "Продавец: " + Authorization.User2.FIO;
            button3.Enabled = false;
            AddOrder.Enabled = true;
        }
        /// <summary>
        /// Кнопка для генерации чека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (orderId > 0)
                {
                    GenerateOrderDocument(orderId); // Генерирация чека
                    MessageBox.Show("Чек успешно сгенерирован и сохранён в папке checks.", "Генерация чека", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Products.currentOrder.Clear();
                    order.Clear();
                    PopulateOrderDetails();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось получить ID последнего заказа.", "Ошибка генерации чека", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации чека: {ex.Message}", "Ошибка генерации чека", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Функция для получения последнего ID заказа
        /// </summary>
        /// <returns></returns>
        private int GetNextOrderId()
        {
            int nextOrderId = 1; // Значение по умолчанию, если нет заказов в базе
            string query = "SELECT MAX(OrderId) FROM `Order`";
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            nextOrderId = Convert.ToInt32(result) + 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при получении последнего ID заказа: {ex.Message}", "Ошибка базы данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
            }
            return nextOrderId;
        }
    }
}
