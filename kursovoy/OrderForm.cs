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
namespace kursovoy
{
    public partial class OrderForm : Form
    {
        private Dictionary<string, int> order;
        public Dictionary<string, int> UpdatedOrder { get; private set; }
        public OrderForm(Dictionary<string, int> order)
        {
            InitializeComponent();
            this.order = new Dictionary<string, int>(order);
            this.UpdatedOrder = order;
            Products.Value.clearOrder = false;
            InitializeOrderFormUI(); // Инициализация всех UI-элементов
            PopulateOrderDetails(); // Отображение данных заказа
            UpdateConfirmButtonState(); // Проверка состояния заказа
        }
        private void InitializeOrderFormUI()
        {
            // Заполнение таблицы оформления заказа
            dataGridViewOrder.Columns.Add("Name", "Товар");
            dataGridViewOrder.Columns.Add("ProductQuantityInStock", "Количество");
            dataGridViewOrder.Columns.Add("Cost", "Цена на ед.");
            dataGridViewOrder.Columns.Add("Total", "Итоговая стоимость");

            // Задание параметров для столбцов и строк таблицы
            dataGridViewOrder.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewOrder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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

            // Кнопка уменьшения количества
            DataGridViewButtonColumn buttonDecrease = new DataGridViewButtonColumn();
            buttonDecrease.Name = "DecreaseButton";
            buttonDecrease.HeaderText = "-";
            buttonDecrease.Text = "-";
            buttonDecrease.UseColumnTextForButtonValue = true;
            dataGridViewOrder.Columns.Add(buttonDecrease);
        }

        /// <summary>
        /// Метод для заполнения DataGridView данными о заказе и расчета общей стоимости заказа.
        /// </summary>
        private void PopulateOrderDetails()
        {
            // Получение ссылки на DataGridView и очистка его
            DataGridView dataGridOrder = Controls["dataGridViewOrder"] as DataGridView;
            if (dataGridOrder == null) return;

            dataGridOrder.Rows.Clear();
            decimal totalOrderPrice = 0;

            // Заполнение таблицы данными о заказе
            foreach (var product in order)
            {
                string productART = GetProductNameFromDB(product.Key);
                decimal productCost = GetProductCostFromDB(product.Key);
                decimal finalPrice = (productCost) * product.Value; // Расчет стоимости заказа
                totalOrderPrice += finalPrice;
                dataGridOrder.Rows.Add(productART, product.Value, productCost, finalPrice);
            }

            label1.Text = $"Итоговая сумма: {totalOrderPrice:F2} руб.";
            label3.Visible = false;
            label4.Visible = false;

            if (totalOrderPrice >= 2000)
            {
                label4.Visible = true;
                label4.Text = $"Cумма без скидки: {totalOrderPrice}";
                label3.Visible = true;
                decimal discount = 0.15m; // 15% скидка
                label3.Text = $"Скидка 15% от 2000р: {(totalOrderPrice * discount):F2} руб."; 

                decimal discountedPrice = totalOrderPrice * (1 - discount);
                label1.Text = $"Итоговая сумма: {discountedPrice:F2} руб."; // Выводим новую цену в label2
            }
            UpdateConfirmButtonState(); // Обновляем состояние кнопки "Оформить заказ"
        }
        private void UpdateConfirmButtonState()
        {
            AddOrder.Enabled = order.Count > 0; // Кнопка активна при наличии в заказе товаров.
        }

        /// <summary>
        /// Генерация чека заказа
        /// </summary>
        /// <param name="orderId"></param>
        public void GenerateOrderDocument(int orderId)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var doc = wordApp.Documents.Add();

            doc.Content.Text += $"====================================================================================";
            doc.Content.Text += $"                                                                                 ЧЕК";
            doc.Content.Text += $"====================================================================================";
            doc.Content.Text += $"Дата заказа: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            doc.Content.Text += $"Номер заказа: {orderId}";
            doc.Content.Text += $"====================================================================================";
            doc.Content.Text += $"Состав заказа:";

            decimal total = 0;
            decimal discountPercentage = 0.15m; // 15% скидка

            foreach (var product in UpdatedOrder)
            {
                string productName = GetProductNameFromDB(product.Key);
                decimal productCost = GetProductCostFromDB(product.Key);
                decimal subtotal = product.Value * productCost;

                doc.Content.Text += $"{productName}                                  {product.Value} шт. {subtotal:F2} руб.";
                total += subtotal;
            }

            if (total >= 2000m) // Проверка суммы заказа
            {
                decimal discountAmount = total * discountPercentage;
                decimal discountedTotal = total * (1 - discountPercentage);
                doc.Content.Text += $"====================================================================================";
                doc.Content.Text += $"Скидка 15%: -{discountAmount:F2} руб.";
                doc.Content.Text += $"Cумма без скидки: {total} руб.";
                doc.Content.Text += $"Итоговая сумма со скидкой: {discountedTotal:F2} руб.";
            }
            else
            {
                doc.Content.Text += $"Итоговая сумма: {total} руб.";
            }
            doc.Content.Text += $"====================================================================================";
           
            doc.SaveAs($"OrderTicket_{orderId}.docx");
            wordApp.Visible = true;
            // doc.Close();
            // wordApp.Quit();
        }

        private void AddOrder_Click(object sender, EventArgs e)
        {
            if (order.Count == 0)
            {
                MessageBox.Show("Ваш заказ пуст!");
                return;
            }
            try
            {
                int orderId = SaveOrderToDB(); // Сохранение заказа в базе данных и получение его ID
                if (orderId > 0)
                {
                    GenerateOrderDocument(orderId); // Генерирация чека
                                                    
                    MessageBox.Show("Заказ успешно оформлен! Чек сохранён как OrderTicket.docx.","Оформление заказа!",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    Products.Value.clearOrder = true;
                    order.Clear();
                    PopulateOrderDetails();
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
            int orderId; // ID созданного заказа
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction(); // Начало транзакции
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

                    // Добавление новой записи в таблицу order
                    string insertOrderQuery = @"INSERT INTO `order` (OrderDate, OrderStatus, OrderUser, OrderPrice)
                    VALUES (@OrderDate, @OrderStatus, @OrderUser, @OrderPrice);
                    SELECT LAST_INSERT_ID();";

                    MySqlCommand cmd = new MySqlCommand(insertOrderQuery, connection, transaction);
                    cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@OrderStatus", "Завершен");
                    cmd.Parameters.AddWithValue("@OrderUser", Authorization.User2.EmployeeID);

                    if (total >= 2000m)//Проверка на сумму заказа
                    {
                        decimal discountAmount = total * discountPercentage;
                        decimal discountedTotal = total * (1 - discountPercentage);
                        cmd.Parameters.AddWithValue("OrderPrice", discountedTotal);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("OrderPrice", total);
                    }
                    // Получение ID нового заказа
                    orderId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Добавление новой записи в таблицу ProductOrder
                    string insertProductQuery = @"INSERT INTO `ProductOrder` (OrderID, ProductID, ProductCount)
                    VALUES (@OrderID, @ProductID, @ProductCount);";
                    foreach (var product in order)
                    {
                        MySqlCommand productCmd = new MySqlCommand(insertProductQuery, connection, transaction);

                        // Параметры для добавления товаров в заказ
                        productCmd.Parameters.AddWithValue("@OrderID", orderId); // ID созданного заказа
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
                    transaction.Commit(); // Конец транзакции
                }
                
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Ошибка при сохранении заказа: {ex.Message}");
                }
                connection.Close();
                return orderId; 
            }
        }

        /// <summary>
        /// Метод обрабатывает изменение количества товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridOrder = sender as DataGridView;

            // При изменении количества пересчет значения в таблице
            if (e.ColumnIndex == dataGridOrder.Columns["ProductQuantityInStock"].Index && e.RowIndex >= 0)
            {
                string productName = dataGridOrder.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                string productId = GetProductIdByName(productName);
                if (!int.TryParse(dataGridViewOrder.Rows[e.RowIndex].Cells["ProductQuantityInStock"].Value?.ToString(), out int newQuantity))
                {
                    MessageBox.Show("Введите целое число!");
                    PopulateOrderDetails(); // Перезаполнение таблицы
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
                        MessageBox.Show($"Количество товара не может превышать {maxQuantity}!");
                        dataGridOrder.Rows[e.RowIndex].Cells["ProductQuantityInStock"].Value = order[productId];
                    }
                }
                PopulateOrderDetails(); // Перезаполнение таблицы
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
                // Подтверждение удаления
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (order.ContainsKey(productId))
                    {
                        order.Remove(productId);
                        UpdatedOrder.Remove(productId);
                    }
                    PopulateOrderDetails(); // Перезаполнение таблицы
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
                        MessageBox.Show("На складе недостаточно товара.");
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
                        //Если количество товара 1, то при нажатии на "-" товар удаляется
                        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (order.ContainsKey(productId))
                            {
                                order.Remove(productId);
                                UpdatedOrder.Remove(productId);
                            }
                        }
                    }
                    PopulateOrderDetails(); // Перезаполнение таблицы
                }
            }
        }

        /// <summary>
        /// Очистка корзины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Products.Value.clearOrder = true;
            order.Clear();
            PopulateOrderDetails();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrder.Rows.Count == 0 || (dataGridViewOrder.Rows.Count == 1 && dataGridViewOrder.Rows[0].IsNewRow))
            {
                order.Clear();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
