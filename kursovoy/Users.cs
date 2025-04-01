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
using System.Security.Cryptography;

namespace kursovoy
{
    public partial class Users : Form
    {
        public Users()
        {
            InitializeComponent();
            LoadDataIntoComboBox();
            textBox2.Visible = false;
        }
        //Кнопка НАЗАД
        private void button4_Click(object sender, EventArgs e)
        {
            Admin ad = new Admin();
            ad.Show();
            this.Hide();
        }
        
        /// <summary>
        /// Заполнение Роли и Фио сотрудника
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            string query1 = "SELECT EmployeeFIO FROM `employee`";
            string query2 = "SELECT Role FROM `role`";
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                MySqlCommand cmd1 = new MySqlCommand(query1, connection);
                using (MySqlDataReader reader1 = cmd1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        comboBox2.Items.Add(reader1["EmployeeFIO"].ToString());
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand(query2, connection);
                using (MySqlDataReader reader2 = cmd2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        comboBox1.Items.Add(reader2["Role"].ToString());
                    }
                }
                connection.Close();

            }
        }
        //Форма загруки
        private void Users_Load(object sender, EventArgs e)
        {
            FillDataGrid("SELECT UserID AS 'Идентификатор'," +
                "employee.EmployeeFIO AS 'ФИО сотрудника'," +
                "role.Role AS 'Роль'," +
                "Login AS 'Логин'," +
                "Password AS 'Пароль'" +
                "FROM user " +
                " INNER JOIN employee ON user.UserFIO = employee.EmployeeID" +
                " INNER JOIN role ON user.RoleID = role.RoleID");
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void FillDataGrid(string strCmd)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(strCmd, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.Columns.Add("UserID", "Идентификатор");
                dataGridView1.Columns["UserID"].Visible = false;
                dataGridView1.Columns.Add("UserFIO", "ФИО сотрудника");
                dataGridView1.Columns.Add("RoleID", "Роль");
                dataGridView1.Columns.Add("Login", "Логин");
                dataGridView1.Columns.Add("Password", "Пароль");
                dataGridView1.Columns["Password"].Visible = false;

                DataGridViewButtonColumn buttonV = new DataGridViewButtonColumn();
                buttonV.Name = "Выбрать";
                buttonV.HeaderText = "Выбрать";
                buttonV.Text = "Выбрать";
                buttonV.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonV);

                DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                buttonDel.Name = "Удалить";
                buttonDel.HeaderText = "Удалить";
                buttonDel.Text = "Удалить";
                buttonDel.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonDel);

                while (rdr.Read())
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["UserID"].Value = rdr[0];
                    row.Cells["UserFIO"].Value = rdr[1];
                    row.Cells["RoleID"].Value = rdr[2];
                    row.Cells["Login"].Value = rdr[3];
                    row.Cells["Password"].Value = rdr[4];
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }
        // Функция для проверки существования пользователя по логину
        private bool UserExists(string login, MySqlConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM User WHERE Login = @login";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@login", login);
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }
        // Функция для проверки существования пользователя по ФИО
        private bool UserFIOExists(int userFIO, MySqlConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM User WHERE UserFIO = @userFIO";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@userFIO", userFIO);
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }  
       
        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "" || textBox1.Text == "" || textBox7.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить эту запись?", "Подтверждение добавления!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            int roleid;
                            string roleidQuery = "SELECT RoleID FROM Role WHERE Role = @role";
                            using (MySqlCommand roleCmd = new MySqlCommand(roleidQuery, connection))
                            {
                                roleCmd.Parameters.AddWithValue("@role", comboBox1.Text);
                                object result = roleCmd.ExecuteScalar();
                                roleid = result != null ? Convert.ToInt32(result) : 0;
                                if (roleid == 0)
                                {
                                    MessageBox.Show("Роль не найдена.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            int userFIO;
                            string userFIOQuery = "SELECT EmployeeID FROM employee WHERE EmployeeFIO = @employeeFIO";
                            using (MySqlCommand userFIOCmd = new MySqlCommand(userFIOQuery, connection))
                            {
                                userFIOCmd.Parameters.AddWithValue("@employeeFIO", comboBox2.Text);
                                object result = userFIOCmd.ExecuteScalar();
                                userFIO = result != null ? Convert.ToInt32(result) : 0;
                                if (userFIO == 0)
                                {
                                    MessageBox.Show("ФИО сотрудника не найдено.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            if (UserFIOExists(userFIO, connection))
                            {
                                MessageBox.Show("Пользователь с таким ФИО уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (UserExists(textBox7.Text, connection))
                            {
                                MessageBox.Show("Пользователь с таким Логином уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            string query = "INSERT INTO User (UserFIO, Login, Password, RoleID) VALUES (@value1,@value2,@value3,@value4)";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                //UserID,cmd.Parameters.AddWithValue("@value0", textBox2.Text);
                                cmd.Parameters.AddWithValue("@value1", userFIO);
                                cmd.Parameters.AddWithValue("@value2", textBox7.Text);
                                if (textBox1.Text.Length < 10)
                                {
                                    string hashedPassword = HashPassword(textBox1.Text);
                                    cmd.Parameters.AddWithValue("@value3", hashedPassword);
                                }
                                else
                                {
                                    string hashedPassword = textBox1.Text;
                                    cmd.Parameters.AddWithValue("@value3", hashedPassword);
                                }
                                cmd.Parameters.AddWithValue("@value4", roleid);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                comboBox2.SelectedItem = null;
                                comboBox1.SelectedItem = null;
                                textBox7.Text = "";
                                textBox1.Text = "";
                                textBox2.Text = "";
                                FillDataGrid("SELECT UserID AS 'Идентификатор'," +
                                "employee.EmployeeFIO AS 'ФИО сотрудника'," +
                                "role.Role AS 'Роль'," +
                                "Login AS 'Логин'," +
                                "Password AS 'Пароль'" +
                                "FROM user " +
                                " INNER JOIN employee ON user.UserFIO = employee.EmployeeID" +
                                " INNER JOIN role ON user.RoleID = role.RoleID");
                            }
                            connection.Close();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка: " + ex);
                        }
                    }
                }
            }
        }
        //Работа кнопок с таблицы
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //При нажатии на кнопку "Выбрать"
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0) {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = row.Cells["UserID"].Value.ToString();
                comboBox2.Text = row.Cells["UserFIO"].Value.ToString();
                comboBox1.Text = row.Cells["RoleID"].Value.ToString();
                textBox7.Text = row.Cells["Login"].Value.ToString();
                textBox1.Text = row.Cells["Password"].Value.ToString();
            }
            // При нажатии кнопки "Удалить"
            if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["UserID"].Value);
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id); // Удаляем запись из базы данных
                    dataGridView1.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                }
            }
        }

        /// <summary>
        /// Для получения ID Сотрудника
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private int GetEmployeeIDByName(string userName, MySqlConnection con)
        {
            int userId = -1; 
            string query = "SELECT EmployeeID FROM Employee WHERE EmployeeFIO = @employeeFIO";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@employeeFIO", userName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }
                connection.Close();

            }
            return userId;
        }
        
        /// <summary>
        /// Для получения ID Роли
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private int GetRoleidByName(string roleName, MySqlConnection con)
        {
            int orderid = -1; 
            string query = "SELECT RoleID FROM Role WHERE Role = @role";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@role", roleName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        orderid = Convert.ToInt32(result);
                    }
                }
                connection.Close();

            }
            return orderid;
        }
        
        /// <summary>
        /// Метод удаления записи
        /// </summary>
        /// <param name="id"></param>
        private void DeleteRecord(int id)
        {
            string query = "DELETE FROM User WHERE UserID = @UserID";
            MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString);
            con.Open();
            MySqlCommand command = new MySqlCommand(query, con);
            command.Parameters.AddWithValue("@UserID", id);
            command.ExecuteNonQuery();
            con.Close();
        }
        
        /// <summary>
        /// Хеширование пароля
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Преобразуем пароль в байтовый массив и хешируем его
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Преобразуем байты в строку
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Форматируем байты в шестнадцатеричную строку
                }
                return builder.ToString(); // Возвращаем хеш как строку
            }
        }
        //Очистка всех полей
        private void button2_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedItem = null;
            comboBox1.SelectedItem = null;
            textBox7.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
        }
        //Ввод для Логина и пароля
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
             (e.KeyChar < 'A' || e.KeyChar > 'Z') &&
             (e.KeyChar < 'a' || e.KeyChar > 'z') &&
             (e.KeyChar < '0' || e.KeyChar > '9') &&
             (e.KeyChar != ' '))
            {
                e.Handled = true;
            }
        }
       
        /// <summary>
        /// Изменение товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "" || textBox1.Text == "" || textBox7.Text == "" )
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    int userID = Convert.ToInt32(textBox2.Text);
                    using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand(@"UPDATE User 
                        SET UserID = @userid,
                            UserFIO = @userFIO,
                            RoleID = @roleID,
                            Login = @login,
                            Password = @password
                        WHERE UserID = @userid", con);

                        int roleid;
                        string roleidQuery = "SELECT RoleID FROM Role WHERE Role = @role";
                        using (MySqlCommand roleCmd = new MySqlCommand(roleidQuery, con))
                        {
                            roleCmd.Parameters.AddWithValue("@role", comboBox1.Text);
                            object result = roleCmd.ExecuteScalar();
                            roleid = result != null ? Convert.ToInt32(result) : 0;
                            if (roleid == 0)
                            {
                                MessageBox.Show("Роль не найдена.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        int userFIO;
                        string userFIOQuery = "SELECT EmployeeID FROM employee WHERE EmployeeFIO = @employeeFIO";
                        using (MySqlCommand userFIOCmd = new MySqlCommand(userFIOQuery, con))
                        {
                            userFIOCmd.Parameters.AddWithValue("@employeeFIO", comboBox2.Text);
                            object result = userFIOCmd.ExecuteScalar();
                            userFIO = result != null ? Convert.ToInt32(result) : 0;
                            if (userFIO == 0)
                            {
                                MessageBox.Show("ФИО сотрудника не найдено.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        cmd.Parameters.AddWithValue("@userid", userID);
                        string selectedUSERFIO = comboBox2.Text.ToString();
                        int userFio = GetEmployeeIDByName(selectedUSERFIO, con);
                        cmd.Parameters.AddWithValue("@userFIO", userFio);
                        string selectedRoleid = comboBox1.Text.ToString();
                        int role = GetRoleidByName(selectedRoleid, con);
                        cmd.Parameters.AddWithValue("@roleID", role);
                        cmd.Parameters.AddWithValue("@login", textBox7.Text);
                        if (textBox1.Text.Length < 10)
                        {
                            string hashedPassword = HashPassword(textBox1.Text);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                        }
                        else
                        {
                            string hashedPassword = textBox1.Text;
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                        }
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        con.Close();
                    }
                    comboBox2.SelectedItem = null;
                    comboBox1.SelectedItem = null;
                    textBox7.Text = "";
                    textBox1.Text = "";
                    textBox2.Text = "";
                    FillDataGrid("SELECT UserID AS 'Идентификатор'," +
                    "employee.EmployeeFIO AS 'ФИО сотрудника'," +
                    "role.Role AS 'Роль'," +
                    "Login AS 'Логин'," +
                    "Password AS 'Пароль'" +
                    "FROM user " +
                    " INNER JOIN employee ON user.UserFIO = employee.EmployeeID" +
                    " INNER JOIN role ON user.RoleID = role.RoleID");
                }
            }
        }
        //Очистка Пароля
        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
