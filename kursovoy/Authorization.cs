using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.IO;
namespace kursovoy
{
    public partial class Authorization : Form
    {
        public static class Program
        {
            //Общее подключение к бд
            //public static string ConnectionString { get; } = "host=localhost;uid=root;pwd=;database=pets_shop";
            public static string ConnectionString { get; } = "host=10.207.106.12;uid=user45;pwd=lj45;database=db45";
        }
        public Authorization()
        {
            InitializeComponent();
        }
        //
        /// <summary>
        /// Класс User - получение информации о пользователе
        /// </summary>
        public class User
        {
            public int Role { get; set; }
            public int UserID { get; set; }
            public int EmployeeID { get; set; }
            public string FIO { get; set; }
        }

        /// <summary>
        /// Класс User2 - получения информации о пользователе и использование его для разграничения прав доступа пользователей
        /// </summary>
        public static class User2
        {
            public static int Role { get; set; }
            public static int UserID { get; set; }
            public static int EmployeeID { get; set; }
            public static string FIO { get; set; }
        }

        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите выйти из приложения?", "Подтверждение на выход", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPwd.Text;
            string defaultUser = ConfigurationManager.AppSettings["DefaultUser"];
            string defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];

            User user = AuthorizeUser(login, password);
            if (login == defaultUser && password == defaultPassword)
            {
                // Успешная авторизация
                import i = new import();
                i.Show();
                this.Hide();
            }
            else if (user != null)
            {
                switchRole(user);
            }
            else
            {
                MessageBox.Show("Неверные данные!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Метод для авторизации пользователя по логину и паролю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        private User AuthorizeUser(string login, string password)
        {
            User user = null;
            try
            {
                // Хешируем введённый пароль
                string hashedPassword = HashPassword(password);

                using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT User.*, e.EmployeeFIO " +
                        "FROM User  " +
                        "INNER JOIN Employee e ON User.UserFIO = e.EmployeeID " +
                        "WHERE User.Login = @login AND User.Password = @password", con);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", hashedPassword); // Используем хешированный пароль
                    MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                    DataTable tb = new DataTable();
                    ad.Fill(tb);

                    // Получаем данные о пользователе
                    if (tb.Rows.Count == 1)
                    {
                        DataRow row = tb.Rows[0];
                        user = new User
                        {
                            Role = Convert.ToInt32(tb.Rows[0].ItemArray.GetValue(2)),
                            UserID = Convert.ToInt32(row.ItemArray.GetValue(0)),
                            EmployeeID = Convert.ToInt32(row.ItemArray.GetValue(1))
                        };
                        User2.UserID = Convert.ToInt32(row.ItemArray.GetValue(0));
                        User2.Role = Convert.ToInt32(tb.Rows[0].ItemArray.GetValue(2));
                        User2.EmployeeID = Convert.ToInt32(row.ItemArray.GetValue(1));
                        User2.FIO = row["EmployeeFIO"].ToString();
                    }
                    con.Close();
                }
            }
            catch (MySqlException ex) 
            {
                MessageBox.Show("Ошибка базы данных: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неизвестная ошибка: " + ex.Message);
            }
            return user;
        }

        /// <summary>
        /// Метод для хеширования пароля
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

        /// <summary>
        /// Метод для входа в учетную запись в зависимости от роли
        /// </summary>
        /// <param name="user"></param>
        private void switchRole(User user)
        {
            switch (user.Role)
            {
                case 1:
                    User2.Role = 1;
                    Admin ad = new Admin();
                    ad.Show();
                    this.Hide();
                    break;
                case 2:
                    User2.Role = 2;
                    СommoditySpecialist CS = new СommoditySpecialist();
                    CS.Show();
                    this.Hide();
                    break;
                case 3:
                    User2.Role = 3;
                    Seller sl = new Seller();
                    sl.Show();
                    this.Hide();
                    break;
                default:
                    MessageBox.Show("Неизвестная роль", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        /// <summary>
        /// Проверка на пустое поле для ввода пароля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Authorization_Load(object sender, EventArgs e)
        {
            if (textBoxPwd.Text == "")
            {
                button1.Enabled = false;
            }
        }
        private void textBoxPwd_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}

