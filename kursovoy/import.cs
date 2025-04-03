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
using System.Configuration;
using System.IO;
namespace kursovoy
{
    public partial class import : Form
    {
        public import()
        {
            InitializeComponent();
            ComboBoxTables();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Authorization a = new Authorization();
            this.Hide();
            a.Show();
        }
        private void ComboBoxTables()
        {
            using (MySqlConnection conn = new MySqlConnection(Authorization.Program.ConnectionStringNotDB))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("USE db45; SHOW TABLES;", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader.GetString(0));
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1049) // Error 1049 - Unknown database
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке таблиц: " + ex.Message);
                }
            }
        }
        private void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
            string dbName = $"db45";
            CreateDatabase(Authorization.Program.ConnectionStringNotDB, dbName);
            CreateTables(Authorization.Program.ConnectionString, dbName);
            ComboBoxTables();
        }
        static void CreateDatabase(string connentionString, string dbName)
        {

            using (MySqlConnection connection = new MySqlConnection(connentionString))
            {
                connection.Open();
                string createDbQuery = $"CREATE DATABASE IF NOT EXISTS {dbName};";
                MySqlCommand command = new MySqlCommand(createDbQuery, connection);
                command.ExecuteNonQuery();
                Console.WriteLine($"Database {dbName} created or already exists.");
            }
        }
        static void CreateTables(string connectionString, string dbName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string createCategoryTable = @"
                    CREATE TABLE IF NOT EXISTS `Category` (
                      `CategoryID` int NOT NULL AUTO_INCREMENT,
                      `CategoryName` varchar(30) NOT NULL,
                         PRIMARY KEY (`CategoryID`));";
                    string createEmployeeTable = @"
                    CREATE TABLE  IF NOT EXISTS `Employee` (
                      `EmployeeID` int NOT NULL AUTO_INCREMENT,
                      `EmployeeFIO` varchar(45) NOT NULL,
                      `telephone` varchar(20) NOT NULL,
                      `pasport` bigint NOT NULL,
                        PRIMARY KEY (`EmployeeID`));";
                    string createOrderTable = @"
                    CREATE TABLE IF NOT EXISTS `Order` (
                      `OrderID` int NOT NULL AUTO_INCREMENT,
                      `OrderDate` datetime NOT NULL,
                      `OrderStatus` varchar(45) NOT NULL,
                      `OrderUser` int NOT NULL,
                      `OrderPrice` int NOT NULL,
                      PRIMARY KEY (`OrderID`),
                      FOREIGN KEY (`OrderUser`) REFERENCES `Employee` (`EmployeeID`));";
                    string createProductTable = @"
                    CREATE TABLE `Product` (
                      `ProductArticul` int NOT NULL,
                      `Name` varchar(45) NOT NULL,
                      `Description` varchar(256) NOT NULL,
                      `Cost` int NOT NULL,
                      `Unit` varchar(20) NOT NULL,
                      `ProductQuantityInStock` int NOT NULL,
                      `ProductCategory` int NOT NULL,
                      `ProductManufactur` int NOT NULL,
                      `ProductSupplier` int NOT NULL,
                      `ProductPhoto` varchar(145) DEFAULT NULL,
                      PRIMARY KEY (`ProductArticul`),
                      FOREIGN KEY (`ProductCategory`) REFERENCES `Category` (`CategoryID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`ProductManufactur`) REFERENCES `ProductManufactur` (`ProductManufacturID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`ProductSupplier`) REFERENCES `Supplier` (`SupplierID`) ON DELETE CASCADE ON UPDATE CASCADE);";
                    string createManufacturerTable = @"
                    CREATE TABLE `ProductManufactur` (
                      `ProductManufacturID` int NOT NULL AUTO_INCREMENT,
                      `ProductManufacturName` varchar(45) NOT NULL,
                      PRIMARY KEY (`ProductManufacturID`));";
                    string createOrderProductTable = @"
                    CREATE TABLE `ProductOrder` (
                      `ProductID` int NOT NULL,
                      `ProductCount` int NOT NULL,
                      `OrderID` int NOT NULL,
                      PRIMARY KEY (`ProductID`,`OrderID`),
                      FOREIGN KEY (`OrderID`) REFERENCES `Order` (`OrderID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`ProductID`) REFERENCES `Product` (`ProductArticul`) ON DELETE CASCADE ON UPDATE CASCADE);";
                    string createRoleTable = @"
                    CREATE TABLE `Role` (
                      `RoleID` int NOT NULL AUTO_INCREMENT,
                      `Role` varchar(45) NOT NULL,
                      PRIMARY KEY (`RoleID`));";
                    string createSupplierTable = @"
                    CREATE TABLE `Supplier` (
                      `SupplierID` int NOT NULL AUTO_INCREMENT,
                      `SupplierName` varchar(25) NOT NULL,
                      PRIMARY KEY (`SupplierID`));";
                    string createUserTable = @"
                    CREATE TABLE `User` (
                      `UserID` int NOT NULL AUTO_INCREMENT,
                      `UserFIO` int NOT NULL,
                      `RoleID` int NOT NULL,
                      `Login` varchar(45) NOT NULL,
                      `Password` varchar(100) NOT NULL,
                      PRIMARY KEY (`UserID`),
                      FOREIGN KEY (`RoleID`) REFERENCES `Role` (`RoleID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`UserFIO`) REFERENCES `Employee` (`EmployeeID`) ON DELETE CASCADE ON UPDATE CASCADE);";

                    MySqlCommand roleCommand = new MySqlCommand(createRoleTable, connection);
                    MySqlCommand employeeCommand = new MySqlCommand(createEmployeeTable, connection);
                    MySqlCommand userCommand = new MySqlCommand(createUserTable, connection);
                    MySqlCommand manufCommand = new MySqlCommand(createManufacturerTable, connection);
                    MySqlCommand supCommand = new MySqlCommand(createSupplierTable, connection);
                    MySqlCommand catCommand = new MySqlCommand(createCategoryTable, connection);
                    MySqlCommand prodCommand = new MySqlCommand(createProductTable, connection);
                    MySqlCommand orderCommand = new MySqlCommand(createOrderTable, connection);
                    MySqlCommand orderproductCommand = new MySqlCommand(createOrderProductTable, connection);

                    roleCommand.ExecuteNonQuery();
                    employeeCommand.ExecuteNonQuery();
                    userCommand.ExecuteNonQuery();
                    manufCommand.ExecuteNonQuery();
                    supCommand.ExecuteNonQuery();
                    catCommand.ExecuteNonQuery();
                    prodCommand.ExecuteNonQuery();
                    orderCommand.ExecuteNonQuery();
                    orderproductCommand.ExecuteNonQuery();
                    MessageBox.Show("База данных восстановлена.");
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка восстановления БД " + ex.Message + Environment.NewLine);
                }
            }
        }

        private void btnSelectCsv_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnImportData_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите таблицу");
                return;
            }
            string tableName = comboBox1.SelectedItem.ToString();

            OpenFileAndImportData(tableName);
        }
        public void OpenFileAndImportData(string selectedTable)
        {
            string filePath = textBox1.Text;
            ImportData(filePath, selectedTable);
        }
        private void ImportData(string filePath, string selectedTable)
        {
            string fullConnectionString = Authorization.Program.ConnectionString;
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Пожалуйста, выберите файл и таблицу.");
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(fullConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var reader = new StreamReader(filePath, Encoding.UTF8))
                            {
                                string line;
                                // Пропускаем заголовок файла (если есть)
                                if ((line = reader.ReadLine()) != null)
                                {
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        var values = line.Split(';');

                                        string insertCommand = GenerateInsertCommand(selectedTable, values);

                                        using (var command = new MySqlCommand(insertCommand, connection, transaction))
                                        {
                                            // Добавляем параметры для предотвращения SQL-инъекций
                                            for (int i = 0; i < values.Length; i++)
                                            {
                                                command.Parameters.AddWithValue($"@param{i}", values[i]);
                                            }
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                            MessageBox.Show("Импорт данных завершен успешно.");
                        }

                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при импорте данных: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }
        private string GenerateInsertCommand(string tableName, string[] values)
        {
            if (tableName == "user" || tableName == "User")
            {
                return $"INSERT INTO user (UserID, UserFIO, RoleID, Login, Password) VALUES (@param0, @param1, @param2, @param3, @param4)";
            }
            else if (tableName == "role" || tableName == "Role")
            {
                return $"INSERT INTO role (RoleID, Role) VALUES (@param0, @param1)";
            }
            if (tableName == "order" || tableName == "Order")
            {
                return $"INSERT INTO `Order` (OrderID, OrderDate, OrderStatus, OrderUser, OrderPrice) VALUES (@param0, @param1, @param2, @param3, @param4)";
            }
            if (tableName == "productorder" || tableName == "ProductOrder")
            {
                return $"INSERT INTO productorder (ProductID, ProductCount, OrderID) VALUES (@param0, @param1, @param2)";
            }
            if (tableName == "product" || tableName == "Product")
            {
                return $"INSERT INTO Product (ProductArticul, Name, Description, Cost, Unit, ProductQuantityInStock, ProductCategory, ProductManufactur, ProductSupplier, ProductPhoto) VALUES (@param0, @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9)";
            }
            else if (tableName == "employee" || tableName == "Employee")
            {
                return $"INSERT INTO employee (EmployeeID, EmployeeFIO, telephone, pasport) VALUES (@param0, @param1,@param2, @param3)";
            }
            else if (tableName == "productmanufactur" || tableName == "ProductManufactur")
            {
                return $"INSERT INTO productmanufactur (ProductManufacturID, ProductManufacturName) VALUES (@param0, @param1)";
            }
            else if (tableName == "supplier" || tableName == "Supplier")
            {
                return $"INSERT INTO Supplier (SupplierID, SupplierName) VALUES (@param0, @param1)";
            }
            else if (tableName == "category" || tableName == "Category")
            {
                return $"INSERT INTO Category (CategoryID, CategoryName) VALUES (@param0, @param1)";
            }
            return "ошмбкаааааааа123";
        }
    }
}
