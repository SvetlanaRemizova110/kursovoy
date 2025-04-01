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
namespace kursovoy
{
    public partial class Staffs : Form
    {
        private List<DataGridViewRow> allRows1 = new List<DataGridViewRow>();
        public Staffs()
        {
            InitializeComponent();
            textBox2.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Admin ad = new Admin();
            ad.Show();
            this.Hide();
        }

        private void Staffs_Load(object sender, EventArgs e)
        {
            FillDataGrid("SELECT EmployeeID AS 'id', EmployeeFIO AS 'ФИО сотрудника'," +
                " telephone AS 'Номер телефона', " +
                "pasport AS 'Паспорт' " +
                "FROM  employee;");
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
                dataGridView1.Columns.Add("EmployeeFIO", "ФИО сотрудника");
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
                    row.Cells["EmployeeFIO"].Value = rdr[1];
                    row.Cells["telephone"].Value = rdr[2];
                    row.Cells["pasport"].Value = rdr[3];

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
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox2.Text = row.Cells["EmployeeID"].Value.ToString(); //id
                textBox4.Text = row.Cells["EmployeeFIO"].Value.ToString(); //FIO
                string cleanNumber = new string(maskedTextBox1.Text.Where(char.IsDigit).ToArray());
                Console.WriteLine(cleanNumber);
                maskedTextBox1.Text = row.Cells["telephone"].Value.ToString(); //telephone
                textBox3.Text = row.Cells["pasport"].Value.ToString();//pasport
            }
        }
        //Для ввода ФИО, Автоматически первая буква слова заглавная
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            // Сохраняем текущее положение курсора
            int selectionStart = textBox4.SelectionStart;
            int selectionLength = textBox4.SelectionLength; 

            // Преобразуем текст так, чтобы каждое слово начиналось с заглавной буквы
            string[] words = textBox4.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0) // Проверка длины слова
                {
                    words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i].ToLower());
                }
            }
            textBox4.Text = string.Join(" ", words);

            // Восстанавливаем положение курсора
            textBox4.SelectionStart = Math.Min(selectionStart, textBox4.Text.Length);  
            textBox4.SelectionLength = selectionLength; 
        }
        //Для ввода ФИО
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем ввод пробела и кириллицы
            if (!char.IsControl(e.KeyChar) &&
                (e.KeyChar < 'а' || e.KeyChar > 'я') &&
                (e.KeyChar < 'А' || e.KeyChar > 'Я') &&
                e.KeyChar != ' ')
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
        /// Изменение сотрудника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text == "" || textBox4.Text == "" || textBox3.Text == "")
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
                        MySqlCommand cmd = new MySqlCommand(@"UPDATE Employee 
                        SET EmployeeID = @employeeID,
                            EmployeeFIO = @employeeFIO,
                            telephone = @telephone,
                            pasport = @pasport
                        WHERE EmployeeID = @employeeID", con);

                        cmd.Parameters.AddWithValue("@employeeID", employeeID);
                        cmd.Parameters.AddWithValue("@employeeFIO", textBox4.Text);
                        cmd.Parameters.AddWithValue("@pasport", textBox3.Text);
                        cmd.Parameters.AddWithValue("@telephone", maskedTextBox1.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        con.Close();
                    }
                    maskedTextBox1.Text = ""; //telephone
                    textBox4.Text = ""; //FIO
                    textBox3.Text = "";//pasport
                    textBox2.Text = ""; //id
                    FillDataGrid("SELECT EmployeeID AS 'id', EmployeeFIO AS 'ФИО сотрудника'," +
                    " telephone AS 'Номер телефона', " +
                    "pasport AS 'Паспорт' " +
                    "FROM  employee;");
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
        private bool UserFIOExists(string employeeFIO, MySqlConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM Employee WHERE EmployeeFIO = @employeeFIO";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@employeeFIO", employeeFIO);
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }
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
            if (maskedTextBox1.Text == "" || textBox4.Text == "" || textBox3.Text == "")
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

                            string query = "INSERT INTO Employee (EmployeeFIO, telephone, pasport) VALUES (@value1,@value2,@value3)";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {

                                if (UserFIOExists(textBox4.Text, connection))
                                {
                                    MessageBox.Show("Пользователь с таким ФИО уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
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
                                cmd.Parameters.AddWithValue("@value1", textBox4.Text);
                                cmd.Parameters.AddWithValue("@value2", maskedTextBox1.Text);
                                cmd.Parameters.AddWithValue("@value3", textBox3.Text);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                maskedTextBox1.Text = ""; //telephone
                                textBox4.Text = ""; //FIO
                                textBox3.Text = "";//pasport
                                textBox2.Text = ""; //id
                                FillDataGrid("SELECT EmployeeID AS 'id', EmployeeFIO AS 'ФИО сотрудника'," +
                                " telephone AS 'Номер телефона', " +
                                "pasport AS 'Паспорт' " +
                                "FROM  employee;");
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
            textBox4.Text = ""; //FIO
            textBox3.Text = "";//pasport
            textBox2.Text = ""; //id
        }
    }
}

