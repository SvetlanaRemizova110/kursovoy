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
using System.Globalization;
using System.Configuration;
namespace kursovoy
{
    public partial class Staffs : Form
    {
        private int inactivityTimeout = 0; // Время бездействия в миллисекундах.
        private int initialInactivityTimeout = 0; // Сохраняем начальное значение таймаута.
        private Timer Timer = new Timer(); //Обязательно инициализируйте Timer
        private bool isTimerTickRunning = false; // Флаг для блокировки повторного входа
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();
        public Staffs()
        {
            InitializeComponent();
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
                    MessageBox.Show("Вы были перенаправлены на страницу авторизации из-за бездействия.", "Блокировка системы", MessageBoxButtons.OK,MessageBoxIcon.Information);
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
        private void button4_Click(object sender, EventArgs e)
        {
            Timer.Stop();
            Admin ad = new Admin();
            ad.Show();
            this.Close();
        }

        private void Staffs_Load(object sender, EventArgs e)
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

            FillDataGrid("SELECT EmployeeID AS 'id', " +
               "EmployeeF AS 'Фамилия'," +
               "EmployeeI AS 'Имя'," +
               "EmployeeO AS 'Отчетство'," +
               "concat(left(telephone, 7), ' * **', right(telephone, 5)) AS 'Номер телефона',"+
               "status AS 'Статус'"+
               " FROM  employeeee; ");
            label2.Text += " " + dataGridView1.Rows.Count;
            maskedTextBox1.Mask = "+7(000)000-00-00";

            role.Text = Authorization.User2.RoleName + ": " + Authorization.User2.FIO;

            comboBoxStatus.Text = "работает";
        }
        public void FillDataGrid(string strCmd)
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
                
                allRows1.Clear();
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;
                // Отключение возможности перемещения строк
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.Columns.Add("EmployeeID", "id");
                dataGridView1.Columns["EmployeeID"].Visible = false;
                dataGridView1.Columns.Add("EmployeeF", "Фамилия");
                dataGridView1.Columns["EmployeeF"].Width = 180;
                dataGridView1.Columns.Add("EmployeeI", "Имя");
                dataGridView1.Columns["EmployeeI"].Width = 180;
                dataGridView1.Columns.Add("EmployeeO", "Отчетство");
                dataGridView1.Columns["EmployeeO"].Width = 100;
                dataGridView1.Columns.Add("telephone", "Номер телефона");
                dataGridView1.Columns["telephone"].Width = 130;
                dataGridView1.Columns.Add("status", "Статус");
                dataGridView1.Columns["status"].Width = 80;

                
                DataGridViewButtonColumn buttonEdit = new DataGridViewButtonColumn();
                buttonEdit.Name = "Выбрать";
                buttonEdit.HeaderText = "Выбрать";
                buttonEdit.Text = "Выбрать";
                buttonEdit.UseColumnTextForButtonValue = true;
                dataGridView1.Columns.Add(buttonEdit);
                dataGridView1.Columns["Выбрать"].Width = 80;
                while (rdr.Read())
                {
                    int rowIndex = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowIndex];
                    row.Cells["EmployeeID"].Value = rdr[0];
                    //row.Cells["EmployeeFIO"].Value = rdr[1];
                    row.Cells["EmployeeF"].Value = rdr[1];
                    row.Cells["EmployeeI"].Value = rdr[2];
                    row.Cells["EmployeeO"].Value = rdr[3];
                    //row.Cells["telephone"].Value = rdr[2];
                    row.Cells["telephone"].Value = rdr[4];
                    row.Cells["status"].Value = rdr[5];

                    allRows1.Add(row);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка: {ex}");
            }
        }

        //Для кнопки "Выбрать"
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                FillDataGrid("SELECT EmployeeID AS 'id', " +
               "EmployeeF AS 'Фамилия'," +
               "EmployeeI AS 'Имя'," +
               "EmployeeO AS 'Отчетство'," +
               "telephone AS 'Номер телефона'," +
               "status AS 'Статус'" +
               "FROM employeeee; ");
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = row.Cells["EmployeeID"].Value.ToString(); //id
                textBoxF.Text = row.Cells["EmployeeF"].Value.ToString(); //FIO
                textBoxI.Text = row.Cells["EmployeeI"].Value.ToString(); //FIO
                textBoxO.Text = row.Cells["EmployeeO"].Value.ToString(); //FIO
                string cleanNumber = new string(maskedTextBox1.Text.Where(char.IsDigit).ToArray());
                Console.WriteLine(cleanNumber);
                maskedTextBox1.Text = row.Cells["telephone"].Value.ToString(); //telephone
                comboBoxStatus.Text = row.Cells["status"].Value.ToString(); //status 
            }
            FillDataGrid("SELECT EmployeeID AS 'id', " +
               "EmployeeF AS 'Фамилия'," +
               "EmployeeI AS 'Имя'," +
               "EmployeeO AS 'Отчетство'," +
               "concat(left(telephone, 7), ' * **', right(telephone, 5)) AS 'Номер телефона',"+
               "status AS 'Статус'"+
               " FROM  employeeee; ");
        }
        //Для ввода ФИО, Автоматически первая буква слова заглавная
        private void textBoxF_TextChanged(object sender, EventArgs e)
        {
            // Сохраняем текущее положение курсора
            int selectionStart = textBoxF.SelectionStart;
            int selectionLength = textBoxF.SelectionLength;

            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBoxF.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0) // Проверка длины слова
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBoxF.Text = string.Join(" ", words);

            // Восстанавливаем положение курсора
            textBoxF.SelectionStart = Math.Min(selectionStart, textBoxF.Text.Length);
            textBoxF.SelectionLength = selectionLength;
        }
        //Для ввода ФИО, Автоматически первая буква слова заглавная
        private void textBoxI_TextChanged(object sender, EventArgs e)
        {
            // Сохраняем текущее положение курсора
            int selectionStart = textBoxI.SelectionStart;
            int selectionLength = textBoxI.SelectionLength;

            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBoxI.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0) // Проверка длины слова
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBoxI.Text = string.Join(" ", words);

            // Восстанавливаем положение курсора
            textBoxI.SelectionStart = Math.Min(selectionStart, textBoxI.Text.Length);
            textBoxI.SelectionLength = selectionLength;
        }
        //Для ввода ФИО, Автоматически первая буква слова заглавная
        private void textBoxO_TextChanged(object sender, EventArgs e)
        {
            // Сохраняем текущее положение курсора
            int selectionStart = textBoxO.SelectionStart;
            int selectionLength = textBoxO.SelectionLength;

            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBoxO.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0) // Проверка длины слова
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBoxO.Text = string.Join(" ", words);

            // Восстанавливаем положение курсора
            textBoxO.SelectionStart = Math.Min(selectionStart, textBoxO.Text.Length);
            textBoxO.SelectionLength = selectionLength;
        }
        //Для ввода ФИО
        private void textBoxF_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                (e.KeyChar < 'а' || e.KeyChar > 'я') &&
                (e.KeyChar < 'А' || e.KeyChar > 'Я') &&
                e.KeyChar != '-')
            {
                e.Handled = true; // Отменяем ввод
            }
        }
        //Для ввода ФИО
        private void textBoxO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                (e.KeyChar < 'а' || e.KeyChar > 'я') &&
                (e.KeyChar < 'А' || e.KeyChar > 'Я'))
            {
                e.Handled = true; // Отменяем ввод
            }
        }
        //Для ввода Паспорта
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                   (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true; // Отменяем ввод
            }
        }

        /// <summary>
        /// Добавление/Изменение сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == "" || textBoxF.Text == "" || textBoxI.Text == "" || textBoxO.Text == "" || comboBoxStatus.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (textBox2.Text != "")
                {
                    // Редактирование существующей записи
                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int employeeID = Convert.ToInt32(textBox2.Text);
                        using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            con.Open();

                            // Проверяем, не занят ли номер телефона другим сотрудником
                            //if (IsTelephoneAlreadyAssignedToAnotherEmployee(maskedTextBox1.Text, employeeID, con))
                            //{
                            //    MessageBox.Show("Этот номер телефона уже используется другим сотрудником.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //    return;
                            //}

                            MySqlCommand cmd = new MySqlCommand(@"UPDATE employeeee 
                        SET EmployeeF = @employeeF,
                            EmployeeI = @employeeI,
                            EmployeeO = @employeeO,
                            telephone = @telephone,
                            status = @status
                        WHERE EmployeeID = @employeeID", con);
                            if (UserTelephoneExists(maskedTextBox1.Text, employeeID, con))
                            {
                                MessageBox.Show("Пользователь с таким телефоном уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            cmd.Parameters.AddWithValue("@employeeID", employeeID);
                            cmd.Parameters.AddWithValue("@employeeF", textBoxF.Text);
                            cmd.Parameters.AddWithValue("@employeeI", textBoxI.Text);
                            cmd.Parameters.AddWithValue("@employeeO", textBoxO.Text);
                            cmd.Parameters.AddWithValue("@telephone", maskedTextBox1.Text);
                            cmd.Parameters.AddWithValue("@status", comboBoxStatus.Text);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            con.Close();
                        }

                        // Очистка полей
                        maskedTextBox1.Text = "";
                        textBoxF.Text = "";
                        textBoxI.Text = "";
                        textBoxO.Text = "";
                        textBox2.Text = "";
                        comboBoxStatus.SelectedItem = null;

                        // Обновление DataGridView
                        FillDataGrid("SELECT EmployeeID AS 'id', " +
                                       "EmployeeF AS 'Фамилия'," +
                                       "EmployeeI AS 'Имя'," +
                                       "EmployeeO AS 'Отчество'," +
                                       "concat(left(telephone, 7), ' * **', right(telephone, 5)) AS 'Номер телефона'," +
                                       "status AS 'Статус'" +
                                       " FROM  employeeee; ");
                    }
                }
                else
                {
                    // Добавление новой записи
                    textBox2.Text = "";
                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить эту запись?", "Подтверждение добавления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            try
                            {
                                connection.Open();

                                string query = "INSERT INTO employeeee (EmployeeF, EmployeeI, EmployeeO, telephone, status) VALUES (@value1,@value4,@value5,@value2, @status)";
                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    if (UserTelephoneExists(maskedTextBox1.Text, connection))
                                    {
                                        MessageBox.Show("Пользователь с таким телефоном уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    cmd.Parameters.AddWithValue("@value1", textBoxF.Text);
                                    cmd.Parameters.AddWithValue("@value4", textBoxI.Text);
                                    cmd.Parameters.AddWithValue("@value5", textBoxO.Text);
                                    cmd.Parameters.AddWithValue("@value2", maskedTextBox1.Text);
                                    cmd.Parameters.AddWithValue("@status", comboBoxStatus.Text);

                                    cmd.ExecuteNonQuery();
                                    MessageBox.Show("Запись добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    maskedTextBox1.Text = "";
                                    textBoxF.Text = "";
                                    textBoxI.Text = "";
                                    textBoxO.Text = "";
                                    textBox2.Text = "";
                                    comboBoxStatus.SelectedItem = null;

                                    FillDataGrid("SELECT EmployeeID AS 'id', " +
                                               "EmployeeF AS 'Фамилия'," +
                                               "EmployeeI AS 'Имя'," +
                                               "EmployeeO AS 'Отчество'," +
                                               "concat(left(telephone, 7), ' * **', right(telephone, 5)) AS 'Номер телефона'," +
                                               "status AS 'Статус'" +
                                               " FROM  employeeee; ");
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
        }

        //// Вспомогательный метод для проверки, не занят ли телефон другим сотрудником при редактировании
        //private bool IsTelephoneAlreadyAssignedToAnotherEmployee(string telephone, int employeeID, MySqlConnection connection)
        //{
        //    string checkQuery = "SELECT COUNT(*) FROM employeeee WHERE telephone = @telephone AND EmployeeID != @employeeID";
        //    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
        //    {
        //        checkCmd.Parameters.AddWithValue("@telephone", telephone);
        //        checkCmd.Parameters.AddWithValue("@employeeID", employeeID);
        //        return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
        //    }
        //}

        // Функция для проверки существования пользователя по телефону (используется только при добавлении)
        private bool UserTelephoneExists(string telephone, MySqlConnection connection)
        {
            // Вызываем версию с employeeID = 0 (или любым другим невалидным ID),
            // чтобы исключить *всех* сотрудников
            return UserTelephoneExists(telephone, 0, connection);
        }

        private bool UserTelephoneExists(string telephone, int employeeID, MySqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM employeeee WHERE telephone = @Telephone AND EmployeeID != @EmployeeID"; // Исключаем текущего сотрудника

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Telephone", telephone);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID); // Добавляем employeeID в параметры
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }



        //Очистка всех полей
        private void button2_Click(object sender, EventArgs e)
        {
            maskedTextBox1.Text = ""; //telephone
            textBoxF.Text = ""; //FIO
            textBoxI.Text = ""; //FIO
            textBoxO.Text = ""; //FIO
            textBox2.Text = ""; //id
            comboBoxStatus.SelectedItem = null; //
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
        private void Staffs_Shown(object sender, EventArgs e)
        {
            Users_ActivateTracking();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}

