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
using System.Globalization;

namespace kursovoy
{
    public partial class ReferenceBooks : Form
    {
        public ReferenceBooks()
        {
            InitializeComponent();
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
        }
        /// <summary>
        /// Кнопка НАЗАД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void ReferenceBooks_Load(object sender, EventArgs e)
        {
            role.Text = Authorization.User2.RoleName + ": " + Authorization.User2.FIO;
            FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM `Category`");
            FillDataGridSupplier("SELECT SupplierID AS 'Идентификатор', SupplierName AS 'Поставщики' FROM `Supplier`");
            FillDataGridManufactur("SELECT ProductManufacturID AS 'Идентификатор', ProductManufacturName AS 'Производители' FROM `ProductManufactur`");
            label3.Text += " " + dataGridView1.Rows.Count;
            label4.Text += " " + dataGridView2.Rows.Count;
            label6.Text += " " + dataGridView3.Rows.Count;
        }
        /// <summary>
        /// Для вывода категории
        /// </summary>
        /// <param name="strCmd"></param>
        public void FillDataGridCategory(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Program1.ConnectionString);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;

                dataGridView1.Columns.Add("CategoryID", "Идентификатор");
                dataGridView1.Columns["CategoryID"].Visible = false;
                dataGridView1.Columns.Add("CategoryName", "Категории");
                dataGridView1.Columns["CategoryName"].Width = 250;

                DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                buttonEdit.Name = "Выбрать";
                buttonEdit.HeaderText = "Выбрать";
                buttonEdit.Text = "Выбрать";
                buttonEdit.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonEdit);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["CategoryID"].Value = rdr[0];
                    row.Cells["CategoryName"].Value = rdr[1];
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Для вывода поставщика
        /// </summary>
        /// <param name="strCmd"></param>
        public void FillDataGridSupplier(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Program1.ConnectionString);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView2.Rows.Count; ++i)
                {
                    dataGridView2.Rows[i].Visible = true;
                }
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView2.ReadOnly = true;
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.AllowUserToDeleteRows = false;
                dataGridView2.AllowUserToOrderColumns = false;
                dataGridView2.AllowUserToResizeColumns = false;
                dataGridView2.AllowUserToResizeRows = false;

                dataGridView2.Columns.Add("SupplierID", "Идентификатор");
                dataGridView2.Columns["SupplierID"].Visible = false;
                dataGridView2.Columns.Add("SupplierName", "Поставщики");
                dataGridView2.Columns["SupplierName"].Width = 250;

                DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                buttonEdit.Name = "Выбрать";
                buttonEdit.HeaderText = "Выбрать";
                buttonEdit.Text = "Выбрать";
                buttonEdit.UseColumnTextForButtonValue = true;
                dataGridView2.Columns.Add(buttonEdit);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView2.Rows.Add();
                    DataGridViewRow row = dataGridView2.Rows[rowIndex];
                    row.Cells["SupplierID"].Value = rdr[0];
                    row.Cells["SupplierName"].Value = rdr[1];
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Для вывода производителя
        /// </summary>
        /// <param name="strCmd"></param>
        public void FillDataGridManufactur(string strCmd)
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Program1.ConnectionString);
                con.Open();
                MySqlCommand command = new MySqlCommand(strCmd, con);
                MySqlDataReader rdr = command.ExecuteReader();
                for (int i = 0; i < dataGridView3.Rows.Count; ++i)
                {
                    dataGridView3.Rows[i].Visible = true;
                }
                dataGridView3.Rows.Clear();
                dataGridView3.Columns.Clear();
                dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView3.ReadOnly = true;
                dataGridView3.AllowUserToAddRows = false;
                dataGridView3.AllowUserToDeleteRows = false;
                dataGridView3.AllowUserToOrderColumns = false;
                dataGridView3.AllowUserToResizeColumns = false;
                dataGridView3.AllowUserToResizeRows = false;

                dataGridView3.Columns.Add("ProductManufacturID", "Идентификатор");
                dataGridView3.Columns["ProductManufacturID"].Visible = false;
                dataGridView3.Columns.Add("ProductManufacturName", "Производители");
                dataGridView3.Columns["ProductManufacturName"].Width = 250;

                DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                buttonEdit.Name = "Выбрать";
                buttonEdit.HeaderText = "Выбрать";
                buttonEdit.Text = "Выбрать";
                buttonEdit.UseColumnTextForButtonValue = true;
                dataGridView3.Columns.Add(buttonEdit);
                while (rdr.Read())
                {
                    int rowIndex = dataGridView3.Rows.Add();
                    DataGridViewRow row = dataGridView3.Rows[rowIndex];
                    row.Cells["ProductManufacturID"].Value = rdr[0];
                    row.Cells["ProductManufacturName"].Value = rdr[1];
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Добавление категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            AddOrUpdateCategory(false); // false - добавление
        }
        /// <summary>
        /// Изменение категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            AddOrUpdateCategory(true); // true - редактирование
        }
        /// <summary>
        /// Общий метод для добавления и редактирования категорий
        /// </summary>
        /// <param name="isUpdate">True - редактирование, False - добавление</param>
        private void AddOrUpdateCategory(bool isUpdate)
        {
            string categoryName = textBox1.Text.Trim();
            string categoryId = textBox4.Text.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isUpdate && string.IsNullOrEmpty(categoryId))
            {
                MessageBox.Show("Необходимо выбрать запись для изменения!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string message = isUpdate ? "Вы уверены, что хотите изменить эту запись ?" : "Вы уверены, что хотите добавить эту запись?";
            string title = isUpdate ? "Подтверждение изменения!" : "Подтверждение добавления!";
            MessageBoxIcon icon = isUpdate ? MessageBoxIcon.Warning : MessageBoxIcon.Question; 

            DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon);
            if (dialogResult == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(Program1.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        if (!isUpdate)
                        {
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT COUNT(*) FROM Category WHERE CategoryName = @CategoryName", conn))
                            {
                                checkcmd.Parameters.AddWithValue("@CategoryName", categoryName);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Категория с таким названием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        string query;
                        MySqlCommand cmd;
                        if (isUpdate)
                        {
                            // Редактирование записи
                            query = "UPDATE Category SET CategoryName = @CategoryName WHERE CategoryID = @CategoryID";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                            cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        }
                        else
                        {
                            // Добавление записи
                            query = "INSERT INTO Category(CategoryName) VALUES (@CategoryName)";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        }
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Запись {(isUpdate ? "изменена!" : "добавлена!")}.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            textBox4.Text = "";
                            textBox1.Text = "";
                            FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM Category");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Проверка ввода для категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                (e.KeyChar < 'а' || e.KeyChar > 'я') &&
                (e.KeyChar < 'А' || e.KeyChar > 'Я'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = false;
            }
        }
        /// <summary>
        /// Проверка ввода для поставщика и производителя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
               (e.KeyChar < 'а' || e.KeyChar > 'я') &&
               (e.KeyChar < 'А' || e.KeyChar > 'Я') &&
                  (e.KeyChar < 'A' || e.KeyChar > 'Z') &&
                  (e.KeyChar < 'a' || e.KeyChar > 'z'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = false;
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox4.Text = row.Cells["CategoryID"].Value.ToString(); 
                textBox1.Text = row.Cells["CategoryName"].Value.ToString(); 
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                textBox5.Text = row.Cells["SupplierID"].Value.ToString(); 
                textBox2.Text = row.Cells["SupplierName"].Value.ToString(); 
            }
        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView3.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView3.Rows[e.RowIndex];
                textBox6.Text = row.Cells["ProductManufacturID"].Value.ToString();
                textBox3.Text = row.Cells["ProductManufacturName"].Value.ToString(); 
            }
        }
        /// <summary>
        /// Добавление поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            AddOrUpdateSupplier(false); // false - добавление
        }
        /// <summary>
        /// Изменение поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e) // Assuming you have a button6 for updating
        {
            AddOrUpdateSupplier(true); // true - редактирование
        }
        /// <summary>
        /// Общий метод для добавления и редактирования поставщиков
        /// </summary>
        /// <param name="isUpdate">True - редактирование, False - добавление</param>
        private void AddOrUpdateSupplier(bool isUpdate)
        {
            string supplierName = textBox2.Text.Trim();
            string supplierId = textBox5.Text.Trim(); 
            if (string.IsNullOrEmpty(supplierName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isUpdate && string.IsNullOrEmpty(supplierId))
            {
                MessageBox.Show("Необходимо выбрать запись для изменения!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string message = isUpdate ? "Вы уверены, что хотите изменить эту запись ?" : "Вы уверены, что хотите добавить эту запись?";
            string title = isUpdate ? "Подтверждение изменения!" : "Подтверждение добавления!";
            MessageBoxIcon icon = isUpdate ? MessageBoxIcon.Warning : MessageBoxIcon.Question;

            DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon);
            if (dialogResult == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(Program1.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        if (!isUpdate)
                        {
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT COUNT(*) FROM Supplier WHERE SupplierName = @SupplierName", conn))
                            {
                                checkcmd.Parameters.AddWithValue("@SupplierName", supplierName);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Поставщик с таким названием уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        string query;
                        MySqlCommand cmd;
                        if (isUpdate)
                        {
                            query = "UPDATE Supplier SET SupplierName = @SupplierName WHERE SupplierID = @SupplierID";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                            cmd.Parameters.AddWithValue("@SupplierID", supplierId);
                        }
                        else
                        {
                            query = "INSERT INTO Supplier(SupplierName) VALUES (@SupplierName)";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                        }
                        MessageBox.Show($"Запись {(isUpdate ? "изменена!" : "добавлена!")}.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox2.Text = "";
                        textBox5.Text = "";
                        FillDataGridSupplier("SELECT SupplierID AS 'Идентификатор', SupplierName AS 'Поставщики' FROM Supplier");
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// Добавление или изменение производителя
        /// </summary>
        /// <param name="isUpdate">True - изменение, False - добавление</param>
        private void AddOrUpdateManufacturer(bool isUpdate)
        {
            string manufacturerName = textBox3.Text.Trim(); //Trim() - удаляет все пробелы из строки.
            string manufacturerId = textBox5.Text.Trim();   
            if (string.IsNullOrEmpty(manufacturerName))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isUpdate && string.IsNullOrEmpty(manufacturerId))
            {
                MessageBox.Show("Необходимо выбрать запись для изменения!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string message = isUpdate ? "Вы уверены, что хотите изменить эту запись ?" : "Вы уверены, что хотите добавить эту запись?";
            string title = isUpdate ? "Подтверждение изменения!" : "Подтверждение добавления!";
            MessageBoxIcon icon = isUpdate ? MessageBoxIcon.Warning : MessageBoxIcon.Question;

            DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon);
            if (dialogResult == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(Program1.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        if (!isUpdate)
                        {
                            using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM ProductManufactur WHERE ProductManufacturName = @ProductManufacturName", conn))
                            {
                                checkCmd.Parameters.AddWithValue("@ProductManufacturName", manufacturerName);
                                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Производитель с таким именем уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        string query;
                        MySqlCommand cmd;
                        if (isUpdate)
                        {
                            query = "UPDATE ProductManufactur SET ProductManufacturName = @ProductManufacturName WHERE ProductManufacturID = @ProductManufacturID";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@ProductManufacturName", manufacturerName);
                            cmd.Parameters.AddWithValue("@ProductManufacturID", manufacturerId);
                        }
                        else
                        {
                            query = "INSERT INTO ProductManufactur (ProductManufacturName) VALUES (@ProductManufacturName)";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@ProductManufacturName", manufacturerName);
                        }
                        MessageBox.Show($"Запись {(isUpdate ? "изменена!" : "добавлена!")}.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox3.Text = "";
                        textBox5.Text = "";
                        FillDataGridManufactur("SELECT ProductManufacturID AS 'Идентификатор', ProductManufacturName AS 'Производители' FROM ProductManufactur"); // Refresh data grid
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            AddOrUpdateManufacturer(false);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            AddOrUpdateManufacturer(true);
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView2.ClearSelection();
        }
        private void dataGridView3_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView3.ClearSelection();
        }
    }
}
