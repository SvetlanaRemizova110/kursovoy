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
        private int inactivityTimeout = 0;
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
        private void button4_Click(object sender, EventArgs e)
        {
            Admin ad = new Admin();
            ad.Show();
            this.Hide();
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

            ResetInactivityTimer(); // Сброс таймера активности
            Timer.Start(); // Запуск таймера активности
            //FillDataGrid("SELECT EmployeeID AS 'id', EmployeeFIO AS 'ФИО сотрудника'," +
            //    " telephone AS 'Номер телефона', " +
            //    "pasport AS 'Паспорт' " +
            //    "FROM  employee;");
            //FillDataGrid("SELECT EmployeeID AS 'id', EmployeeF AS 'Фамилия',EmployeeI AS 'Имя',EmployeeO AS 'Отчетство'," +
            //            "telephone AS 'Номер телефона'," +
            //            "pasport AS 'Паспорт'" +
            //            "FROM  employeeee; ");
            FillDataGrid("SELECT EmployeeID AS 'id', " +
               "EmployeeF AS 'Фамилия'," +
               "left(EmployeeI, 1) AS 'Имя'," +
               "left(EmployeeO, 1) AS 'Отчетство'," +
               "concat(left(telephone, 7), ' * **', right(telephone, 5)) AS 'Номер телефона'," +
               "concat(left(pasport, 5), ' * ***') AS 'Паспорт'" +
               " FROM  employeeee; ");

            maskedTextBox1.Mask = "+7(000)000-00-00";
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

                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ReadOnly = true;

                dataGridView1.Columns.Add("EmployeeID", "id");
                dataGridView1.Columns["EmployeeID"].Visible = false;
                //dataGridView1.Columns.Add("EmployeeFIO", "ФИО сотрудника");
                dataGridView1.Columns.Add("EmployeeF", "Фамилия");
                dataGridView1.Columns.Add("EmployeeI", "Имя");
                dataGridView1.Columns.Add("EmployeeO", "Отчетство");
                dataGridView1.Columns.Add("telephone", "Номер телефона");
                dataGridView1.Columns.Add("pasport", "Паспорт");

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
                    row.Cells["EmployeeID"].Value = rdr[0];
                    //row.Cells["EmployeeFIO"].Value = rdr[1];
                    row.Cells["EmployeeF"].Value = rdr[1];
                    row.Cells["EmployeeI"].Value = rdr[2];
                    row.Cells["EmployeeO"].Value = rdr[3];
                    //row.Cells["telephone"].Value = rdr[2];
                    //row.Cells["pasport"].Value = rdr[3];
                    row.Cells["telephone"].Value = rdr[4];
                    row.Cells["pasport"].Value = rdr[5];

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
            FillDataGrid("SELECT EmployeeID AS 'id', " +
               "EmployeeF AS 'Фамилия'," +
               "EmployeeI AS 'Имя'," +
               "EmployeeO AS 'Отчетство'," +
               "telephone AS 'Номер телефона'," +
               "pasport AS 'Паспорт'" +
               "FROM employeeee; ");
            if (e.ColumnIndex == dataGridView1.Columns["Выбрать"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = row.Cells["EmployeeID"].Value.ToString(); //id
                //textBox4.Text = row.Cells["EmployeeFIO"].Value.ToString(); //FIO
                textBoxF.Text = row.Cells["EmployeeF"].Value.ToString(); //FIO
                textBoxI.Text = row.Cells["EmployeeI"].Value.ToString(); //FIO
                textBoxO.Text = row.Cells["EmployeeO"].Value.ToString(); //FIO
                string cleanNumber = new string(maskedTextBox1.Text.Where(char.IsDigit).ToArray());
                Console.WriteLine(cleanNumber);
                maskedTextBox1.Text = row.Cells["telephone"].Value.ToString(); //telephone
                textBox3.Text = row.Cells["pasport"].Value.ToString();//pasport
               // FillDataGrid("SELECT EmployeeID AS 'id', " +
               //"EmployeeF AS 'Фамилия'," +
               //"left(EmployeeI, 1) AS 'Имя'," +
               //"left(EmployeeO, 1) AS 'Отчетство'," +
               //"telephone AS 'Номер телефона'," +
               //"pasport AS 'Паспорт'" +
               //"FROM employeeee; ");
                FillDataGrid("SELECT EmployeeID AS 'id', " +
                "EmployeeF AS 'Фамилия',"+
                "left(EmployeeI, 1) AS 'Имя'," +
                "left(EmployeeO, 1) AS 'Отчетство'," +
                "concat(left(telephone, 7), ' * **', right(telephone, 5)) AS 'Номер телефона'," +
                "concat(left(pasport, 5), ' * ***') AS 'Паспорт'"+
                " FROM  employeeee; ");
//                SELECT EmployeeID AS 'id', 
//EmployeeF AS 'Фамилия',
//left(EmployeeI, 1) AS 'Имя',
//left(EmployeeO, 1) AS 'Отчетство',
//concat(left(telephone, 7), "***", right(telephone, 5)) AS 'Номер телефона',
//concat(left(pasport, 5), "****") AS 'Паспорт'
//FROM employeeee;
            }
        }
        //Для ввода ФИО, Автоматически первая буква слова заглавная
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            //// Сохраняем текущее положение курсора
            //int selectionStart = textBox4.SelectionStart;
            //int selectionLength = textBox4.SelectionLength; 

            //// Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            //string[] words = textBox4.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < words.Length; i++)
            //{
            //    if (words[i].Length > 0) // Проверка длины слова
            //    {
            //        words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
            //    }
            //}
            //textBox4.Text = string.Join(" ", words);

            //// Восстанавливаем положение курсора
            //textBox4.SelectionStart = Math.Min(selectionStart, textBox4.Text.Length);  
            //textBox4.SelectionLength = selectionLength; 
        }
        //Для ввода ФИО
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            //// Разрешаем ввод пробела и кириллицы
            //if (!char.IsControl(e.KeyChar) &&
            //    (e.KeyChar < 'а' || e.KeyChar > 'я') &&
            //    (e.KeyChar < 'А' || e.KeyChar > 'Я') &&
            //    e.KeyChar != ' ')
            //{
            //    e.Handled = true; // Отменяем ввод
            //}
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
        /// Изменение сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == "" || textBoxF.Text == "" ||textBoxI.Text == "" ||textBoxO.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    int employeeID = Convert.ToInt32(textBox2.Text);
                    using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand(@"UPDATE employeeee 
                        SET EmployeeID = @employeeID,
                            EmployeeF = @employeeF,
                            EmployeeI = @employeeI,
                            EmployeeO = @employeeO,
                            telephone = @telephone,
                            pasport = @pasport
                        WHERE EmployeeID = @employeeID", con);

                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        //cmd.Parameters.AddWithValue("@employeeFIO", textBox4.Text);
                        cmd.Parameters.AddWithValue("@employeeF", textBoxF.Text);
                        cmd.Parameters.AddWithValue("@employeeI", textBoxI.Text);
                        cmd.Parameters.AddWithValue("@employeeO", textBoxO.Text);
                        cmd.Parameters.AddWithValue("@pasport", textBox3.Text);
                        cmd.Parameters.AddWithValue("@telephone", maskedTextBox1.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        con.Close();
                    }
                    maskedTextBox1.Text = ""; //telephone
                    textBoxF.Text = ""; //FIO
                    textBoxI.Text = ""; //FIO
                    textBoxO.Text = ""; //FIO
                    textBox3.Text = "";//pasport
                    textBox2.Text = ""; //id
                    //FillDataGrid("SELECT EmployeeID AS 'id', EmployeeFIO AS 'ФИО сотрудника'," +
                    //" telephone AS 'Номер телефона', " +
                    //"pasport AS 'Паспорт' " +
                    //"FROM  employee;");
                    FillDataGrid("SELECT EmployeeID AS 'id', EmployeeF AS 'Фамилия',EmployeeI AS 'Имя',EmployeeO AS 'Отчетство'," +
                        "telephone AS 'Номер телефона'," +
                        "pasport AS 'Паспорт'" +
                        "FROM  employeeee; ");
                }

            }
        }
        // Функция для проверки существования пользователя по телефону
        private bool UserTelephoneExists(string telephone, MySqlConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM Employee WHERE telephone = @telephone";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@telephone", telephone);
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }
        // Функция для проверки существования пользователя по ФИО
        //private bool UserFIOExists(string employeeFIO, MySqlConnection connection)
        //{
        //    //string checkQuery = "SELECT COUNT(*) FROM Employee WHERE EmployeeFIO = @employeeFIO";
        //    //using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
        //    //{
        //    //    checkCmd.Parameters.AddWithValue("@employeeFIO", employeeFIO);
        //    //    return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
        //    //}
        //}
        // Функция для проверки существования пользователя по Паспорту
        private bool UserPasportExists(string employeeFIO, MySqlConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM Employee WHERE pasport = @pasport";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@pasport", employeeFIO);
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// Добавление сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == "" || textBoxF.Text == "" ||textBoxI.Text == "" ||textBoxO.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить эту запись?", "Подтверждение добавления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        try
                        {
                            connection.Open();

                            //string query = "INSERT INTO Employee (EmployeeFIO, telephone, pasport) VALUES (@value1,@value2,@value3)";
                            string query = "INSERT INTO employeeee (EmployeeF, EmployeeI, EmployeeO, telephone, pasport) VALUES (@value1,@value4,@value5,@value2,@value3)";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {

                                //if (UserFIOExists(textBox4.Text, connection))
                                //{
                                //    MessageBox.Show("Пользователь с таким ФИО уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //    return;
                                //}
                                if (UserTelephoneExists(maskedTextBox1.Text, connection))
                                {
                                    MessageBox.Show("Пользователь с таким телефоном уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                if (UserPasportExists(textBox3.Text, connection))
                                {
                                    MessageBox.Show("Пользователь с таким паспортом уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                cmd.Parameters.AddWithValue("@value1", textBoxF.Text);
                                cmd.Parameters.AddWithValue("@value4", textBoxI.Text);
                                cmd.Parameters.AddWithValue("@value5", textBoxO.Text);
                                cmd.Parameters.AddWithValue("@value2", maskedTextBox1.Text);
                                cmd.Parameters.AddWithValue("@value3", textBox3.Text);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                maskedTextBox1.Text = ""; //telephone
                                textBoxF.Text = ""; //FIO
                                textBoxI.Text = ""; //FIO
                                textBoxO.Text = ""; //FIO
                                textBox3.Text = "";//pasport
                                textBox2.Text = ""; //id
                                FillDataGrid("SELECT EmployeeID AS 'id', EmployeeF AS 'Фамилия',EmployeeI AS 'Имя',EmployeeO AS 'Отчетство'," +
                                "telephone AS 'Номер телефона'," +
                                "pasport AS 'Паспорт'" +
                                "FROM  employeeee; ");
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
        //Очистка всех полей
        private void button2_Click(object sender, EventArgs e)
        {
            maskedTextBox1.Text = ""; //telephone
            textBoxF.Text = ""; //FIO
            textBoxI.Text = ""; //FIO
            textBoxO.Text = ""; //FIO
            textBox3.Text = "";//pasport
            textBox2.Text = ""; //id
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
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Staffs_Shown(object sender, EventArgs e)
        {
            Users_ActivateTracking();
        }
    }
}

