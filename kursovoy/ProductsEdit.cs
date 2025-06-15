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
    public partial class ProductsEdit : Form
    {
        private int productArticul_;
        private string imageDirectory = @"./photo/";

        private string _currentPhotoPath = null; // Храним текущий путь к фотографии
        private const string DefaultImageName = "picture.png"; // Имя файла-заглушки

        public ProductsEdit(int productArticul)
        {
            InitializeComponent();
            this.productArticul_ = productArticul;
            label12.Visible = false;
        }
        private void ProductsEdit_Load(object sender, EventArgs e)
        {
            LoadDataIntoComboBox();
            LoadProductData(); 
            LoadImage(); 
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
        /// Загрузка фото товара из Базы данных
        /// </summary>
        private void LoadImage()
        {
            string fileName = label12.Text;
            string fullPath = Path.Combine(imageDirectory, fileName);
            if (File.Exists(fullPath))
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(fullPath);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                    pictureBox1.Image = null;
                }
            }
            else
            {
                LoadDefaultImage();
            }
        }
        /// <summary>
        ///  Для загрузки данных в comboBox
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
        /// Загрузка данных в поля по артикулу
        /// </summary>
        private void LoadProductData()
        {
            using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(@"SELECT ProductArticul,Name,Description,
                 Cost,Unit,ProductQuantityInStock,ProductManufactur.ProductManufacturName,
                 Supplier.SupplierName,Category.CategoryName,ProductPhoto
                 FROM Product 
                 INNER JOIN ProductManufactur ON ProductManufactur = ProductManufacturID
                 INNER JOIN Supplier ON ProductSupplier = SupplierID
                 INNER JOIN Category ON ProductCategory = CategoryID WHERE ProductArticul = @ProductArticul", con);
                cmd.Parameters.AddWithValue("@ProductArticul", productArticul_);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        textBox6.Text = rdr["ProductArticul"].ToString();//Name
                        textBoxName.Text = rdr["Name"].ToString();//Name
                        textBox5.Text = rdr["Description"].ToString();//description
                        textBox7.Text = rdr["Cost"].ToString();//Cost
                        textBox2.Text = rdr["Unit"].ToString();//Unit
                        textBox1.Text = rdr["ProductQuantityInStock"].ToString();//ProductQuantityInStock
                        comboBox1.Text = rdr["CategoryName"].ToString();//category
                        comboBox2.Text = rdr["SupplierName"].ToString(); //supplier
                        comboBox3.Text = rdr["ProductManufacturName"].ToString(); //Manufacter
                        label12.Text = rdr["ProductPhoto"].ToString(); //PHOTO
                    }
                }
                con.Close();
            }
        }
        /// <summary>
        /// Проверка на уникальность артикула
        /// </summary>
        /// <param name="articul">Артикул для проверки</param>
        /// <param name="con">Открытое соединение с базой данных</param>
        /// <returns>True, если артикул уникален, False - иначе</returns>
        private bool IsArticulUnique(int articul, MySqlConnection con)
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM Product WHERE ProductArticul = @ProductArticul", con))
            {
                cmd.Parameters.AddWithValue("@ProductArticul", articul);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 0; // Артикул уникален, если количество совпадений равно 0
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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
        /// Редактирование товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBoxName.Text == "" || textBox5.Text == "" || textBox7.Text == "" || textBox2.Text == "" || textBox1.Text == "" || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите изменить эту запись?", "Подтверждение изменения!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    con.Open();
                    int newArticul = Convert.ToInt32(textBox6.Text);
                    if (newArticul != productArticul_)
                    {
                        // Проверка на дублирование данных(по артикулу)
                        if (!IsArticulUnique(newArticul, con))
                        {
                            MessageBox.Show("Товар с таким артикулом уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; 
                        }
                        MySqlCommand cmdUpdateArticul = new MySqlCommand("UPDATE Product SET ProductArticul = @newArticul WHERE ProductArticul = @oldArticul", con);
                        cmdUpdateArticul.Parameters.AddWithValue("@newArticul", newArticul);
                        cmdUpdateArticul.Parameters.AddWithValue("@oldArticul", productArticul_); // Используем старый артикль для WHERE
                        cmdUpdateArticul.ExecuteNonQuery();

                        productArticul_ = newArticul;
                    }
                    MySqlCommand cmd = new MySqlCommand(@"UPDATE Product 
                    SET Name = @name,
                    Description = @description,
                    Cost = @cost,
                    Unit = @unit,
                    ProductCategory=@ProductCategory,
                    ProductManufactur=@ProductManufactur,
                    ProductSupplier = @ProductSupplier,
                    ProductQuantityInStock = @ProductQuantityInStock,
                    ProductPhoto = @ProductPhoto
                    WHERE ProductArticul = @productArticul", con);
                    cmd.Parameters.AddWithValue("@name", textBoxName.Text);
                    cmd.Parameters.AddWithValue("@description", textBox5.Text);
                    cmd.Parameters.AddWithValue("@cost", int.Parse(textBox7.Text));
                    cmd.Parameters.AddWithValue("@unit", textBox2.Text);
                    cmd.Parameters.AddWithValue("@ProductQuantityInStock", int.Parse(textBox1.Text));
                    string selectedCategory = comboBox1.Text.ToString();
                    int categoryid = GetCategoryIdByName(selectedCategory, con);
                    cmd.Parameters.AddWithValue("@ProductCategory", categoryid);
                    string selectedManufactur = comboBox3.Text.ToString();
                    int Manufacturid = GetManufacturIdByName(selectedManufactur, con);
                    cmd.Parameters.AddWithValue("@ProductManufactur", Manufacturid);
                    string selectedSupplier = comboBox2.Text.ToString();
                    int Supplierid = GetSupplierIdByName(selectedSupplier, con);
                    cmd.Parameters.AddWithValue("@ProductSupplier", Supplierid);
                    cmd.Parameters.AddWithValue("@ProductPhoto", label12.Text);
                    cmd.Parameters.AddWithValue("@productArticul", productArticul_); // Используем productArticul_
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                }
                this.Close();
            }
        }
        /// <summary>
        /// Для получения ID категории
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private int GetCategoryIdByName(string categoryName, MySqlConnection con)
        {
            int categoryId = -1;
            string query = "SELECT CategoryID FROM Category WHERE CategoryName = @categoryName";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@categoryName", categoryName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        categoryId = Convert.ToInt32(result);
                    }
                }
                connection.Close();
            }
            return categoryId;
        }
        /// <summary>
        /// Для получения ID производителя
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private int GetManufacturIdByName(string manufacturName, MySqlConnection con)
        {
            int Manufacturid = -1;
            string query = "SELECT ProductManufacturID FROM productmanufactur WHERE ProductManufacturName = @productManufacturName";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@productManufacturName", manufacturName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        Manufacturid = Convert.ToInt32(result);
                    }
                }
                connection.Close();
            }
            return Manufacturid;
        }
        /// <summary>
        /// Для получения ID поставщика
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private int GetSupplierIdByName(string SupplierName, MySqlConnection con)
        {
            int Supplierid = -1;
            string query = "SELECT SupplierID FROM Supplier WHERE SupplierName = @SupplierName";
            using (var connection = new MySqlConnection(Authorization.Program.ConnectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SupplierName", SupplierName);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        Supplierid = Convert.ToInt32(result);
                    }
                }
                connection.Close();
            }
            return Supplierid;
        }
        /// <summary>
        /// Изменение фото товара
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
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
                    // Проверяем, существует ли файл с таким именем
                    if (File.Exists(targetPath))
                    {
                        pictureBox1.Image = Image.FromFile(targetPath);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        _currentPhotoPath = targetPath;
                        label12.Text = newFileName;
                        MessageBox.Show("Фотография уже существует. Использована существующая.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Если файл не существует, копируем его
                        File.Copy(selectedFilePath, targetPath, true);

                        pictureBox1.Image = Image.FromFile(targetPath);
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                        // Сохраняем путь к новой фотографии и имя файла
                        _currentPhotoPath = targetPath;
                        label12.Text = newFileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
                e.Handled = true; // Отменить ввод, если символ не является цифрой
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
    }
}
