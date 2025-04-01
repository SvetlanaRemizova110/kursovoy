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

namespace kursovoy
{
    public partial class ProductsEdit : Form
    {
        private int productId_;
        private string imageDirectory = @"./photo/";
        public ProductsEdit(int productId)
        {
            InitializeComponent();
            this.productId_ = productId;
        }
        private void ProductsEdit_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadDataIntoComboBox();// Для загрузки данных в comboBox
            LoadProductData(); // Загрузка данных в поля по артикулу
            LoadImage(); // Загрузка фото товара из Базы данных
        }

        /// <summary>
        /// Загрузка фото товара из Базы данных
        /// </summary>
        private void LoadImage()
        {
            string fileName = label10.Text;
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
        /// Загрузка фото по умолчанию
        /// </summary>
        private void LoadDefaultImage()
        {
            string defaultImagePath = Path.Combine(imageDirectory, "picture.png");
            if (File.Exists(defaultImagePath))
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(defaultImagePath);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения по умолчанию: {ex.Message}");
                    pictureBox1.Image = null; 
                }
            }
            else
            {
                MessageBox.Show("Изображение по умолчанию не найдено: picture.png");
                pictureBox1.Image = null; 
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
             INNER JOIN Category ON ProductCategory = CategoryID WHERE ProductArticul = " + productId_, con);
                cmd.Parameters.AddWithValue("ProductArticul", productId_);
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
                        label10.Text = rdr["ProductPhoto"].ToString(); //PHOTO
                    }
                }
                con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
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
                MessageBox.Show("Необходимо заполнить все поля!");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Вы уверены, что хотите отредактировать этот товар?", "Подтверждение редактирования!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    using (MySqlConnection con = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        con.Open();
                        
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
                            cmd.Parameters.AddWithValue("@ProductPhoto", label10.Text);
                            cmd.Parameters.AddWithValue("@productArticul", productId_);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Запись изменена!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        con.Close();
                    }
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Для получения ID категории
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private int GetCategoryIdByName(string categoryName,MySqlConnection con)
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
        /// Для получения ID прлизводителя
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
            label10.Visible = true;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Выберите фотографию";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(selectedFilePath);
                if (fileInfo.Extension.ToLower() != ".jpg" && fileInfo.Extension.ToLower() != ".png")
                {
                    MessageBox.Show("Ошибка: Выберите файл с расширением .jpg или .png.", "Ошибка выбора файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (fileInfo.Length > 2 * 1024 * 1024)
                {
                    MessageBox.Show("Ошибка: Размер файла должен быть не более 2 Мб.", "Ошибка размера файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string folderPath = @"./photo/";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Копируем выбранный файл
                File.Copy(selectedFilePath, Path.Combine(folderPath, Path.GetFileName(selectedFilePath)), true);

                pictureBox1.ImageLocation = Path.Combine(folderPath, Path.GetFileName(selectedFilePath));
                
                string fileName = Path.GetFileName(selectedFilePath);
                label10.Text = fileName;
            }
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
                e.Handled = true; // Отменяем ввод
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
                e.Handled = true; // Отменить ввод, если символ не является цифрой
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
                e.Handled = true; // Отменяем ввод
            }
            if (e.KeyChar == ' ')
            {
                e.Handled = false;
            }
        }
    }
}

