﻿using System;
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
using System.Threading;
using System.Drawing.Drawing2D;

namespace kursovoy
{
    public partial class Authorization : Form
    {
        //Общее подключение к бд
        //public static string ConnectionString { get; } = $"host={ConfigurationManager.AppSettings["host"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["password"]};database={ConfigurationManager.AppSettings["db"]};";
        //public static string ConnectionStringNotDB { get; } = $"host={ConfigurationManager.AppSettings["host"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["password"]};";


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

                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
                    stringFormat.LineAlignment = StringAlignment.Center;

                    // Рисуем каждый символ
                    for (int i = 0; i < captchaText.Length; i++)
                    {
                        int cellWidth = symbolArea.Width / captchaText.Length;

                        // Генерируем случайные координаты
                        int x = symbolArea.X + i * cellWidth + random.Next(cellWidth / 4);
                        int y = symbolArea.Y + random.Next(0, symbolArea.Height / 2);

                        int rotationAngle = random.Next(-15, 15);

                        // Создаем матрицу преобразования
                        Matrix matrix = new Matrix();
                        matrix.RotateAt(rotationAngle, new PointF(x + (float)cellWidth / 2, y + (float)symbolArea.Height / 2));
                        g.Transform = matrix;

                        // Рисуем символ
                        g.DrawString(captchaText[i].ToString(), font, Brushes.Black, new PointF(x, y));

                        // Сбрасываем преобразование
                        g.ResetTransform();
                    }

                    // Рисуем линии
                    for (int i = 0; i < 4; i++)
                    {
                        int startX = random.Next(0, width / 4);
                        int startY = random.Next(0, height);
                        int endX = random.Next(width / 4 * 3, width);
                        int endY = random.Next(0, height);
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
        private bool IsDatabaseExists()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1049) // Unknown database
                {
                    return false;
                }
                throw;
            }
        }
        private bool IsServerAvailable()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionStringNotDB))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void authorization_Click(object sender, EventArgs e)
        {
            string enteredLogin = textBoxLogin.Text;
            string enteredPassword = textBoxPwd.Text;

            // Проверяем подключение к серверу
            if (!IsServerAvailable())
            {
                using (var connSettingsForm = new ConnectionSettingsForm())
                {
                    if (connSettingsForm.ShowDialog() == DialogResult.OK)
                    {
                        if (!IsServerAvailable())
                        {
                            MessageBox.Show("Не удалось подключиться к серверу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        return; // Пользователь нажал "Назад"
                    }
                }
            }

            // После успешного подключения к серверу — проверяем наличие БД
            if (!IsDatabaseExists())
            {
                // Проверяем, является ли пользователь администратором по конфигурации
                if (ConfigurationManager.AppSettings["DefaultUser"] == enteredLogin &&
                    ConfigurationManager.AppSettings["DefaultPassword"] == enteredPassword)
                {
                    // Разрешаем вход под admin/admin при отсутствии базы данных
                    import admin = new import();
                    this.Hide();
                    admin.Show();
                    return;
                }
                else
                {
                    // Не admin/admin → нельзя работать без БД
                    MessageBox.Show("База данных не найдена. Только admin может войти для её восстановления.",
                                    "Ошибка",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
            }

            string login = textBoxLogin.Text;
            string password = textBoxPwd.Text;
            string defaultUser = null;
            string defaultPassword = null;

            try
            {
                defaultUser = ConfigurationManager.AppSettings["DefaultUser"];
                defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Ошибка чтения конфигурации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (captchaPictureBox.Visible == false || (captchaPictureBox.Visible == true && captchaTextBox.Text == captchaText))
            {
                User authorizedUser = null;
                try
                {
                    authorizedUser = AuthorizeUser(login, password);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (textBoxLogin.Text == defaultUser && textBoxPwd.Text == defaultPassword)
                {
                    import admin = new import();
                    this.Hide();
                    admin.Show();
                }
                else if (authorizedUser != null)
                {
                    try
                    {
                        SwitchRole(authorizedUser.Role);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка переключения роли: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    failedLoginAttempts++;
                    //Проверка кол-ва неверных попыток ввода логина/пароля
                    if (failedLoginAttempts >= 1)
                    {
                        //Делаем видимыми элементы CAPTCHA
                        try
                        {
                            LoadCaptcha();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка загрузки CAPTCHA: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        captchaPictureBox.Visible = true;
                        button3.Visible = true;
                        label4.Visible = true;
                        captchaTextBox.Visible = true;
                        if (failedLoginAttempts >= 2)
                        {
                            //Блокируем кнопку для входа
                            button1.Enabled = false;
                            MessageBox.Show("Блокировка 10 сек", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Thread.Sleep(10000);
                            button1.Enabled = true;
                            MessageBox.Show("Система разблокирована!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверная CAPTCHA!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (failedLoginAttempts >= 1)
                {
                    button1.Enabled = false;
                    MessageBox.Show("Блокировка 10 сек", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Thread.Sleep(10000);
                    button1.Enabled = true;
                    MessageBox.Show("Система разблокирована!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите выйти из приложения?", "Подтверждение на выход", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                using (MySqlConnection con = new MySqlConnection(Program1.ConnectionString))
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
                            Role = Convert.ToInt32(row["RoleID"]),
                            UserID = Convert.ToInt32(row["UserID"]),
                            EmployeeID = Convert.ToInt32(row["UserFIO"]),
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
                if (ex.Number == 1049)
                {
                    string defaultUser = ConfigurationManager.AppSettings["DefaultUser"];
                    string defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];
                    if (textBoxLogin.Text == defaultUser && textBoxPwd.Text == defaultPassword)
                    {
                        user = new User { };
                    }
                    else
                    {
                        MessageBox.Show("База данных не существует. Сначало восстановите базу данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                user = null;
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
                return builder.ToString();
            }
        }
        /// <summary>
        /// Метод для входа в учетную запись в зависимости от роли
        /// </summary>
        /// <param name="user"></param>
        private void SwitchRole(int role)
        {
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
            Program1.ReloadConnectionStrings();
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
            if ((e.KeyChar >= 'А' && e.KeyChar <= 'Я') ||
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
    public partial class ConnectionSettingsForm : Form
    {
        public ConnectionSettingsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Настройка подключения к серверу";
            this.Size = new Size(450, 250);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ControlBox = false;
            this.BackColor = Color.FromArgb(255, 224, 192);

            Font font = new Font("Comic Sans MS", 15.75f);
            this.Font = font;

            Label labelServer = new Label() { Text = "Сервер:", Location = new Point(30, 30), AutoSize = true, BackColor = Color.FromArgb(255, 192, 128) };
            Label labelUid = new Label() { Text = "Пользователь:", Location = new Point(30, 80), AutoSize = true, BackColor = Color.FromArgb(255, 192, 128) };
            Label labelPwd = new Label() { Text = "Пароль:", Location = new Point(30, 130), AutoSize = true, BackColor = Color.FromArgb(255, 192, 128) };

            TextBox txtServer = new TextBox() { Text = ConfigurationManager.AppSettings["host"], Location = new Point(200, 30), Width = 200 };
            TextBox txtUid = new TextBox() { Text = ConfigurationManager.AppSettings["uid"], Location = new Point(200, 80), Width = 200 };
            TextBox txtPwd = new TextBox()
            {
                Text = ConfigurationManager.AppSettings["password"],
                Location = new Point(200, 130),
                Width = 200,
                PasswordChar = '*'
            };

            Button btnSave = new Button() { Text = "Применить", Location = new Point(90, 180), Width = 150, Height = 50, BackColor = Color.FromArgb(255, 192, 128) };
            Button btnBack = new Button() { Text = "Назад", Location = new Point(250, 180), Width = 100, Height = 50, BackColor = Color.FromArgb(255, 192, 128) };

            btnSave.Click += (sender, e) =>
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["host"].Value = txtServer.Text;
                config.AppSettings.Settings["uid"].Value = txtUid.Text;
                config.AppSettings.Settings["password"].Value = txtPwd.Text;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                Program1.ReloadConnectionStrings();

                DialogResult = DialogResult.OK;
                this.Close();
            };


            // Добавляем элементы на форму
            Controls.Add(labelServer); Controls.Add(txtServer);
            Controls.Add(labelUid); Controls.Add(txtUid);
            Controls.Add(labelPwd); Controls.Add(txtPwd);
            // Controls.Add(labelDatabase); Controls.Add(txtDatabase);
            Controls.Add(btnSave);
            Controls.Add(btnBack);
        }
    }
    public static class Program1
    {
        public static string ConnectionString { get; set; }
        public static string ConnectionStringNotDB { get; set; }

        public static void ReloadConnectionStrings()
        {
            ConnectionString =
                $"host={ConfigurationManager.AppSettings["host"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["password"]};database={ConfigurationManager.AppSettings["db"]};";

            ConnectionStringNotDB =
                $"host={ConfigurationManager.AppSettings["host"]};uid={ConfigurationManager.AppSettings["uid"]};pwd={ConfigurationManager.AppSettings["password"]};";
        }
    }
}
