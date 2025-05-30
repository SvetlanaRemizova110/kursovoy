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
using System.Configuration;
namespace kursovoy
{
    public partial class Products : Form
    {
        private int inactivityTimeout = 0;
        // Для заказа
        private int currentPage1 = 0;
        private int rowsPerPage1 = 20;
        private int totalRows1 = 0;
        private int totalRecords;//кол-во строк всего
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();
        private Dictionary<string, int> currentOrder = new Dictionary<string, int>();
        public static class Value
        {
            public static bool clearOrder;
        }

        public Products()
        {
            InitializeComponent();
            Timer.Tick += inactivityTimer_Tick;
            Timer.Interval = 1000; // Проверка каждые 1 секунду
            GetCategoryIdByName("");
            if (Value.clearOrder == true)
            {
                currentOrder.Clear();
                Value.clearOrder = false;
            }
        }
        /// <summary>
        /// Назначение обработчиков событий клавиатуры и мыши для отслеживания активности.
        /// </summary>
        private void Users_ActivateTracking()
        {
            // Назначаем обработчики событий для всей формы
            this.MouseMove += Users_ActivityDetected;
            this.KeyPress += Users_ActivityDetected;
            this.MouseClick += Users_ActivityDetected;

            // Если есть встроенные контролы, следим за их активностью
            foreach (Control control in this.Controls)
            {
                control.MouseMove += Users_ActivityDetected;
                control.MouseClick += Users_ActivityDetected;
            }
        }
        /// <summary>
        /// Обработчик любых событий, связанных с активностью пользователя (например, движение мыши или нажатие клавиш).
        /// Отслеживает действия пользователя и сбрасывает таймер бездействия.
        /// </summary>
        private void Users_ActivityDetected(object sender, EventArgs e)
        {
            ResetInactivityTimer();
        }
        private void inactivityTimer_Tick(object sender, EventArgs e)
        {
            // Это событие сработает при превышении заданного времени бездействия
            if (inactivityTimeout > 0)
            {
                inactivityTimeout -= 1000; // Уменьшаем тайм-аут
            }
            else
            {
                Timer.Stop(); // Останавливаем таймер
                MessageBox.Show("Вы были перенаправлены на страницу авторизации из-за бездействия.", "Блокировка системы");

                Authorization authorization = new Authorization();
                this.Close();
                authorization.Show();
            }
        }

        private void Products_Load(object sender, EventArgs e)
        {
            // Загрузить интервал времени бездействия из App.config
            if (int.TryParse(ConfigurationManager.AppSettings["InactivityTimeout"], out int timeoutInSeconds))
            {
                inactivityTimeout = timeoutInSeconds * 1000; // Перевод в миллисекунды
            }
            else
            {
                // Значение по умолчанию (30 секунд), если не удалось считать App.config
                inactivityTimeout = 30000;
            }

            ResetInactivityTimer(); // Сброс таймера активности
            Timer.Start(); // Запуск таймера активности
            FillDataGrid("SELECT " +
            "ProductArticul AS 'Артикул'," +
            "Name AS 'Наименование товара'," +
            "Description AS 'Описание'," +
            "Cost AS 'Стоимость'," +
            "Unit AS 'Единица измерения'," +
            "ProductQuantityInStock AS 'Количество на складе'," +
            "ProductManufactur.ProductManufacturName AS 'Производитель'," +
            "Supplier.SupplierName AS 'Поставщик'," +
            "Category.CategoryName AS 'Категория'," +
            "ProductPhoto " +
            "FROM Product " +
            "INNER JOIN ProductManufactur ON Product.ProductManufactur = ProductManufactur.ProductManufacturID " +
            "INNER JOIN Supplier ON Product.ProductSupplier = Supplier.SupplierID " +
            "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID", 1);
            // Для запрета изменять ComboBox
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
            FillCount();
        }

        /// <summary>
        /// Метод для загрузки данных
        /// </summary>
        /// <param name="strCmd"></param>
        /// <param name="categoryId"></param>
        public void FillDataGrid(string strCmd, int categoryId)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);

                if (categoryId != -1)
                {
                    command.Parameters.AddWithValue("@categoryId", categoryId);
                }

                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
                allRows1.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                imageColumn.Name = "ProductPhoto";
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imageColumn.HeaderText = "Фото";

                dataGridView1.AutoResizeColumns();
                dataGridView1.AutoResizeRows();
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.RowTemplate.Height = 80;
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.Columns.Add(imageColumn);

                dataGridView1.Columns["ProductPhoto"].Visible = true;

                dataGridView1.Columns.Add("ProductArticul", "Артикул");
                dataGridView1.Columns.Add("Name", "Наименование товара");
                dataGridView1.Columns.Add("Desctription", "Описание");
                dataGridView1.Columns.Add("Cost", "Стоимость");
                dataGridView1.Columns.Add("Unit", "Единица измерения");
                dataGridView1.Columns["Unit"].Visible = false;
                dataGridView1.Columns.Add("ProductQuantityInStock", "Количество на складе");
                dataGridView1.Columns.Add("ProductCategory", "Категория");
                dataGridView1.Columns.Add("ProductManufactur", "Производитель");
                dataGridView1.Columns["ProductManufactur"].Visible = false;
                dataGridView1.Columns.Add("ProductSupplier", "Поставщик");
                dataGridView1.Columns["ProductSupplier"].Visible = false;

                // Возможность редактирования товара не доступна для продавца
                if (Authorization.User2.Role != 3)
                {
                    button2.Visible = false;
                    DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                    buttonEdit.Name = "Редактировать";
                    buttonEdit.HeaderText = "Редактировать";
                    buttonEdit.Text = "Редактировать";
                    buttonEdit.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(buttonEdit);
                }

                // Возможность удаления товара доступна только администратору
                if (Authorization.User2.Role != 3 && Authorization.User2.Role != 2)
                {
                    button2.Visible = false;
                    DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                    buttonDel.Name = "Удалить";
                    buttonDel.HeaderText = "Удалить";
                    buttonDel.Text = "Удалить";
                    buttonDel.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(buttonDel);
                }

                // Возможность оформления заказа доступна только для продавца
                if (Authorization.User2.Role != 1 && Authorization.User2.Role != 2)
                {
                    button3.Visible = false;
                    ContextMenuStrip menu = new ContextMenuStrip();
                    ToolStripMenuItem addToOrderMenu = new ToolStripMenuItem("Добавить в корзину");
                    addToOrderMenu.Click += AddToOrderMenuClick;
                    menu.Items.Add(addToOrderMenu);
                    dataGridView1.ContextMenuStrip = menu;
                }
                while (rdr.Read())
                {
                    string imsName = rdr[9].ToString();
                    if (string.IsNullOrEmpty(imsName))
                    {
                        imsName = "picture.png";
                    }
                    Image img = Image.FromFile(@"./photo/" + imsName);

                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["ProductPhoto"].Value = img;
                    row.Cells["ProductArticul"].Value = rdr[0];
                    row.Cells["Name"].Value = rdr[1];
                    row.Cells["Desctription"].Value = rdr[2];
                    row.Cells["Cost"].Value = rdr[3];
                    row.Cells["ProductQuantityInStock"].Value = rdr[5];
                    row.Cells["ProductCategory"].Value = rdr[8];
                    allRows1.Add(row);
                }
                totalRows1 = allRows1.Count;
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }

        // выбор страницы пагинации
        // те строки которые нам не нужны на выбраной странице - скрываем
        private void LinkLabel_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.CurrentCell = null;

            // узнаём какая страница выбрана
            LinkLabel l = sender as LinkLabel;
            if (l != null)
            {
                currentPage1 = Convert.ToInt32(l.Text) - 1;
                UpdatePag(); //Перерисовываем интерфейс
            }
        }

        //Пагинация
        private void UpdatePag()
        {
            // Очищаем только строки, не трогая столбцы
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
            int x = 252;
            int y = 669, step = 15;

            // Создаем новые LinkLabel для страниц
            for (int i = 0; i < totalPages; i++)
            {
                var linkLabel = new LinkLabel();
                linkLabel.Text = (i + 1).ToString();
                linkLabel.Name = "page" + i;
                linkLabel.ForeColor = Color.Black;
                linkLabel.Font = new Font(linkLabel.Font.FontFamily, 14);
                linkLabel.AutoSize = true;
                linkLabel.Location = new Point(x, y);
                linkLabel.Click += LinkLabel_Click;

                // Убираем подчеркивание только у текущей страницы
                if (i == currentPage1)
                {
                    linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;
                }

                this.Controls.Add(linkLabel);
                x += step;
            }

            // Обновляем состояние кнопок
            buttonPag1.Enabled = currentPage1 > 0;
            buttonPag2.Enabled = currentPage1 < totalPages - 1;

            // Обновляем счетчик записей
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}";
            labelVSE.Text = $"/ {totalRows1}";
        }
        /// <summary>
        /// Количество строк всего
        /// </summary>
        public void FillCount()
        {
            string connectionString = Authorization.Program.ConnectionString;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand totalCommand = new MySqlCommand("SELECT COUNT(*) FROM Product", connection))
                {
                    totalRecords = Convert.ToInt32(totalCommand.ExecuteScalar());
                }

                labelVSE.Text = $"/ {totalRecords}";
            }
        }

        /// <summary>
        /// Метод для получения категории
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        private int GetCategoryIdByName(string categoryName)
        {
            int categoryId = -1;
            string query = "SELECT CategoryID FROM Category WHERE CategoryName = @categoryName";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@categoryName", categoryName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        categoryId = Convert.ToInt32(result);
                    }
                }
                connection.Close();
            }
            return categoryId;
        }

        /// <summary>
        /// Фильтр, сортировка, поиск
        /// </summary>
        private void UpdateDataGrid()
        {
            string searchStr = SearchText.Text;
            string orderBy = comboBox1.SelectedItem?.ToString() == "По убыванию" ? "DESC" : "ASC";
            string selectedCategory = comboBox2.SelectedItem?.ToString();
            int categoryId = -1;

            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Все категории")
            {
                categoryId = GetCategoryIdByName(selectedCategory);
            }

            string strCmd = BuildSqlQuery(searchStr, orderBy, categoryId);
            FillDataGrid(strCmd, categoryId);
            currentPage1 = 0; // Сбрасываем на первую страницу
            UpdatePag(); // Обновляем пагинацию
        }

        private string BuildSqlQuery(string searchStr, string orderBy, int categoryId)
        {
            string strCmd = "SELECT ProductArticul, Name, Description, Cost, Unit, " +
                           "ProductQuantityInStock, ProductManufactur.ProductManufacturName, " +
                           "Supplier.SupplierName, Category.CategoryName, ProductPhoto " +
                           "FROM Product " +
                           "INNER JOIN ProductManufactur ON Product.ProductManufactur = ProductManufactur.ProductManufacturID " +
                           "INNER JOIN Supplier ON Product.ProductSupplier = Supplier.SupplierID " +
                           "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID";

            List<string> conditions = new List<string>();

            if (!string.IsNullOrWhiteSpace(searchStr) && searchStr.Length >= 3)
            {
                conditions.Add($"(Name LIKE '%{searchStr}%' OR ProductArticul LIKE '%{searchStr}%')");
            }

            if (categoryId != -1)
            {
                conditions.Add($"ProductCategory = {categoryId}");
            }

            if (conditions.Count > 0)
            {
                strCmd += " WHERE " + string.Join(" AND ", conditions);
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                strCmd += $" ORDER BY Cost {orderBy}";
            }

            return strCmd;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
            FillCount();
        }
        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
            FillCount();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
            FillCount();
        }

        /// <summary>
        /// Переход на главную форму в зависимости от роли
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Authorization.User2.Role == 1)
            {
                Admin ad = new Admin();
                ad.Show();
                this.Hide();
            }
            else if (Authorization.User2.Role == 2)
            {
                СommoditySpecialist CS = new СommoditySpecialist();
                CS.Show();
                this.Hide();
            }
            else if (Authorization.User2.Role == 3)
            {
                Seller sl = new Seller();
                sl.Show();
                this.Hide();
            }
        }

        /// <summary>
        /// Метод для добавления товара в заказ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orderADD(object sender, EventArgs e)
        {
            if (Value.clearOrder == true)
            {
                currentOrder.Clear();
                Value.clearOrder = false;
            }
            OrderForm orderForm = new OrderForm(currentOrder);
            if (orderForm.ShowDialog() == DialogResult.OK)
            {
                currentOrder = orderForm.UpdatedOrder;
            }
            FillDataGrid("SELECT " +
            "ProductArticul AS 'Артикул'," +
             "Name AS 'Наименование товара'," +
             "Description AS 'Описание'," +
             "Cost AS 'Стоимость'," +
             "Unit AS 'Единица измерения'," +
             "ProductQuantityInStock AS 'Количество на складе'," +
             "ProductManufactur.ProductManufacturName AS 'Производитель'," +
             "Supplier.SupplierName AS 'Поставщик'," +
             "Category.CategoryName AS 'Категория'," +
             "ProductPhoto " +
             "FROM Product " +
             "INNER JOIN ProductManufactur ON Product.ProductManufactur = ProductManufactur.ProductManufacturID " +
             "INNER JOIN Supplier ON Product.ProductSupplier = Supplier.SupplierID " +
             "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID", 1);
        }

        /// <summary>
        /// Метод добавления товара в корзину
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToOrderMenuClick(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string productArticleNumber = dataGridView1.CurrentRow.Cells["ProductArticul"].Value.ToString();
                int productCount = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProductQuantityInStock"].Value); // Получаем количество на складе

                if (currentOrder.ContainsKey(productArticleNumber))
                {
                    // Проверяем, не превысит ли добавление товара количество на складе
                    if (currentOrder[productArticleNumber] + 1 > productCount)
                    {
                        MessageBox.Show("Невозможно добавить больше товара, чем есть на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    currentOrder[productArticleNumber]++;
                }
                else
                {
                    // Если товара еще нет в заказе, проверяем, есть ли он на складе
                    if (productCount <= 0)
                    {
                        MessageBox.Show("Товара нет на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    currentOrder[productArticleNumber] = 1;
                }
            }
        }

        /// <summary>
        /// Кнопки Редактирования и удаления товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Authorization.User2.Role != 3)
            {
                // Проверяем, нажата ли кнопка "Редактировать"
                if (e.ColumnIndex == dataGridView1.Columns["Редактировать"].Index && e.RowIndex >= 0)
                {
                    int productId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ProductArticul"].Value);
                    ProductsEdit editForm = new ProductsEdit(productId);
                    editForm.ShowDialog();
                    UpdateDataGrid();
                    FillCount();
                    UpdatePag();
                    labelCount.Text = "Количество записей: ";
                    labelCount.Text += dataGridView1.Rows.Count;
                }
            }
            if (Authorization.User2.Role != 3 && Authorization.User2.Role != 2)
            {
                // Проверяем, что нажата кнопка "Удалить"
                if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index && e.RowIndex >= 0)
                {
                    int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ProductArticul"].Value);
                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        MessageBox.Show("Запись удалена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        DeleteRecord(id); // Удаляем запись из базы данных
                        dataGridView1.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                        UpdateDataGrid();
                        FillCount();
                        UpdatePag();
                        labelCount.Text = "Количество записей: ";
                        labelCount.Text += dataGridView1.Rows.Count;
                    }
                }
            }
        }

        /// <summary>
        /// Метод для удаления товара
        /// </summary>
        /// <param name="id"></param>
        private void DeleteRecord(int id)
        {
            string query = "DELETE FROM Product WHERE ProductArticul = @ProductArticul";
            MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString);
            con.Open();
            MySqlCommand command = new MySqlCommand(query, con);
            command.Parameters.AddWithValue("@ProductArticul", id);
            command.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>
        /// Переход на форму добавления товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void add_Click(object sender, EventArgs e)
        {
            ProductsAdd ProductsAdd = new ProductsAdd();
            ProductsAdd.ShowDialog();
            UpdateDataGrid();
            FillCount();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
        }

        /// <summary>
        /// Для выделения остатков товаров цветом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["ProductQuantityInStock"].Index && e.Value != null)
            {
                int ProductQuantityInStock = Convert.ToInt32(e.Value);

                if (ProductQuantityInStock == 0)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ff4e33");

                }
                else if (ProductQuantityInStock <= 3)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ffff66");
                }
            }
        }

        private void buttonPag1_Click(object sender, EventArgs e)
        {
            if (currentPage1 > 0)
            {
                currentPage1--;
                UpdatePag(); // Обновляем только пагинацию, без полной перезагрузки данных
            }
        }

        private void buttonPag2_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRows1 / rowsPerPage1);
            if (currentPage1 < totalPages - 1)
            {
                currentPage1++;
                UpdatePag(); // Обновляем только пагинацию, без полной перезагрузки данных
            }
        }
        /// <summary>
        /// Сбрасывает отслеживание времени бездействия.
        /// </summary>
        private void ResetInactivityTimer()
        {
            // Перезапускаем таймер
            if (Timer != null)
            {
                Timer.Stop();
                Timer.Start();
            }
        }

        /// <summary>
        /// Запускает отслеживание активности при загрузке окна.
        /// </summary>
        private void Products_Shown(object sender, EventArgs e)
        {
            Users_ActivateTracking();
        }

    }
}
