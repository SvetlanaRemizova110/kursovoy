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
using System.Drawing.Drawing2D;
using System.Threading;
namespace kursovoy
{
    public partial class Authorization : Form
    {
        public static class Program
        {
            //Общее подключение к бд
            public static string ConnectionString { get; } = $"host={ConfigurationManager.AppSettings["host"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["password"]};database={ConfigurationManager.AppSettings["db"]};";
            public static string ConnectionStringNotDB { get; } = $"host={ConfigurationManager.AppSettings["host"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["password"]};";
            //public static string ConnectionString { get; } = "host=10.207.106.12;uid=user45;pwd=lj45;database=db45;";
            //public static string ConnectionStringNotDB { get; } = "host=10.207.106.12;uid=user45;pwd=lj45;";
        }
        public class CaptchaGenerator
        {
            private static Random random = new Random();

            public static Bitmap GenerateCaptcha(out string captchaText)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                captchaText = new string(Enumerable.Range(0, 4).Select(x => chars[random.Next(chars.Length)]).ToArray());
                int width = 199;
                int height = 69;
                Bitmap bmp = new Bitmap(width, height);

                // Определяем область для символов (оставляем отступы)
                Rectangle symbolArea = new Rectangle(10, 10, width - 20, height - 20);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    Font font = new Font("Arial", 22, FontStyle.Bold);

                    // stringFormat для выравнивания ВСЕЙ строки в области (не отдельных символов!)
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Рисуем каждый символ, ограничивая его областью
                    for (int i = 0; i < captchaText.Length; i++)
                    {
                        // Вычисляем ширину ячейки для символа
                        int cellWidth = symbolArea.Width / captchaText.Length;

                        // Генерируем случайные координаты внутри ячейки
                        int x = symbolArea.X + i * cellWidth + random.Next(cellWidth / 4);
                        int y = symbolArea.Y + random.Next(0, symbolArea.Height / 2); //Поднимаем символы повыше

                        // Генерируем случайный угол поворота
                        int rotationAngle = random.Next(-15, 15); // Уменьшил диапазон вращения

                        // Создаем матрицу преобразования
                        Matrix matrix = new Matrix();
                        matrix.RotateAt(rotationAngle, new PointF(x + (float)cellWidth / 2, y + (float)symbolArea.Height / 2));
                        g.Transform = matrix;

                        // Рисуем символ
                        g.DrawString(captchaText[i].ToString(), font, Brushes.Black, new PointF(x, y));

                        // Сбрасываем преобразование
                        g.ResetTransform();
                    }

                    // Рисуем линии искажения
                    for (int i = 0; i < 4; i++)
                    {
                        int startX = random.Next(0, width / 4);
                        int startY = random.Next(0, height); // Расширил диапазон
                        int endX = random.Next(width / 4 * 3, width);
                        int endY = random.Next(0, height); // Расширил диапазон
                        g.DrawLine(new Pen(Color.Black, 2), startX, startY, endX, endY);
                    }
                }
                return bmp;
            }
        }
        private string captchaText;
        private int failedLoginAttempts = 0;
        public Authorization()
        {
            InitializeComponent();
            LoadCaptcha();
        }
        /// <summary>
        /// Функция загрузки капчи
        /// </summary>
        private void LoadCaptcha()
        {
            captchaPictureBox.Image = CaptchaGenerator.GenerateCaptcha(out captchaText);
            captchaPictureBox.Visible = false;
            button3.Visible = false;
            label4.Visible = false;
            captchaTextBox.Visible = false;
        }
        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void authorization_Click(object sender, EventArgs e)
        {
            //Получаем логин и пароль из текстовых полей на форме
            string login = textBoxLogin.Text;
            string password = textBoxPwd.Text;
            string defaultUser = ConfigurationManager.AppSettings["DefaultUser"];
            string defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];
            //Проверка на заполненность полей ввода логина и пароля, в случае отсутствия выводим сообщение
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Логика проверки капчи, если капча не видна или видна и введена правильно запускаем авторизацию
            if (captchaPictureBox.Visible == false || (captchaPictureBox.Visible == true && captchaTextBox.Text == captchaText))
            {
                //Ищем пользователей по логину и паролю в БД
                User authorizedUser = AuthorizeUser(login, password);
                //Если пользователь найден, то проверяем логин и пароль администратора
                if (textBoxLogin.Text == defaultUser && textBoxPwd.Text == defaultPassword)
                {
                    //Если введены логин и пароль администратора, то открываем форму admin
                    import admin = new import();
                    this.Hide();
                    admin.Show();
                }
                else if (authorizedUser != null)
                {
                    //Если найден пользователь, то устанавливаем свойства его роли и переключаем
                    SwitchRole(authorizedUser.Role);
                }
                else
                {
                    //Если пользователь с указанным логином и паролем не найден выводим сообщение об ошибке
                    MessageBox.Show("Неверный логин или пароль", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    failedLoginAttempts++;
                    //Проверка кол-ва неверных попыток ввода логина/пароля, если кол-во попыток больше или равно допустимому кол-ву - выводим панель CAPTCHA
                    if (failedLoginAttempts >= 1)
                    {
                        //Делаем видимыми элементы CAPTCHA
                        LoadCaptcha();
                        captchaPictureBox.Visible = true;
                        button3.Visible = true;
                        label4.Visible = true;
                        captchaTextBox.Visible = true;
                        if (failedLoginAttempts >= 2)
                        {
                            //Блокируем кнопку для входа 
                            button1.Enabled = false;
                            MessageBox.Show("Блокировка 10 сек", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Thread.Sleep(10000);
                            button1.Enabled = true;
                            MessageBox.Show("Система разблокирована!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверная CAPTCHA!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (failedLoginAttempts >= 1)
                {
                    //Блокируем кнопку для входа 
                    button1.Enabled = false;
                    MessageBox.Show("Блокировка 10 сек", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Thread.Sleep(10000);
                    button1.Enabled = true;
                    MessageBox.Show("Система разблокирована!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        /// <summary>
        /// Класс User - получение информации о пользователе
        /// </summary>
        public class User
        {
            public int Role { get; set; }
            public int UserID { get; set; }
            public int EmployeeID { get; set; }
            public string FIO { get; set; }
            public string RoleName { get; set; }
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
            public static string RoleName { get; set; }
        }
        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите выйти из приложения?", "Подтверждение на выход", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                import.AutomaticBackup();
                Application.Exit();
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
                    MySqlCommand cmd = new MySqlCommand($"SELECT u.*, e.EmployeeF, e.EmployeeI, e.EmployeeO, r.Role" +
                    " FROM user u " +
                    " INNER JOIN employeeee e ON u.UserFIO = e.EmployeeID " +
                    " INNER JOIN role r ON u.RoleID = r.RoleID " +
                    $" WHERE u.Login = '{login}' AND u.Password = '{hashedPassword}';", con);
                    MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                    DataTable tb = new DataTable();
                    ad.Fill(tb);
                    // Получаем данные о пользователе
                    if (tb.Rows.Count == 1)
                    {
                        DataRow row = tb.Rows[0];
                        user = new User
                        {
                            Role = Convert.ToInt32(row["RoleID"]),// Получаем Роль, используем имя столбца
                            UserID = Convert.ToInt32(row["UserID"]),
                            EmployeeID = Convert.ToInt32(row["UserFIO"]), // Используем внешний ключ UserFIO
                            RoleName = row["Role"].ToString(),
                            FIO = row["EmployeeF"].ToString() + " " +
                                  row["EmployeeI"].ToString() + " " + row["EmployeeO"].ToString()
                        };
                    }
                    con.Close();
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1049) // Error 1049 - Unknown database
                {
                    string defaultUser = ConfigurationManager.AppSettings["DefaultUser"];
                    string defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];
                    if (textBoxLogin.Text == defaultUser && textBoxPwd.Text == defaultPassword)
                    {

                        user = new User { };
                    }
                    else
                    {
                        MessageBox.Show("База данных не существует. Создайте базу данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        user = new User { };
                    }
                }
                else
                {
                    user = null;
                }
                return user;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при авторизации: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                user = null; // Возвращаем null, если произошла ошибка
            }
            if (user != null)
            {
                User2.EmployeeID = user.EmployeeID;
                User2.FIO = user.FIO;
                User2.Role = user.Role;
                User2.RoleName = user.RoleName;
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
        private void SwitchRole(int role)
        {
            // Определите форму для отображения в зависимости от роли
            switch (role)
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
                    //MessageBox.Show("Ваша роль в системе не определена. Обратитесь к администратору.");
                    break;
            }
        }
        /// <summary>
        /// Проверка на пустое поле для ввода пароля при загрузке формы
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
        /// <summary>
        /// При любом изменении пароля кнопка для входа становится активной.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPwd_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
        /// <summary>
        /// Кнопка для обновления капчи.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateCaptcha_Click(object sender, EventArgs e)
        {
            LoadCaptcha();
            captchaPictureBox.Visible = true;
            button3.Visible = true;
            label4.Visible = true;
            captchaTextBox.Visible = true;
        }
        /// <summary>
        /// Обработчик нажатия клавиш, ввод только русских букв.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((e.KeyChar >= 'А' && e.KeyChar <= 'Я') || 
                (e.KeyChar >= 'а' && e.KeyChar <= 'я'))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}

