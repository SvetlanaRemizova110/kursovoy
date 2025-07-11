﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace kursovoy
{
    public partial class ViewOrder : Form
    {
        int OrderID;
        private bool isOrderCancelled = false;
        private Color defaultLabel4BackColor;

        private Dictionary<string, int> initialQuantities = new Dictionary<string, int>();
        private Dictionary<string, int> maxAvailableQuantities = new Dictionary<string, int>();

        private decimal discountRate = 0.15m; 
        private decimal discountThreshold = 2000m; 
        private DateTime orderDate; 

        public ViewOrder(int id)
        {
            InitializeComponent();
            this.OrderID = id;
            defaultLabel4BackColor = label4.BackColor;
        }
        private void ViewOrder_Load(object sender, EventArgs e)
        {
            FillDataGrid();
            FillDataGriOrder();
            label7.Text = "Количество записей: ";
            label7.Text += dataGridView1.Rows.Count;
        }
        void FillDataGrid()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Program1.ConnectionString);
                con.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT productorder.ProductID AS 'Артикул товара', product.Name AS 'Название товара'," +
                    "product.Cost AS 'Стоимость(одной)', product.Unit AS 'Единица измерения', productorder.ProductCount AS 'Количество'," +
                    "productorder.OrderID 'ID', product.ProductPhoto AS 'Фото'" +
                    " FROM productorder" +
                    " INNER JOIN product ON product.ProductArticul = productorder.ProductID" +
                    $" WHERE OrderID = {OrderID}; ", con);

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRows();
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.RowTemplate.Height = 80;
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;

                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                imageColumn.Name = "ProductPhoto";
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imageColumn.HeaderText = "Фото";
                dataGridView1.Columns.Add(imageColumn);
                dataGridView1.Columns["ProductPhoto"].Visible = true;

                dataGridView1.Columns.Add("ProductID", "Артикул товара");
                dataGridView1.Columns.Add("product.Name", "Название товара");
                dataGridView1.Columns.Add("product.Cost", "Стоимость");
                dataGridView1.Columns.Add("DiscountedCost", "Стоимость со скидкой"); // Новая колонка
                dataGridView1.Columns.Add("product.Unit", "Единица измерения");
                dataGridView1.Columns.Add("ProductCount", "Количество");
                dataGridView1.Columns["ProductCount"].Width = 88;
                dataGridView1.Columns.Add("OrderID", "Заказ");
                dataGridView1.Columns["OrderID"].Visible = false;
                dataGridView1.Columns["product.Name"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                isOrderCancelled = false;

                if (!isOrderCancelled)
                {
                    DataGridViewButtonColumn plusButtonColumn = new DataGridViewButtonColumn();
                    plusButtonColumn.Name = "Добавить";
                    plusButtonColumn.Text = "+";
                    plusButtonColumn.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(plusButtonColumn);
                    dataGridView1.Columns["Добавить"].Width = 68;

                    DataGridViewButtonColumn minusButtonColumn = new DataGridViewButtonColumn();
                    minusButtonColumn.Name = "Убрать";
                    minusButtonColumn.Text = "-";
                    minusButtonColumn.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(minusButtonColumn);
                    dataGridView1.Columns["Убрать"].Width = 68;

                    DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                    deleteButtonColumn.Name = "Удалить";
                    deleteButtonColumn.Text = "x";
                    deleteButtonColumn.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(deleteButtonColumn);
                    dataGridView1.Columns["Удалить"].Width = 68;
                }
                MySqlDataReader rdr = cmd.ExecuteReader();
                initialQuantities.Clear();
                maxAvailableQuantities.Clear();
                while (rdr.Read())
                {
                    string productID = rdr[0].ToString();
                    int productCount = Convert.ToInt32(rdr[4]);
                    decimal productCost = Convert.ToDecimal(rdr[2]);
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["ProductID"].Value = productID;
                    string imsName = rdr[6].ToString();
                    if (string.IsNullOrEmpty(imsName))
                    {
                        imsName = "picture.png";
                    }
                    Image img = Image.FromFile(@"./photo/" + imsName);
                    row.Cells["ProductPhoto"].Value = img;
                    row.Cells["product.Name"].Value = rdr[1].ToString();
                    row.Cells["product.Cost"].Value = productCost.ToString("F2");
                    row.Cells["product.Unit"].Value = rdr[3].ToString();
                    row.Cells["ProductCount"].Value = productCount.ToString();
                    row.Cells["OrderID"].Value = rdr[5].ToString();
                    row.Cells["DiscountedCost"].Value = productCost.ToString("F2");

                    initialQuantities[productID] = productCount;
                    maxAvailableQuantities[productID] = productCount;
                }
                con.Close();

                UpdateOrderPrice();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Заполняет информацию о заказе
        /// </summary>
        public void FillDataGriOrder()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Program1.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand command = new MySqlCommand("SELECT OrderID AS 'Номер заказа',OrderDate AS 'Дата заказа'," +
                    " OrderStatus AS 'Статус заказа', e.EmployeeF AS 'Фамилия', e.EmployeeI AS 'Имя', e.EmployeeO AS 'Отчество', OrderPrice AS 'Сумма заказа'" +
                    " FROM `order`" +
                    $" INNER JOIN `employeeee` e ON `order`.OrderUser = e.EmployeeID WHERE OrderID={OrderID}", con))
                    {
                        MySqlDataReader rdr = command.ExecuteReader();
                        while (rdr.Read())
                        {
                            label2.Text = $"Номер заказа: {rdr["Номер заказа"]}";
                            DateTime tempOrderDate = Convert.ToDateTime(rdr["Дата заказа"]);
                            label1.Text = $"Дата заказа: {tempOrderDate}";
                            orderDate = tempOrderDate;  //Сохраняем дату заказа
                            label4.Text = $"Статус заказа: {rdr["Статус заказа"]}";
                            string orderStatus = rdr["Статус заказа"].ToString();
                            isOrderCancelled = (orderStatus == "Отменён");

                            if (isOrderCancelled)
                            {
                                label4.BackColor = Color.LightCoral;
                            }
                            else
                            {
                                label4.BackColor = defaultLabel4BackColor;
                            }
                            label4.Refresh();
                            string employeeName = $"{rdr["Фамилия"]} {rdr["Имя"]} {rdr["Отчество"]}";
                            label3.Text = $"Сотрудник: {employeeName}";
                            decimal orderPrice = Convert.ToDecimal(rdr["Сумма заказа"]);
                            label5.Text = $"Сумма заказа: {orderPrice:F2} руб.";
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    if (Authorization.User2.Role == 3)
                    {
                        MessageBox.Show("Продавец не может отменять заказ!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (isOrderCancelled)
                    {
                        MessageBox.Show("Заказ отменен. Редактирование запрещено.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    string productID = dataGridView1.Rows[e.RowIndex].Cells["ProductID"].Value.ToString();
                    int currentQuantity = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ProductCount"].Value);

                    if (e.ColumnIndex == dataGridView1.Columns["Добавить"].Index)
                    {

                        if (maxAvailableQuantities.ContainsKey(productID) && currentQuantity < maxAvailableQuantities[productID])
                        {
                            ChangeProductQuantity(productID, e.RowIndex, currentQuantity + 1);
                        }
                        else
                        {
                            MessageBox.Show("Нельзя увеличить количество больше изначального.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (e.ColumnIndex == dataGridView1.Columns["Убрать"].Index)
                    {
                        int newQuantity = currentQuantity - 1;
                        if (newQuantity == 0)
                        {
                            DeleteProductFromOrder(e.RowIndex);
                            FillDataGrid();
                            FillDataGriOrder();
                        }
                        else
                        {
                            ChangeProductQuantity(productID, e.RowIndex, newQuantity);
                        }
                    }
                    else if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index)
                    {
                        DeleteProductFromOrder(e.RowIndex);
                        FillDataGrid();
                        FillDataGriOrder();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Удаляет товар из заказа
        /// </summary>
        /// <param name="rowIndex"></param>
        private void DeleteProductFromOrder(int rowIndex)
        {
            if ((DateTime.Now - orderDate).TotalDays > 14)
            {
                MessageBox.Show("Нельзя отменить заказ, так как прошло больше 14 дней с даты заказа.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isOrderCancelled)
            {
                MessageBox.Show("Заказ отменен. Редактирование запрещено.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Проверяем, если ли в заказе остался только один товар в количестве 1
            if (dataGridView1.Rows.Count == 1)
            {
                CancelOrder(rowIndex); // Отменяем заказ и возвращаем товар
                return; 
            }
            if (MessageBox.Show("Вы уверены, что хотите удалить этот товар из заказа?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    MySqlConnection con = new MySqlConnection(Program1.ConnectionString);
                    con.Open();
                    string productID = dataGridView1.Rows[rowIndex].Cells["ProductID"].Value.ToString();
                    string orderID = OrderID.ToString();
                    int quantityToRemove = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["ProductCount"].Value);
                    string query = "DELETE FROM productorder WHERE ProductID = @ProductID AND OrderID = @OrderID";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@OrderID", orderID);
                    cmd.ExecuteNonQuery();
                    string updateStockQuery = "UPDATE product SET ProductQuantityInStock = ProductQuantityInStock + @Quantity WHERE ProductArticul = @ProductID";
                    MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, con);
                    updateStockCmd.Parameters.AddWithValue("@Quantity", quantityToRemove);
                    updateStockCmd.Parameters.AddWithValue("@ProductID", productID);
                    updateStockCmd.ExecuteNonQuery();
                    con.Close();
                    dataGridView1.Rows.RemoveAt(rowIndex);
                    UpdateOrderPrice(); // Пересчитываем сумму заказа
                    initialQuantities.Remove(productID);
                    maxAvailableQuantities.Remove(productID);
                    MessageBox.Show("Товар успешно удален из заказа.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Изменяет количество товара в заказе
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="rowIndex"></param>
        /// <param name="newQuantity"></param>
        private void ChangeProductQuantity(string productID, int rowIndex, int newQuantity)
        {
            // Проверяем, прошло ли более 14 дней с даты заказа
            if ((DateTime.Now - orderDate).TotalDays > 14)
            {
                MessageBox.Show("Нельзя отменить заказ, так как прошло больше 14 дней с даты заказа.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isOrderCancelled)
            {
                MessageBox.Show("Заказ отменен. Редактирование запрещено.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Проверяем, если при уменьшении кол-ва останется только один товар в количестве 1
            if (newQuantity == 0 && dataGridView1.Rows.Count == 1)
            {
                CancelOrder(rowIndex); // Отменяем заказ и возвращаем товар
                return;
            }
            try
            {
                if (MessageBox.Show("Отменить одну единицу товара в заказе?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    MySqlConnection con = new MySqlConnection(Program1.ConnectionString);
                    con.Open();
                    int oldQuantity = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["ProductCount"].Value);
                    string query = "UPDATE productorder SET ProductCount = @NewQuantity WHERE ProductID = @ProductID AND OrderID = @OrderID";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NewQuantity", newQuantity);
                    cmd.Parameters.AddWithValue("@ProductID", productID);
                    cmd.Parameters.AddWithValue("@OrderID", OrderID);
                    cmd.ExecuteNonQuery();

                    string updateStockQuery = "UPDATE product SET ProductQuantityInStock = ProductQuantityInStock - (@NewQuantity - @OldQuantity) WHERE ProductArticul = @ProductID";
                    MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, con);
                    updateStockCmd.Parameters.AddWithValue("@NewQuantity", newQuantity);
                    updateStockCmd.Parameters.AddWithValue("@OldQuantity", oldQuantity);
                    updateStockCmd.Parameters.AddWithValue("@ProductID", productID);
                    updateStockCmd.ExecuteNonQuery();
                    con.Close();

                    dataGridView1.Rows[rowIndex].Cells["ProductCount"].Value = newQuantity;
                    UpdateOrderPrice(); // Пересчитываем сумму заказа
                    maxAvailableQuantities[productID] -= (newQuantity - oldQuantity);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении количества товара: {ex.Message}");
            }
        }
        /// <summary>
        /// Метод для обновления общей стоимости заказа с учетом возможной скидки
        /// </summary>
        private void UpdateOrderPrice()
        {
            try
            {
                decimal totalOrderPriceWithoutDiscount = 0;
                decimal totalOrderPriceWithDiscount = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    decimal productCost = Convert.ToDecimal(row.Cells["product.Cost"].Value); // Цена БЕЗ скидки
                    int quantity = Convert.ToInt32(row.Cells["ProductCount"].Value);
                    totalOrderPriceWithoutDiscount += productCost * quantity;
                }
                // Проверяем, должна ли применяться скидка
                bool applyDiscount = totalOrderPriceWithoutDiscount >= discountThreshold;
                if (applyDiscount)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        decimal productCost = Convert.ToDecimal(row.Cells["product.Cost"].Value); // Цена БЕЗ скидки
                        decimal discountedCost = productCost * (1 - discountRate); // Применяем скидку
                        int quantity = Convert.ToInt32(row.Cells["ProductCount"].Value);
                        totalOrderPriceWithDiscount += discountedCost * quantity;
                        row.Cells["DiscountedCost"].Value = discountedCost.ToString("F2"); // Обновляем отображаемую цену со скидкой
                    }
                }
                else
                {
                    totalOrderPriceWithDiscount = totalOrderPriceWithoutDiscount;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        row.Cells["DiscountedCost"].Value = row.Cells["product.Cost"].Value; // Показываем цену без скидки
                    }
                }
                label5.Text = $"Сумма заказа: {totalOrderPriceWithDiscount:F2} руб.";
                UpdateOrderPriceInDatabase(totalOrderPriceWithDiscount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Метод для обновления суммы заказа в базе данных
        /// </summary>
        /// <param name="newPrice"></param>
        private void UpdateOrderPriceInDatabase(decimal newPrice)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Program1.ConnectionString))
                {
                    con.Open();
                    string query = "UPDATE `order` SET OrderPrice = @NewPrice WHERE OrderID = @OrderID";
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@NewPrice", newPrice);
                        cmd.Parameters.AddWithValue("@OrderID", OrderID);
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении суммы заказа в базе данных: {ex.Message}");
            }
        }
        /// <summary>
        /// Метод для отмены заказа
        /// </summary>
        /// <param name="rowIndex"></param>
        private void CancelOrder(int rowIndex)
        {
            try
            {
                if (MessageBox.Show("Отменить заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    using (MySqlConnection con = new MySqlConnection(Program1.ConnectionString))
                    {
                        con.Open();
                        // Возвращаем товар на склад
                        string productID = dataGridView1.Rows[rowIndex].Cells["ProductID"].Value.ToString();
                        int quantityToRemove = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["ProductCount"].Value);
                        string updateStockQuery = "UPDATE product SET ProductQuantityInStock = ProductQuantityInStock + @Quantity WHERE ProductArticul = @ProductID";
                        MySqlCommand updateStockCmd = new MySqlCommand(updateStockQuery, con);
                        updateStockCmd.Parameters.AddWithValue("@Quantity", quantityToRemove);
                        updateStockCmd.Parameters.AddWithValue("@ProductID", productID);
                        updateStockCmd.ExecuteNonQuery();

                        // Обновляем статус заказа
                        string query = "UPDATE `order` SET OrderStatus = 'Отменён' WHERE OrderID = @OrderID";
                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", OrderID);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                        isOrderCancelled = true;
                        label4.Text = "Статус заказа: Отменён";
                        label4.BackColor = Color.LightCoral;
                        MessageBox.Show("Заказ успешно отменен. Товар возвращен на склад.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Обновляем отображение заказа
                        FillDataGriOrder();
                        FillDataGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене заказа: {ex.Message}");
            }
        }
    }
}
