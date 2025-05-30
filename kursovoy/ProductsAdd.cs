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
using System.IO;
using System.Configuration;
namespace kursovoy
{
    public partial class ProductsAdd : Form
    {
        private int inactivityTimeout = 0;
        public ProductsAdd()
        {
            InitializeComponent();
            Timer.Tick += inactivityTimer_Tick;
            Timer.Interval = 1000; // Проверка каждые 1 секунду
            LoadDataIntoComboBox();
            label12.Visible = false;
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
        /// <summary>
        /// Для загрузки данных в comboBox
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            string query1 = "SELECT ProductManufacturName FROM ProductManufactur";
            string query2 = "SELECT SupplierName FROM Supplier";
            string query3 = "SELECT CategoryName FROM Category";
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                MySqlCommand command1 = new MySqlCommand(query1, connection);
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        comboBox3.Items.Add(reader1["ProductManufacturName"].ToString());
                    }
                }
                MySqlCommand command2 = new MySqlCommand(query2, connection);
                using (MySqlDataReader reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        comboBox2.Items.Add(reader2["SupplierName"].ToString());
                    }
                }
                MySqlCommand command3 = new MySqlCommand(query3, connection);
                using (MySqlDataReader reader3 = command3.ExecuteReader())
                {
                    while (reader3.Read())
                    {
                        comboBox1.Items.Add(reader3["CategoryName"].ToString());
                    }
                }
                connection.Close();
            }
        }
        /// <summary>
        /// Расчет кол-ва записей для проверки на дублирование
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private bool UserProductArticulExists(string articul, MySqlConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM Product WHERE ProductArticul = @productArticul";
            using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection))
            {
                checkCmd.Parameters.AddWithValue("@productArticul", articul);
                return Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
            }
        }

        /// <summary>
        /// Добавление товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if ( textBoxName.Text == "" || textBox5.Text == "" || textBox7.Text == "" || textBox2.Text == "" || textBox1.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля!");
            }//textBox6.Text == "" ||
            else
            {
                using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    try
                    {
                        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить товар?", "Подтверждение добавления!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            connection.Open();
                            // Проверка на дублирование данных(по артикулу)
                            if (UserProductArticulExists(textBox6.Text, connection))
                            {
                                MessageBox.Show("Товар с таким артикулом уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            // Для получения ID производителя
                            int manufacturerId;
                            string manufacturerQuery = "SELECT ProductManufacturID FROM ProductManufactur WHERE ProductManufacturName = @productmanufacturname";
                            using (MySqlCommand manufacturerCmd = new MySqlCommand(manufacturerQuery, connection))
                            {
                                manufacturerCmd.Parameters.AddWithValue("@productmanufacturname", comboBox3.Text);
                                object result = manufacturerCmd.ExecuteScalar();
                                manufacturerId = result != null ? Convert.ToInt32(result) : 0;
                            }
                            // Для получения ID поставщика
                            int supplierId;
                            string supplierQuery = "SELECT SupplierID FROM Supplier WHERE SupplierName = @supplierName";
                            using (MySqlCommand supplierCmd = new MySqlCommand(supplierQuery, connection))
                            {
                                supplierCmd.Parameters.AddWithValue("@supplierName", comboBox2.Text);
                                object result = supplierCmd.ExecuteScalar();
                                supplierId = result != null ? Convert.ToInt32(result) : 0;
                            }
                            // Для получения ID категории
                            int categoryID;
                            string categoryQuery = "SELECT CategoryID FROM Category WHERE CategoryName = @categoryName";
                            using (MySqlCommand categoryCmd = new MySqlCommand(categoryQuery, connection))
                            {
                                categoryCmd.Parameters.AddWithValue("@categoryName", comboBox1.Text);
                                object result = categoryCmd.ExecuteScalar();
                                categoryID = result != null ? Convert.ToInt32(result) : 0;
                            }
                            // Запрос добавления
                            string query2 = "INSERT INTO Product (" +
                               "Name," +
                               "Description," +
                               "Cost," +
                               "Unit," +
                               "ProductQuantityInStock," +
                               "ProductCategory, " +
                               "ProductManufactur," +
                               "ProductSupplier," +
                               "ProductPhoto" +
                               ")" +
                               " VALUES (@value1,@value2,@value3,@value4,@value5,@value6,@value7,@value8,@value9)";
                            using (MySqlCommand cmd = new MySqlCommand(query2, connection))
                            {
                               // cmd.Parameters.AddWithValue("@value0", textBox6.Text);//Art@value0, "ProductArticul," +
                                cmd.Parameters.AddWithValue("@value1", textBoxName.Text);//Name
                                cmd.Parameters.AddWithValue("@value2", textBox5.Text);//description
                                cmd.Parameters.AddWithValue("@value3", textBox7.Text);//Cost
                                cmd.Parameters.AddWithValue("@value4", textBox2.Text);//Unit
                                cmd.Parameters.AddWithValue("@value5", textBox1.Text);//ProductQuantityInStock

                                cmd.Parameters.AddWithValue("@value6", categoryID);//category
                                cmd.Parameters.AddWithValue("@value7", manufacturerId);//Manufacter
                                cmd.Parameters.AddWithValue("@value8", supplierId);//supplier

                                cmd.Parameters.AddWithValue("@value9", label12.Text);//PHOTO
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Запись добавлена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            connection.Close();

                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex);
                    }
                }
            }
        }

        private void ProductsAdd_Load(object sender, EventArgs e)
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
            // Запрет изменения comboBox
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// Добавление фото товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            label12.Visible = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Выберите фотографию";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(selectedFilePath);

                // Проверка типа файла
                if (fileInfo.Extension.ToLower() != ".jpg" && fileInfo.Extension.ToLower() != ".png")
                {
                    MessageBox.Show("Ошибка: Выберите файл с расширением .jpg или .png.", "Ошибка выбора файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Проверка размера файла
                if (fileInfo.Length > 2 * 1024 * 1024) // 2 Мб в байтах
                {
                    MessageBox.Show("Ошибка: Размер файла должен быть не более 2 Мб.", "Ошибка размера файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Путь к папке для хранения изображений
                string folderPath = @"./photo/";
                // Создаем папку, если она не существует
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Копируем файл в папку
                string fileName = Path.GetFileName(selectedFilePath);
                string targetPath = Path.Combine(folderPath, fileName);
                File.Copy(selectedFilePath, targetPath, true);

                // Отображаем фото в PictureBox
                pictureBox1.Image = Image.FromFile(targetPath);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                // Выводим название файла в Label
                label12.Text = fileName;
            }
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
        private void ProductsAdd_Shown(object sender, EventArgs e)
        {
            Users_ActivateTracking();
        }

        //Description
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                    !char.IsLetter(e.KeyChar))
            {
                e.Handled = true; // Отменяем ввод
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = false;
            }
        }

        //Name
        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
            (e.KeyChar < 'а' || e.KeyChar > 'я') &&
            (e.KeyChar < 'А' || e.KeyChar > 'Я') &&
            (e.KeyChar < 'A' || e.KeyChar > 'Z') &&
             (e.KeyChar < 'a' || e.KeyChar > 'z'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = false;
            }
        }
        //Cost, count, Art
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверка на ввод только чисел
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
        //Unit
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) &&
            (e.KeyChar < 'а' || e.KeyChar > 'я') &&
            (e.KeyChar < 'А' || e.KeyChar > 'Я') &&
            (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true; 
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}

