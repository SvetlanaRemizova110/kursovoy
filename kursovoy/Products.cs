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
namespace kursovoy
{
    public partial class Products : Form
    {
        // Для заказа
        private int currentPage1 = 1;
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
            GetCategoryIdByName("");
            if (Value.clearOrder == true)
            {
                currentOrder.Clear();
                Value.clearOrder = false;
            }
        }

        private void Products_Load(object sender, EventArgs e)
        {
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
            
            //label6.Text += dataGridView1.Rows.Count;
            //label6.Text = "Количество записей: ";
            //label6.Text += dataGridView1.Rows.Count;
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

        //Пагинация
        private void UpdatePag()
        {
            dataGridView1.Rows.Clear(); // Очистка существующих строк

            int startIndex = (currentPage1 - 1) * rowsPerPage1;
            int endIndex = Math.Min(startIndex + rowsPerPage1, totalRows1);

            for (int i = startIndex; i < endIndex; i++)
            {
                dataGridView1.Rows.Add(allRows1[i].Cells.Cast<DataGridViewCell>().Select(cell => cell.Value).ToArray());
            }
        }
        //Количество строк всего
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

                labelVSE.Text = $"/{totalRecords}";
                //if (SearchText.Text == "" && comboBox2.Text == "Все категории")
                //{
                //    labelVSE.Visible = false;

                //}
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

            // Получение выбранной категории
            string selectedCategory = comboBox2.SelectedItem?.ToString();
            int categoryId = -1;

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                categoryId = GetCategoryIdByName(selectedCategory); //метод для получения категории
            }

            string strCmd = "SELECT " +
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
             "INNER JOIN Category ON Product.ProductCategory = Category.CategoryID";

            if (SearchText.Text == "")
            {
                // Заполнение DataGrid
                FillDataGrid(strCmd, categoryId);
            }
            // Поиск
            if (!string.IsNullOrWhiteSpace(searchStr) && searchStr.Length >= 3)
            {
                strCmd += $" AND (Name LIKE '%{searchStr}%' OR ProductArticul LIKE '%{searchStr}%')";
                // Заполнение DataGrid
                FillDataGrid(strCmd, categoryId);
            }
            if (comboBox2.Text != "")
            {
                // Фильтрация по категориям
                if (categoryId != -1) // Проверяем, если categoryId был найден
                {
                    strCmd += $" AND ProductCategory = @categoryId";
                    // Заполнение DataGrid
                    FillDataGrid(strCmd, categoryId);
                }
            }
            else if (comboBox2.Text == "Все категории")
            {
                _ = strCmd;
                // Заполнение DataGrid
                FillDataGrid(strCmd, categoryId);
            }
            if (comboBox1.Text != "")
            {
                // Сортировка
                strCmd += $" ORDER BY Cost {orderBy}";
                // Заполнение DataGrid
                FillDataGrid(strCmd, categoryId);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            FillCount();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
        }
        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            FillCount();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            FillCount();
            UpdatePag();
            labelCount.Text = "Количество записей: ";
            labelCount.Text += dataGridView1.Rows.Count;
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
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FF0000"); 

                } else if (ProductQuantityInStock <= 3)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ffff00"); 
                }
            }
        }

        private void buttonPag2_Click(object sender, EventArgs e)
        {
            if (currentPage1 * rowsPerPage1 < totalRows1)
            {
                currentPage1++;
                UpdateDataGrid();
                UpdatePag();
                FillCount();
                labelCount.Text = "Количество записей: ";
                labelCount.Text += dataGridView1.Rows.Count;

            }
        }

        private void buttonPag1_Click(object sender, EventArgs e)
        {
            if (currentPage1 > 1)
            {
                currentPage1--;
                UpdateDataGrid();
                UpdatePag();
                FillCount();
                labelCount.Text = "Количество записей: ";
                labelCount.Text += dataGridView1.Rows.Count;

            }

        }
    }
}
