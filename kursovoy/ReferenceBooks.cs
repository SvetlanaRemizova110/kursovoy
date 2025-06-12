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

        //Кнопка НАЗАД
        private void button2_Click(object sender, EventArgs e)
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

        //При загрузке формы
        private void ReferenceBooks_Load(object sender, EventArgs e)
        {
            role.Text = Authorization.User2.RoleName + ": " + Authorization.User2.FIO;
            //empl.Text = "Сотрудник: " + ;

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
                MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString);
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
                throw new Exception($"Ошибка: {ex}");
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
                MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString);
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
                throw new Exception($"Ошибка: {ex}");
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
                MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString);
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
                throw new Exception($"Ошибка: {ex}");
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
            string categoryName = textBox1.Text.Trim(); // Trim убирает пробелы в начале и конце
            string categoryId = textBox4.Text.Trim(); //  Для редактирования

            if (string.IsNullOrEmpty(categoryName)) // Проверка на пустоту более современным способом
            {
                MessageBox.Show("Необходимо заполнить поле!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isUpdate && string.IsNullOrEmpty(categoryId))
            {
                MessageBox.Show("Необходимо выбрать категорию для редактирования!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string message = isUpdate ? "Вы уверены, что хотите изменить эту категорию?" : "Вы уверены, что хотите добавить категорию?";
            string title = isUpdate ? "Подтверждение изменения" : "Подтверждение добавления";
            MessageBoxIcon icon = isUpdate ? MessageBoxIcon.Warning : MessageBoxIcon.Question; // Более подходящие иконки

            DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon);
            if (dialogResult == DialogResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    try
                    {
                        conn.Open();

                        // Проверка на существование записи (ТОЛЬКО для добавления)
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
                            cmd.Parameters.AddWithValue("@CategoryID", categoryId); // Используем ID
                        }
                        else
                        {
                            // Добавление записи
                            query = "INSERT INTO Category(CategoryName) VALUES (@CategoryName)";
                            cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        }

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Запись успешно обновлена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //int rowsAffected = cmd.ExecuteNonQuery();

                        //if (rowsAffected > 0)
                        //{
                        MessageBox.Show($"Категория {(isUpdate ? "успешно изменена" : "успешно добавлена")}.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            textBox4.Text = "";
                            textBox1.Text = "";
                            FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM Category"); // Обновляем DataGridView
                        //}
                        //else
                        //{
                        //    MessageBox.Show($"Не удалось {(isUpdate ? "изменить" : "добавить")} категорию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //}
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Добавление категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    textBox4.Text = "";
        //    string categoryName = textBox1.Text;
        //    if (textBox1.Text == "")
        //    {
        //        MessageBox.Show("Необходимо заполнить поле!");
        //    }
        //    else
        //    {
        //        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить категорию?", "Подтверждение удаления", MessageBoxButtons.YesNo);
        //        if (dialogResult == DialogResult.Yes)
        //        {
        //            using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString))
        //            {
        //                try
        //                {
        //                    conn.Open();
        //                    using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM Category WHERE CategoryName = @CategoryName", conn))
        //                    {
        //                        checkcmd.Parameters.AddWithValue("@CategoryName", categoryName);
        //                        int count = Convert.ToInt32(checkcmd.ExecuteScalar());
        //                        if (count > 0)
        //                        {
        //                            MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //                            return;
        //                        }
        //                    }
        //                    string query = "INSERT INTO Category(CategoryName) VALUES (@value0)";
        //                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
        //                    {
        //                        cmd.Parameters.AddWithValue("@value0", categoryName);
        //                        cmd.ExecuteNonQuery();
        //                        MessageBox.Show("Запись добавлена.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                        textBox4.Text = "";
        //                        textBox1.Text = "";
        //                        FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM `Category`");
        //                    }
        //                    conn.Close();
        //                }
        //                catch (MySqlException ex)
        //                {
        //                    MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message);
        //                    textBox1.Text = "";
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
        //                    textBox1.Text = "";
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Добавление поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string supplierName = textBox2.Text;
            if (textBox2.Text == "")
            {
                MessageBox.Show("Необходимо заполнить поле!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить поставщика?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT COUNT(*) FROM Supplier WHERE SupplierName = @SupplierName", conn))
                            {
                                checkcmd.Parameters.AddWithValue("@SupplierName", supplierName);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            string query = "INSERT INTO Supplier(SupplierName) VALUES (@SupplierName)";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                textBox2.Text = "";
                                FillDataGridSupplier("SELECT SupplierID AS 'Идентификатор', SupplierName AS 'Поставщики' FROM Supplier");
                            }
                            conn.Close();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message);
                            textBox2.Text = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
                            textBox2.Text = "";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Добавление производителя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            string productManufacturName = textBox3.Text;
            if (string.IsNullOrEmpty(productManufacturName))
            {
                MessageBox.Show("Необходимо заполнить поле!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить производителя?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM ProductManufactur WHERE ProductManufacturName = @ProductManufacturName;", conn))
                            {
                                checkcmd.Parameters.AddWithValue("@ProductManufacturName", productManufacturName);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            string query = "INSERT INTO ProductManufactur(ProductManufacturName) VALUES (@ProductManufacturName)"; // Используем productManufacturName
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ProductManufacturName", productManufacturName); // Используем productManufacturName
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                textBox3.Text = "";
                                FillDataGridManufactur("SELECT ProductManufacturID AS 'Идентификатор', ProductManufacturName AS 'Производители' FROM ProductManufactur");
                            }
                            conn.Close();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message);
                            textBox3.Text = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
                            textBox3.Text = "";
                        }
                    }
                }
            }
        }

        //Проверка ввода для категории
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
        //Проверка ввода для поставщика и производителя
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

        /// <summary>
        /// Изменение категории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void button5_Click(object sender, EventArgs e)
        //{
        //    string categoryName = textBox1.Text;
        //    if (textBox1.Text == "")
        //    {
        //        MessageBox.Show("Необходимо заполнить все поля!");
        //    }
        //    else
        //    {
        //        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение измения", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        //        if (dialogResult == DialogResult.Yes)
        //        {

        //            if (textBox4.Text == "")
        //            {
        //                MessageBox.Show("Данной записи не существует! Выберите заново.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //            else
        //            {
        //                int сategoryID = Convert.ToInt32(textBox4.Text);
        //                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
        //                {
        //                    con.Open();
        //                    using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM Category WHERE CategoryName = @CategoryName AND CategoryID != @CategoryID", con))
        //                    {
        //                        checkcmd.Parameters.AddWithValue("@CategoryName", categoryName);
        //                        checkcmd.Parameters.AddWithValue("@CategoryID", сategoryID);
        //                        int count = Convert.ToInt32(checkcmd.ExecuteScalar());
        //                        if (count > 0)
        //                        {
        //                            MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                            return;
        //                        }
        //                    }

        //                    MySqlCommand cmd = new MySqlCommand(@"UPDATE Category 
        //                SET CategoryID = @сategoryID,
        //                CategoryName = @сategoryName
        //                WHERE CategoryID = @сategoryID", con);

        //                    cmd.Parameters.AddWithValue("@сategoryName", textBox1.Text);
        //                    cmd.Parameters.AddWithValue("@сategoryID", сategoryID);
        //                    cmd.ExecuteNonQuery();
        //                    con.Close();
        //                }
        //                textBox4.Text = "";
        //                textBox1.Text = "";
        //                FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM `Category`");
        //            }
        //        }
        //    }
        //}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox4.Text = row.Cells["CategoryID"].Value.ToString(); //id
                textBox1.Text = row.Cells["CategoryName"].Value.ToString(); //сategoryName
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                textBox5.Text = row.Cells["SupplierID"].Value.ToString(); //id
                textBox2.Text = row.Cells["SupplierName"].Value.ToString(); //
            }
        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView3.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView3.Rows[e.RowIndex];
                textBox6.Text = row.Cells["ProductManufacturID"].Value.ToString(); //id
                textBox3.Text = row.Cells["ProductManufacturName"].Value.ToString(); //сategoryName
            }
        }

        /// <summary>
        /// Изменение поставщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            string supplierName = textBox2.Text;
            if (textBox2.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение измения", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    if (textBox5.Text == "")
                    {
                        MessageBox.Show("Данной записи не существует! Выберите заново.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        int supplierID = Convert.ToInt32(textBox5.Text);
                        using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT COUNT(*) FROM Supplier WHERE SupplierName = @SupplierName AND SupplierID != @SupplierID", con))
                            {
                                checkcmd.Parameters.AddWithValue("@SupplierName", supplierName);
                                checkcmd.Parameters.AddWithValue("@SupplierID", supplierID);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            MySqlCommand cmd = new MySqlCommand(@"UPDATE Supplier 
                            SET SupplierID = @supplierID,
                            SupplierName = @supplierName
                            WHERE SupplierID = @supplierID", con);

                            cmd.Parameters.AddWithValue("@supplierName", textBox2.Text);
                            cmd.Parameters.AddWithValue("@supplierID", supplierID);
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        textBox5.Text = ""; //id
                        textBox2.Text = "";//сategoryName
                        FillDataGridSupplier("SELECT SupplierID AS 'Идентификатор', SupplierName AS 'Поставщики' FROM `Supplier`");
                    }
                }
            }
        }

        /// <summary>
        /// Изменение производителя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            string productManufacturName = textBox3.Text;
            if (textBox3.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение измения", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    if (textBox6.Text == "")
                    {
                        MessageBox.Show("Данной записи не существует! Выберите заново.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        int productManufacturID = Convert.ToInt32(textBox6.Text);
                        using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM ProductManufactur WHERE ProductManufacturName = @ProductManufacturName AND ProductManufacturID != @ProductManufacturID;", con))
                            {
                                checkcmd.Parameters.AddWithValue("@ProductManufacturName", productManufacturName);
                                checkcmd.Parameters.AddWithValue("@ProductManufacturID", productManufacturID);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            MySqlCommand cmd = new MySqlCommand(@"UPDATE ProductManufactur 
                            SET ProductManufacturID = @productManufacturID,
                            ProductManufacturName = @productManufacturName
                            WHERE ProductManufacturID = @productManufacturID", con);

                            cmd.Parameters.AddWithValue("@productManufacturName", textBox3.Text);
                            cmd.Parameters.AddWithValue("@productManufacturID", productManufacturID);
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        textBox4.Text = ""; //id
                        textBox1.Text = "";//сategoryName
                        FillDataGridManufactur("SELECT ProductManufacturID AS 'Идентификатор', ProductManufacturName AS 'Производители' FROM `ProductManufactur`");
                    }
                }
            }
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

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    // Сохраняем текущее положение курсора
        //    int selectionStart = textBox1.SelectionStart;
        //    int selectionLength = textBox1.SelectionLength;

        //    // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
        //    string[] words = textBox1.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        if (words[i].Length > 0) // Проверка длины слова
        //        {
        //            words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
        //        }
        //    }
        //    textBox1.Text = string.Join(" ", words);

        //    // Восстанавливаем положение курсора
        //    textBox1.SelectionStart = Math.Min(selectionStart, textBox1.Text.Length);
        //    textBox1.SelectionLength = selectionLength;
        //}

        //private void textBox2_TextChanged(object sender, EventArgs e)
        //{
        //    // Сохраняем текущее положение курсора
        //    int selectionStart = textBox2.SelectionStart;
        //    int selectionLength = textBox2.SelectionLength;

        //    // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
        //    string[] words = textBox2.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        if (words[i].Length > 0) // Проверка длины слова
        //        {
        //            words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
        //        }
        //    }
        //    textBox2.Text = string.Join(" ", words);

        //    // Восстанавливаем положение курсора
        //    textBox2.SelectionStart = Math.Min(selectionStart, textBox2.Text.Length);
        //    textBox2.SelectionLength = selectionLength;
        //}

        //private void textBox3_TextChanged(object sender, EventArgs e)
        //{
        //    // Сохраняем текущее положение курсора
        //    int selectionStart = textBox3.SelectionStart;
        //    int selectionLength = textBox3.SelectionLength;

        //    // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
        //    string[] words = textBox3.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        if (words[i].Length > 0) // Проверка длины слова
        //        {
        //            words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
        //        }
        //    }
        //    textBox3.Text = string.Join(" ", words);

        //    // Восстанавливаем положение курсора
        //    textBox3.SelectionStart = Math.Min(selectionStart, textBox3.Text.Length);
        //    textBox3.SelectionLength = selectionLength;
        //}

        //private void tabPage1_Click(object sender, EventArgs e)
        //{

        //}
    }
}
