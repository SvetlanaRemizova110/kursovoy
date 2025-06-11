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
        private int inactivityTimeout = 0; // Время бездействия в миллисекундах.
        private int initialInactivityTimeout = 0; // Сохраняем начальное значение таймаута.
        private Timer Timer = new Timer(); //Обязательно инициализируйте Timer
        private bool isTimerTickRunning = false; // Флаг для блокировки повторного входа
        public Users()
        {
            InitializeComponent();
            LoadDataIntoComboBox();
            textBox2.Visible = false;
            Timer.Tick += inactivityTimer_Tick;
            Timer.Interval = 1000; // Проверка каждые 1 секунду
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
            // Блокируем повторный вход
            if (isTimerTickRunning)
            {
                return; // Если таймер уже работает, выходим
            }

            isTimerTickRunning = true; // Устанавливаем флаг, что таймер работает

            try
            {
                if (inactivityTimeout > 0)
                {
                    inactivityTimeout -= Timer.Interval; // Уменьшаем тайм-аут на интервал таймера
                }
                else
                {
                    Timer.Stop();
                    MessageBox.Show("Вы были перенаправлены на страницу авторизации из-за бездействия.", "Блокировка системы", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    import.AutomaticBackup();
                    this.Close();
                    Authorization authorization = new Authorization();
                    authorization.Show();
                }
            }
            finally
            {
                isTimerTickRunning = false; // Снимаем флаг в любом случае (даже если было исключение)
            }
        }

        //Кнопка НАЗАД
        private void button4_Click(object sender, EventArgs e)
        {
            Timer.Stop();
            Admin ad = new Admin();
            ad.Show();
            this.Close();
        }
        
        /// <summary>
        /// Заполнение Роли и Фио сотрудника
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            //string query1 = "SELECT EmployeeFIO FROM `employee`";
            string query2 = "SELECT Role FROM `role`";
            string query = " SELECT EmployeeF,EmployeeI,EmployeeO FROM employeeee";

            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (MySqlDataReader reader1 = cmd.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        comboBox2.Items.Add(reader1["EmployeeF"].ToString() + " " + reader1["EmployeeI"].ToString() + " " + reader1["EmployeeO"].ToString());
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

            initialInactivityTimeout = inactivityTimeout; // Сохраняем начальное значение
            ResetInactivityTimer(); // Сброс таймера активности
            Timer.Start(); // Запуск таймера активностиs
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
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AllowUserToDeleteRows = false;
                //dataGridView1.AllowUserToOrderColumns = false;
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
                    row.Cells["UserFIO"].Value = string.Format("{0} {1} {2}", rdr[1], rdr[2], rdr[3]);
                    row.Cells["RoleID"].Value = rdr[4];
                    row.Cells["Login"].Value = rdr[5];
                    row.Cells["Password"].Value = rdr[6];
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


        //Работа кнопок с таблицы
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //При нажатии на кнопку "Выбрать"
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
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

                // Проверяем, не пытаемся ли мы удалить текущего пользователя
                string currentUserFIO = Authorization.User2.FIO; // Получаем ФИО текущего пользователя
                string selectedUserFIO = (string)dataGridView1.Rows[e.RowIndex].Cells["UserFIO"].Value; // Получаем ФИО удаляемого пользователя

                if (selectedUserFIO == currentUserFIO)
                {
                    MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Прекращаем удаление
                }

                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    DeleteRecord(id); // Удаляем запись из базы данных
                    dataGridView1.Rows.RemoveAt(e.RowIndex); // Удаляем строку из DataGridView
                    MessageBox.Show("Запись успешно удалена.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string userFIOQuery = " SELECT EmployeeID FROM employeeee WHERE EmployeeF LIKE @employeeF AND EmployeeI LIKE @employeeI AND EmployeeO LIKE @employeeO;";
            //string query = "SELECT EmployeeID FROM Employee WHERE EmployeeFIO = @employeeFIO";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(userFIOQuery, connection))
                {
                    command.Parameters.AddWithValue("@employeeF", userName);
                    command.Parameters.AddWithValue("@employeeI", userName);
                    command.Parameters.AddWithValue("@employeeO", userName);
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
            string query = "DELETE FROM user WHERE UserID = @UserID";
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
    ((e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
     (e.KeyChar >= 'а' && e.KeyChar <= 'я') ||
     (e.KeyChar == 'Ё') || (e.KeyChar == 'ё')))
            {
                e.Handled = true;
            }


        }

        /// <summary>
        /// Добавление/Изменение товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || comboBox2.Text == "" || textBox1.Text == "" || textBox7.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                if (textBox2.Text != "")
                {
                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int userID = Convert.ToInt32(textBox2.Text);
                        using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            con.Open();
                            MySqlCommand cmd = new MySqlCommand(@"UPDATE user 
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
                            //string userFIOQuery = " SELECT EmployeeID FROM employeeee WHERE EmployeeF LIKE @userF AND EmployeeI LIKE @userI AND EmployeeO LIKE @userO;";
                            //string userFIOQuery = " SELECT EmployeeID FROM employeeee WHERE EmployeeF + EmployeeI + EmployeeO LIKE @userFIO;";
                            string userFIOQuery = "  SELECT EmployeeID FROM employeeee WHERE CONCAT(EmployeeF, EmployeeI, EmployeeO) = @userFIO; ";
                            string[] fioParts = comboBox2.Text.Split(' ');
                            if (fioParts.Length >= 3)
                            {
                                string employeeF = fioParts[0];
                                string employeeI = fioParts[1];
                                string employeeO = fioParts[2];

                                using (MySqlCommand userFIOCmd = new MySqlCommand(userFIOQuery, con))
                                {
                                    userFIOCmd.Parameters.AddWithValue("@userFIO", employeeF + employeeI + employeeO);
                                    //userFIOCmd.Parameters.AddWithValue("@userI", employeeI);
                                    //userFIOCmd.Parameters.AddWithValue("@userO", employeeO);
                                    object result = userFIOCmd.ExecuteScalar();
                                    userFIO = result != null ? Convert.ToInt32(result) : 0;
                                    //if (userFIO == 0)
                                    //{
                                    //    MessageBox.Show("ФИО сотрудника не найдено.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //    return;
                                    //}
                                }

                                cmd.Parameters.AddWithValue("@userid", userID);

                                string selectedUSERFIO = comboBox2.Text.ToString();
                                int userFio = GetEmployeeIDByName(selectedUSERFIO, con);
                                cmd.Parameters.AddWithValue("@userFIO", userFIO);
                                //cmd.Parameters.AddWithValue("@userI", userFIO);
                                //cmd.Parameters.AddWithValue("@userO", userFIO);

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
                }
                else
                {
                    textBox2.Text = "";
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
                                string userFIOQuery = "SELECT EmployeeID FROM employeeee WHERE EmployeeF LIKE @employeeF AND EmployeeI LIKE @employeeI AND EmployeeO LIKE @employeeO;";
                                string[] fioParts = comboBox2.Text.Split(' ');
                                if (fioParts.Length >= 3)
                                {
                                    string employeeF = fioParts[0];
                                    string employeeI = fioParts[1];
                                    string employeeO = fioParts[2];

                                    using (MySqlCommand userFIOCmd = new MySqlCommand(userFIOQuery, connection))
                                    {
                                        userFIOCmd.Parameters.AddWithValue("@employeeF", employeeF);
                                        userFIOCmd.Parameters.AddWithValue("@employeeI", employeeI);
                                        userFIOCmd.Parameters.AddWithValue("@employeeO", employeeO);
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
                                    string query = "INSERT INTO user (UserFIO, Login, Password, RoleID) VALUES (@value1,@value2,@value3,@value4)";
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
                                    connection.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка: " + ex);
                            }
                        }
                    }
                }
            }
        }
        //Очистка Пароля
        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        /// <summary>
        /// Сбрасывает отслеживание времени бездействия.
        /// </summary>
        private void ResetInactivityTimer()
        {
            inactivityTimeout = initialInactivityTimeout; //сбрасываем значение тайм-аута!
        }

        /// <summary>
        /// Запускает отслеживание активности при загрузке окна.
        /// </summary>
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
