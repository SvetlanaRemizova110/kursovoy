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

    public partial class ReferenceBooks : Form
    {
        private int inactivityTimeout = 0;
        public ReferenceBooks()
        {
            InitializeComponent();
            Timer.Tick += inactivityTimer_Tick;
            Timer.Interval = 1000; // Проверка каждые 1 секунду
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
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

        //Кнопка НАЗАД
        private void button2_Click(object sender, EventArgs e)
        {
            СommoditySpecialist ad = new СommoditySpecialist();
            ad.Show();
            this.Hide();
        }

        //При загрузке формы
        private void ReferenceBooks_Load(object sender, EventArgs e)
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
            FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM `Category`");
            FillDataGridSupplier("SELECT SupplierID AS 'Идентификатор', SupplierName AS 'Поставщики' FROM `Supplier`");
            FillDataGridManufactur("SELECT ProductManufacturID AS 'Идентификатор', ProductManufacturName AS 'Производители' FROM `ProductManufactur`");
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
            textBox4.Text = "";
            string categoryName = textBox1.Text;
            if (textBox1.Text == "")
            {
                MessageBox.Show("Необходимо заполнить поле!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить категорию?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM Category WHERE CategoryName = @CategoryName", conn))
                            {
                                checkcmd.Parameters.AddWithValue("@CategoryName", categoryName);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    
                                    return;
                                }
                            }
                            string query = "INSERT INTO Category(CategoryName) VALUES (@value0)";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@value0", categoryName);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                textBox4.Text = ""; 
                                textBox1.Text = "";
                                FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM `Category`");
                            }
                            conn.Close();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Ошибка при работе с базой данных: " + ex.Message);
                            textBox1.Text = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Произошла непредвиденная ошибка: " + ex.Message);
                            textBox1.Text = "";
                        }
                    }
                }
            }
        }

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
            if(!char.IsControl(e.KeyChar) &&
                (e.KeyChar < 'а' || e.KeyChar > 'я') && 
                (e.KeyChar < 'А' || e.KeyChar > 'Я'))
            {
                e.Handled = true;
            }
            if(e.KeyChar == ' ')
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
        private void button5_Click(object sender, EventArgs e)
        {
            string categoryName = textBox1.Text;
            if (textBox1.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение измения", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    if (textBox4.Text == "")
                    {
                        MessageBox.Show("Данной записи не существует! Выберите заново.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        int сategoryID = Convert.ToInt32(textBox4.Text);
                        using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM Category WHERE CategoryName = @CategoryName", con))
                            {
                                checkcmd.Parameters.AddWithValue("@CategoryName", categoryName);
                                int count = Convert.ToInt32(checkcmd.ExecuteScalar());
                                if (count > 0)
                                {
                                    MessageBox.Show("Запись уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    return;
                                }
                            }

                            MySqlCommand cmd = new MySqlCommand(@"UPDATE Category 
                            SET CategoryID = @сategoryID,
                            CategoryName = @сategoryName
                            WHERE CategoryID = @сategoryID", con);

                            cmd.Parameters.AddWithValue("@сategoryName", textBox1.Text);
                            cmd.Parameters.AddWithValue("@сategoryID", сategoryID);
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        textBox4.Text = ""; 
                        textBox1.Text = "";
                        FillDataGridCategory("SELECT CategoryID AS 'Идентификатор', CategoryName AS 'Категории' FROM `Category`");
                    }
                }
            }
        }

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
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT COUNT(*) FROM Supplier WHERE SupplierName = @SupplierName", con))
                            {
                                checkcmd.Parameters.AddWithValue("@SupplierName", supplierName);
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
                            using (MySqlCommand checkcmd = new MySqlCommand("SELECT count(*) FROM ProductManufactur WHERE ProductManufacturName = @ProductManufacturName;", con))
                            {
                                checkcmd.Parameters.AddWithValue("@ProductManufacturName", productManufacturName);
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
        private void ReferenceBooks_Shown(object sender, EventArgs e)
        {
            Users_ActivateTracking();
        }


    }
}
