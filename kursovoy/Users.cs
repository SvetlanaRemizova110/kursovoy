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
using System.Security.Cryptography;
using System.Configuration;

namespace kursovoy
{
    public partial class Users : Form
    {
        private int inactivityTimeout = 0;
        private int initialInactivityTimeout = 0;
        private Timer Timer = new Timer();
        private bool isTimerTickRunning = false;
        public Users()
        {
            InitializeComponent();
            LoadDataIntoComboBox();
            textBox2.Visible = false;
            Timer.Tick += inactivityTimer_Tick;
            Timer.Interval = 1000;
        }
        private void Users_ActivateTracking()
        {
            this.MouseMove += Users_ActivityDetected;
            this.KeyPress += Users_ActivityDetected;
            this.MouseClick += Users_ActivityDetected;

            foreach (Control control in this.Controls)
            {
                control.MouseMove += Users_ActivityDetected;
                control.MouseClick += Users_ActivityDetected;
            }
        }
        private void Users_ActivityDetected(object sender, EventArgs e)
        {
            ResetInactivityTimer();
        }
        private void inactivityTimer_Tick(object sender, EventArgs e)
        {
            if (isTimerTickRunning)
            {
                return;
            }
            isTimerTickRunning = true;
            try
            {
                if (inactivityTimeout > 0)
                {
                    inactivityTimeout -= Timer.Interval;
                }
                else
                {
                    Timer.Stop();
                    MessageBox.Show("Вы были перенаправлены на страницу авторизации из-за бездействия.", "Блокировка системы", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    import.AutomaticBackup();
                    this.Close();
                    Authorization authorization = new Authorization();
                    authorization.Show();
                }
            }
            finally
            {
                isTimerTickRunning = false;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Timer.Stop();
            Admin ad = new Admin();
            ad.Show();
            this.Close();
        }
        private void LoadDataIntoComboBox()
        {
            string queryEmployee = "SELECT EmployeeF,EmployeeI,EmployeeO FROM employeeee";
            string queryRole = "SELECT Role FROM `role`";

            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(queryEmployee, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox2.Items.Add($"{reader["EmployeeF"]} {reader["EmployeeI"]} {reader["EmployeeO"]}");
                        }
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand(queryRole, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader["Role"].ToString());
                        }
                    }
                }
            }
        }
        private void Users_Load(object sender, EventArgs e)
        {
            if (int.TryParse(ConfigurationManager.AppSettings["InactivityTimeout"], out int timeoutInSeconds))
            {
                inactivityTimeout = timeoutInSeconds * 1000;
            }
            else
            {
                inactivityTimeout = 30000;
            }

            initialInactivityTimeout = inactivityTimeout;
            ResetInactivityTimer();
            Timer.Start();
            FillDataGrid("SELECT UserID AS 'Идентификатор'," +
                "employeeee.EmployeeF AS 'Ф сотрудника'," +
                "employeeee.EmployeeI AS 'И сотрудника'," +
                "employeeee.EmployeeO AS 'О сотрудника'," +
                "role.Role AS 'Роль'," +
                "Login AS 'Логин'," +
                "Password AS 'Пароль'" +
                "FROM user " +
                " INNER JOIN employeeee ON user.UserFIO = employeeee.EmployeeID" +
                " INNER JOIN role ON user.RoleID = role.RoleID");
            label2.Text += " " + dataGridView1.Rows.Count;
            role.Text = Authorization.User2.RoleName + ": " + Authorization.User2.FIO;
        }

        public void FillDataGrid(string strCmd)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(strCmd, conn))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Columns.Clear();
                            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                            dataGridView1.AllowUserToDeleteRows = false;
                            dataGridView1.AllowUserToResizeColumns = false;
                            dataGridView1.AllowUserToResizeRows = false;
                            dataGridView1.AllowUserToAddRows = false;
                            dataGridView1.ReadOnly = true;

                            dataGridView1.Columns.Add("UserID", "Идентификатор");
                            dataGridView1.Columns["UserID"].Visible = false;
                            dataGridView1.Columns.Add("UserFIO", "ФИО сотрудника");
                            dataGridView1.Columns["UserFIO"].Width = 270;
                            dataGridView1.Columns.Add("RoleID", "Роль");
                            dataGridView1.Columns["RoleID"].Width = 110;
                            dataGridView1.Columns.Add("Login", "Логин");
                            dataGridView1.Columns["Login"].Width = 100;
                            dataGridView1.Columns.Add("Password", "Пароль");
                            dataGridView1.Columns["Password"].Visible = false;
                            dataGridView1.Columns["UserFIO"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                            DataGridViewButtonColumn buttonV = new DataGridViewButtonColumn();
                            buttonV.Name = "Выбрать";
                            buttonV.HeaderText = "Выбрать";
                            buttonV.Text = "Выбрать";
                            buttonV.UseColumnTextForButtonValue = true;
                            dataGridView1.Columns.Add(buttonV);
                            dataGridView1.Columns["Выбрать"].Width = 75;

                            DataGridViewButtonColumn buttonDel = new DataGridViewButtonColumn();
                            buttonDel.Name = "Удалить";
                            buttonDel.HeaderText = "Удалить";
                            buttonDel.Text = "Удалить";
                            buttonDel.UseColumnTextForButtonValue = true;
                            dataGridView1.Columns.Add(buttonDel);
                            dataGridView1.Columns["Удалить"].Width = 75;

                            while (rdr.Read())
                            {
                                int rowIndex = dataGridView1.Rows.Add();
                                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                                row.Cells["UserID"].Value = rdr[0];
                                row.Cells["UserFIO"].Value = $"{rdr[1]} {rdr[2]} {rdr[3]}";
                                row.Cells["RoleID"].Value = rdr[4];
                                row.Cells["Login"].Value = rdr[5];
                                row.Cells["Password"].Value = rdr[6];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при заполнении DataGridView: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool UserExists(string login, MySqlConnection connection, int? userIdToExclude = null)
        {
            string checkQuery = "SELECT COUNT(*) FROM User WHERE Login = @login";
            if (userIdToExclude.HasValue)
            {
                checkQuery += " AND UserID != @userId";
            }

            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@login", login);
                if (userIdToExclude.HasValue)
                {
                    checkCmd.Parameters.AddWithValue("@userId", userIdToExclude.Value);
                }
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = row.Cells["UserID"].Value.ToString();
                comboBox2.Text = row.Cells["UserFIO"].Value.ToString();
                comboBox1.Text = row.Cells["RoleID"].Value.ToString();
                textBox7.Text = row.Cells["Login"].Value.ToString();
                textBox1.Text = row.Cells["Password"].Value.ToString();
            }

            if (e.ColumnIndex == dataGridView1.Columns["Удалить"].Index && e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["UserID"].Value);
                string currentUserFIO = Authorization.User2.FIO;
                string selectedUserFIO = (string)dataGridView1.Rows[e.RowIndex].Cells["UserFIO"].Value;

                if (selectedUserFIO == currentUserFIO)
                {
                    MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id);
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    MessageBox.Show("Запись удалена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private int GetEmployeeIDByName(string userName, MySqlConnection con)
        {

            string userFIOQuery = " SELECT EmployeeID FROM employeeee WHERE CONCAT(EmployeeF, ' ', EmployeeI, ' ', EmployeeO) = @userName;";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(userFIOQuery, connection))
                {
                    command.Parameters.AddWithValue("@userName", userName);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        private int GetRoleidByName(string roleName, MySqlConnection con)
        {
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
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        private void DeleteRecord(int id)
        {
            string query = "DELETE FROM user WHERE UserID = @UserID";
            using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                con.Open();
                using (MySqlCommand command = new MySqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@UserID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedItem = null;
            comboBox1.SelectedItem = null;
            textBox7.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                ((e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
                 (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
                 (e.KeyChar == 'Ё') || (e.KeyChar == 'ё')))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox7.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                int employeeId = GetEmployeeIDByName(comboBox2.Text, connection);
                int roleId = GetRoleidByName(comboBox1.Text, connection);

                if (employeeId == -1)
                {
                    MessageBox.Show("Не удалось найти EmployeeID для указанного ФИО.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (roleId == -1)
                {
                    MessageBox.Show("Не удалось найти RoleID для указанной роли.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (textBox2.Text != "")
                {
                    int userId = Convert.ToInt32(textBox2.Text);

                    // Проверка на существование логина, исключая текущего пользователя
                    if (UserExists(textBox7.Text, connection, userId))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string hashedPassword = textBox1.Text.Length < 10 ? HashPassword(textBox1.Text) : textBox1.Text;

                        string query = @"UPDATE user SET UserFIO = @userFIO, RoleID = @roleID, Login = @login, Password = @password WHERE UserID = @userID";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@userID", userId);
                            cmd.Parameters.AddWithValue("@userFIO", employeeId);
                            cmd.Parameters.AddWithValue("@roleID", roleId);
                            cmd.Parameters.AddWithValue("@login", textBox7.Text);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    // Проверка на существование логина
                    if (UserExists(textBox7.Text, connection))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить эту запись?", "Подтверждение добавления!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string hashedPassword = textBox1.Text.Length < 10 ? HashPassword(textBox1.Text) : textBox1.Text;

                        string query = "INSERT INTO user (UserFIO, Login, Password, RoleID) VALUES (@userFIO, @login, @password, @roleID)";
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@userFIO", employeeId);
                            cmd.Parameters.AddWithValue("@login", textBox7.Text);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                            cmd.Parameters.AddWithValue("@roleID", roleId);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Запись добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                connection.Close(); // Ensure connection is closed after operations.

                // Clear fields and refresh the data grid
                comboBox2.SelectedItem = null;
                comboBox1.SelectedItem = null;
                textBox7.Text = "";
                textBox1.Text = "";
                textBox2.Text = "";
                FillDataGrid("SELECT UserID AS 'Идентификатор'," +
                    "employeeee.EmployeeF AS 'Ф сотрудника'," +
                    "employeeee.EmployeeI AS 'И сотрудника'," +
                    "employeeee.EmployeeO AS 'О сотрудника'," +
                    "role.Role AS 'Роль'," +
                    "Login AS 'Логин'," +
                    "Password AS 'Пароль'" +
                    "FROM user " +
                    " INNER JOIN employeeee ON user.UserFIO = employeeee.EmployeeID" +
                    " INNER JOIN role ON user.RoleID = role.RoleID");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void ResetInactivityTimer()
        {
            inactivityTimeout = initialInactivityTimeout;
        }

        private void Users_Shown(object sender, EventArgs e)
        {
            Users_ActivateTracking();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}
