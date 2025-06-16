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
using System.Configuration;
namespace kursovoy
{
    public partial class Products : Form
    {
        private FormWindowState _previousWindowState; 
        private FormBorderStyle _previousBorderStyle; 
        private ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
        private int currentPage1 = 0;
        private int rowsPerPage1 = 20;
        private int totalRows1 = 0;
        private int totalRecords;//кол-во строк всего
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();
        public static Dictionary<string, int> currentOrder = new Dictionary<string, int>();
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
            LoadDataIntoComboBox();
        }
        /// <summary>
        /// Для загрузки данных в comboBox
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            try
            {
                string query3 = "SELECT CategoryName FROM Category";
            comboBox2.Items.Add("Все категории");
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                MySqlCommand command3 = new MySqlCommand(query3, connection);
                using (MySqlDataReader reader3 = command3.ExecuteReader())
                {
                    while (reader3.Read())
                    {
                        comboBox2.Items.Add(reader3["CategoryName"].ToString());
                    }
                }
                connection.Close();
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
                _previousWindowState = this.WindowState; 
                _previousBorderStyle = this.FormBorderStyle;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// Событие при загрузке формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Products_Load(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            FillCount();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            labelVSE.Visible = false;
            label7.Text = Authorization.User2.RoleName + ": " + Authorization.User2.FIO;
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
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.RowTemplate.Height = 80;
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;

                dataGridView1.Columns.Add("ProductArticul", "Артикул");
                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                imageColumn.Name = "ProductPhoto";
                imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                imageColumn.HeaderText = "Фото";
                dataGridView1.Columns.Add(imageColumn);
                dataGridView1.Columns["ProductPhoto"].Visible = true;
                dataGridView1.Columns.Add("Name", "Название товара");
                dataGridView1.Columns["Name"].Width = 150;
                dataGridView1.Columns.Add("Desctription", "Описание");
                dataGridView1.Columns["Desctription"].Width = 150;
                dataGridView1.Columns.Add("Cost", "Стоимость");
                dataGridView1.Columns["Cost"].Width = 100;
                dataGridView1.Columns.Add("Unit", "Единица измерения");
                dataGridView1.Columns.Add("ProductQuantityInStock", "Количество на складе");
                dataGridView1.Columns.Add("ProductCategory", "Категория");
                dataGridView1.Columns.Add("ProductManufactur", "Производитель");
                dataGridView1.Columns["ProductManufactur"].Width = 120;
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
                    dataGridView1.Columns["Редактировать"].Width = 120;
                    dataGridView1.Columns["ProductManufactur"].Visible = false;
                    dataGridView1.Columns["Unit"].Visible = false;
                }

                // Возможность удаления товара доступна только администратору
                if (Authorization.User2.Role == 1)
                {
                    button2.Visible = false;
                    DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                    buttonDel.Name = "Удалить";
                    buttonDel.HeaderText = "Удалить";
                    buttonDel.Text = "Удалить";
                    buttonDel.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(buttonDel);
                }
                if (Authorization.User2.Role == 2)
                {
                    dataGridView1.Size = new Size(987, 452);
                }
                // Возможность оформления заказа доступна только для продавца
                if (Authorization.User2.Role == 3)
                {
                    button3.Visible = false;
                    contextMenuStrip1.Items.Clear();
                    ToolStripMenuItem addToOrderMenu = new ToolStripMenuItem("Добавить в корзину");
                    addToOrderMenu.Click += AddToOrderMenuClick;
                    contextMenuStrip1.Items.Add(addToOrderMenu);
                    dataGridView1.ContextMenuStrip = contextMenuStrip1;
                }
                while (rdr.Read())
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["ProductArticul"].Value = rdr[0];
                    string imsName = rdr[9].ToString();
                    if (string.IsNullOrEmpty(imsName))
                    {
                        imsName = "picture.png";
                    }
                    Image img = Image.FromFile(@"./photo/" + imsName);
                    row.Cells["ProductPhoto"].Value = img;
                    row.Cells["Name"].Value = rdr[1];
                    row.Cells["Desctription"].Value = rdr[2];
                    row.Cells["Cost"].Value = rdr[3];
                    row.Cells["Unit"].Value = rdr[4];
                    row.Cells["ProductQuantityInStock"].Value = rdr[5];
                    row.Cells["ProductCategory"].Value = rdr[8];
                    row.Cells["ProductManufactur"].Value = rdr[7];
                    allRows1.Add(row);
                }
                totalRows1 = allRows1.Count;
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex}");
            }
        }
        /// <summary>
        /// выбор страницы пагинации
        /// не нужные строки на выбраной странице - скрываем
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkLabel_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Пагинация
        /// </summary>
        private void UpdatePag()
        {
            try
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
                    // Устанавливаем цвет ссылок
                    linkLabel.LinkColor = Color.Black;
                    linkLabel.VisitedLinkColor = Color.Black;
                    linkLabel.ActiveLinkColor = Color.Black;

                    linkLabel.Font = new Font(linkLabel.Font.FontFamily, 14);
                    linkLabel.AutoSize = true;
                    linkLabel.Location = new Point(x, y);
                    linkLabel.Click += LinkLabel_Click;
                    linkLabel.LinkBehavior = LinkBehavior.NeverUnderline;

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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Количество строк всего
        /// </summary>
        public void FillCount()
        {
            try
            {
                string conStr = Authorization.Program.ConnectionString;
                using (MySqlConnection connection = new MySqlConnection(conStr))
                {
                    connection.Open();
                    using (MySqlCommand totalCommand = new MySqlCommand("SELECT COUNT(*) FROM Product", connection))
                    {
                        totalRecords = Convert.ToInt32(totalCommand.ExecuteScalar());
                    }
                    labelVSE.Text = $"/{totalRecords}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
            try
            {
                string searchStr = SearchText.Text;
                string orderByClause;
                if (comboBox1.SelectedItem?.ToString() == "По убыванию")
                {
                    orderByClause = "Cost DESC";
                }
                else if (comboBox1.SelectedItem?.ToString() == "По возрастанию")
                {
                    orderByClause = "Cost ASC";
                }
                else
                {
                    orderByClause = "ProductArticul ASC";  // сортировка по артикулу
                }
                string selectedCategory = comboBox2.SelectedItem?.ToString();
                int categoryId = -1;
                if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Все категории")
                {
                    categoryId = GetCategoryIdByName(selectedCategory);
                }
                string strCmd = BuildSqlQuery(searchStr, orderByClause, categoryId);
                FillDataGrid(strCmd, categoryId);
                currentPage1 = 0; // Сбрасываем на первую страницу
                UpdatePag(); // Обновляем пагинацию
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private string BuildSqlQuery(string searchStr, string orderByClause, int categoryId)
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
                conditions.Add($"(Name LIKE '%{searchStr}%' OR ProductArticul LIKE '{searchStr}%')");
            }
            if (categoryId != -1)
            {
                conditions.Add($"ProductCategory = {categoryId}");
            }
            if (conditions.Count > 0)
            {
                strCmd += " WHERE " + string.Join(" AND ", conditions);
            }
            if (!string.IsNullOrEmpty(orderByClause))
            {
                if (orderByClause.Contains("ProductArticul"))
                {
                    strCmd += $" ORDER BY CONVERT(ProductArticul, UNSIGNED) {orderByClause.Split(' ')[1]}";
                }
                else
                {
                    strCmd += $" ORDER BY {orderByClause}";
                }
            }
            else
            {
                strCmd += $" ORDER BY CONVERT(ProductArticul, UNSIGNED) ASC";
            }
            return strCmd;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            FillCount();
        }
        private void SearchText_TextChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            FillCount();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGrid();
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
            FillCount();
        }

        /// <summary>
        /// Переход на главную форму в зависимости от роли
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Authorization.User2.Role == 1)
                {
                    Admin ad = new Admin();
                    ad.Show();
                    this.Close();
                }
                else if (Authorization.User2.Role == 2)
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Метод для добавления товара в заказ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orderADD(object sender, EventArgs e)
        {
            try
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
                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Метод добавления товара в корзину
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToOrderMenuClick(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow != null)
                {
                    string productArticleNumber = dataGridView1.CurrentRow.Cells["ProductArticul"].Value.ToString();
                    int productCount = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProductQuantityInStock"].Value);
                    if (currentOrder.ContainsKey(productArticleNumber))
                    {
                        // Проверяем, не превысит ли добавление товара количество на складе
                        if (currentOrder[productArticleNumber] + 1 > productCount)
                        {
                            MessageBox.Show("Невозможно добавить больше товара, чем есть на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        currentOrder[productArticleNumber]++;
                        Value.clearOrder = false;
                    }
                    else
                    {
                        if (productCount <= 0)
                        {
                            MessageBox.Show("Товара нет на складе.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        currentOrder[productArticleNumber] = 1;
                        Value.clearOrder = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
                    UpdatePag();
                    labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
                }
            }
            if (Authorization.User2.Role != 3 && Authorization.User2.Role != 2)
            {
                // Проверяем, что нажата кнопка "Удалить"
                if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index && e.RowIndex >= 0)
                {
                    int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ProductArticul"].Value);
                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            DeleteRecord(id); // Удаляем запись из базы данных
                            MessageBox.Show("Запись удалена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            UpdatePag();
                            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
                            FillCount();
                            UpdateDataGrid();
                        }
                        catch (MySqlException ex) when (ex.Number == 1451)
                        {
                            MessageBox.Show("Невозможно удалить товар, так как он используется в других таблицах.",
                                           "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                                           "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
            UpdatePag();
            labelCount.Text = $"Количество записей: {dataGridView1.Rows.Count}" + labelVSE.Text;
        }
        /// <summary>
        /// Для выделения остатков товаров цветом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == dataGridView1.Columns["ProductQuantityInStock"].Index && e.Value != null)
                {
                    int ProductQuantityInStock = Convert.ToInt32(e.Value);

                    if (ProductQuantityInStock == 0)
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;

                    }
                    else if (ProductQuantityInStock <= 3)
                    {
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#ffff66");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void buttonPag1_Click(object sender, EventArgs e)
        {
            if (currentPage1 > 0)
            {
                currentPage1--;
                UpdatePag(); // Обновляем только пагинацию
                FillCount();
            }
        }
        private void buttonPag2_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRows1 / rowsPerPage1);
            if (currentPage1 < totalPages - 1)
            {
                currentPage1++;
                UpdatePag(); // Обновляем только пагинацию
                FillCount();
            }
        }
        private void Products_SizeChanged(object sender, EventArgs e)
        {
            UpdatePag();
        }
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}
