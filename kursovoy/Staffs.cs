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
        private Timer Timer = new Timer(); 
        private bool isTimerTickRunning = false;
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
                    MessageBox.Show("Вы были перенаправлены на страницу авторизации из-за бездействия.", "Блокировка системы", MessageBoxButtons.OK,MessageBoxIcon.Information);
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
        private void Staffs_Load(object sender, EventArgs e)
        {
            if (int.TryParse(ConfigurationManager.AppSettings["InactivityTimeout"], out int timeoutInSeconds))
            {
                inactivityTimeout = timeoutInSeconds * 1000; 
            }
            else
            {
                inactivityTimeout = 180000;
            }
            initialInactivityTimeout = inactivityTimeout; // Сохраняем начальное значение
            ResetInactivityTimer(); // Сброс таймера активности
            Timer.Start(); // Запуск таймера активности
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
                    row.Cells["EmployeeF"].Value = rdr[1];
                    row.Cells["EmployeeI"].Value = rdr[2];
                    row.Cells["EmployeeO"].Value = rdr[3];
                    row.Cells["telephone"].Value = rdr[4];
                    row.Cells["status"].Value = rdr[5];
                    allRows1.Add(row);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Для кнопки "Выбрать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Для ввода ФИО, Автоматически первая буква слова заглавная
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxF_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = textBoxF.SelectionStart;
            int selectionLength = textBoxF.SelectionLength;
            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBoxF.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBoxF.Text = string.Join(" ", words);
            textBoxF.SelectionStart = Math.Min(selectionStart, textBoxF.Text.Length);
            textBoxF.SelectionLength = selectionLength;
        }
        private void textBoxI_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = textBoxI.SelectionStart;
            int selectionLength = textBoxI.SelectionLength;
            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBoxI.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBoxI.Text = string.Join(" ", words);
            textBoxI.SelectionStart = Math.Min(selectionStart, textBoxI.Text.Length);
            textBoxI.SelectionLength = selectionLength;
        }
        private void textBoxO_TextChanged(object sender, EventArgs e)
        {
            int selectionStart = textBoxO.SelectionStart;
            int selectionLength = textBoxO.SelectionLength;
            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBoxO.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0) 
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBoxO.Text = string.Join(" ", words);
            textBoxO.SelectionStart = Math.Min(selectionStart, textBoxO.Text.Length);
            textBoxO.SelectionLength = selectionLength;
        }
        /// <summary>
        /// Для ввода ФИО
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void textBoxO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                (e.KeyChar < 'а' || e.KeyChar > 'я') &&
                (e.KeyChar < 'А' || e.KeyChar > 'Я'))
            {
                e.Handled = true; // Отменяем ввод
            }
        }
        /// <summary>
        /// Для ввода Паспорта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                // Редактирование записи
                if (textBox2.Text != "")
                {
                    DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int employeeID = Convert.ToInt32(textBox2.Text);
                        using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                        {
                            con.Open();
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
        /// <summary>
        /// Функция для проверки существования пользователя по телефону
        /// Для добавления
        /// </summary>
        /// <param name="telephone"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private bool UserTelephoneExists(string telephone, MySqlConnection connection)
        {
            return UserTelephoneExists(telephone, 0, connection);
        }
       /// <summary>
       /// Функция для проверки существования пользователя по телефону
       /// Для редактирования
       /// </summary>
       /// <param name="telephone"></param>
       /// <param name="employeeID"></param>
       /// <param name="connection"></param>
       /// <returns></returns>
        private bool UserTelephoneExists(string telephone, int employeeID, MySqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM employeeee WHERE telephone = @Telephone AND EmployeeID != @EmployeeID"; 

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Telephone", telephone);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID); 
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
        /// <summary>
        /// Очистка всех полей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            maskedTextBox1.Text = "";
            textBoxF.Text = "";
            textBoxI.Text = ""; 
            textBoxO.Text = ""; 
            textBox2.Text = ""; 
            comboBoxStatus.SelectedItem = null;
        }
        /// <summary>
        /// Сбрасывает отслеживание времени бездействия
        /// </summary>
        private void ResetInactivityTimer()
        {
            inactivityTimeout = initialInactivityTimeout; 
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
