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
using System.IO;
using System.Configuration;
namespace kursovoy
{
    public partial class ProductsAdd : Form
    {
        private string _currentPhotoPath = null; // текущий путь к фотографии
        private const string DefaultImageName = "picture.png"; // Имя файла-заглушки
        public ProductsAdd()
        {
            InitializeComponent();
            LoadDataIntoComboBox();
            label12.Visible = false;
        }
        /// <summary>
        /// Для загрузки данных в comboBox
        /// </summary>
        private void LoadDataIntoComboBox()
        {
            try
            {
                string query1 = "SELECT ProductManufacturName FROM ProductManufactur";
                string query2 = "SELECT SupplierName FROM Supplier";
                string query3 = "SELECT CategoryName FROM Category";
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
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
            if (textBoxName.Text == "" || textBox5.Text == "" || textBox7.Text == "" || textBox2.Text == "" || textBox1.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
                {
                    try
                    {
                        DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите добавить эту запись?", "Подтверждение добавления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            connection.Open();
                            // Проверка на дублирование данных(по артикулу)
                            if (UserProductArticulExists(textBox6.Text, connection))
                            {
                                MessageBox.Show("Товар с таким артикулом уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            string query2 = "INSERT INTO Product (" +
                                "ProductArticul," +
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
                               " VALUES (@value0,@value1,@value2,@value3,@value4,@value5,@value6,@value7,@value8,@value9)";
                            using (MySqlCommand cmd = new MySqlCommand(query2, connection))
                            {
                                cmd.Parameters.AddWithValue("@value0", textBox6.Text);//Art 
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
            LoadDefaultImage();
        }
        /// <summary>
        /// Загрузка стандартной картинки
        /// </summary>
        private void LoadDefaultImage()
        {
            string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo", DefaultImageName);
            if (!File.Exists(defaultImagePath))
            {
                MessageBox.Show($"Файл заглушки {DefaultImageName} не найден. Пожалуйста, поместите его в папку photo.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                pictureBox1.Image = null;
                label12.Text = "";
                _currentPhotoPath = null;
                return;
            }
            try
            {
                pictureBox1.Image = Image.FromFile(defaultImagePath);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                label12.Text = DefaultImageName; // Указываем имя файла заглушки
                _currentPhotoPath = defaultImagePath; // Сохраняем путь к заглушке
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки дефолтной картинки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Добавление фото товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                openFileDialog.Title = "Выберите фотографию";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;
                    FileInfo fileInfo = new FileInfo(selectedFilePath);
                    // Проверка типа файла
                    if (fileInfo.Extension.ToLower() != ".jpg" && fileInfo.Extension.ToLower() != ".jpeg" && fileInfo.Extension.ToLower() != ".png") // Добавил jpeg
                    {
                        MessageBox.Show("Ошибка: Выберите файл с расширением .jpg, .jpeg или .png.", "Ошибка выбора файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // Проверка размера файла
                    if (fileInfo.Length > 3 * 1024 * 1024) // 3 Мб 
                    {
                        MessageBox.Show("Ошибка: Размер файла должен быть не более 3 Мб.", "Ошибка размера файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // Получаем артикул товара, чтобы использовать его в качестве имени файла
                    string productArticul = textBox6.Text;
                    if (string.IsNullOrEmpty(productArticul))
                    {
                        MessageBox.Show("Пожалуйста, укажите артикул товара перед добавлением фотографии.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // Путь к папке для хранения изображений
                    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo");

                    // Создаем папку, если она не существует
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Формируем новое имя файла на основе артикула
                    string fileExtension = Path.GetExtension(selectedFilePath);
                    string newFileName = productArticul + fileExtension;
                    string targetPath = Path.Combine(folderPath, newFileName);

                    try
                    {
                        // Копируем файл в папку с новым именем
                        File.Copy(selectedFilePath, targetPath, true);

                        pictureBox1.Image = Image.FromFile(targetPath);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                        // Сохраняем путь к новой фотографии и имя файла
                        _currentPhotoPath = targetPath;
                        label12.Text = newFileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Кнопка "Удалить фото"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            LoadDefaultImage();
        }
       /// <summary>
       /// Проверка на ввод только чисел для Cost, count, Art
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        ///  Проверка на ввод для Unit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
