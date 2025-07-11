﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;

namespace kursovoy
{
    public partial class import : Form
    {
        public import()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Кнопка для восстановления структуры БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
                string dbName = $"db45";

                if (DatabaseExists(Program1.ConnectionStringNotDB, dbName))
                {
                    var result = MessageBox.Show("База данных уже существует. Хотите удалить и восстановить её заново?", "Восстановление БД", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Удаляем базу
                        DropDatabase(Program1.ConnectionStringNotDB, dbName);
                        // Создаём новую
                        CreateDatabase(Program1.ConnectionStringNotDB, dbName);
                        CreateTables(Program1.ConnectionString, dbName);
                    }
                    else
                    {
                        MessageBox.Show("Восстановление базы данных отменено.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    CreateDatabase(Program1.ConnectionStringNotDB, dbName);
                    CreateTables(Program1.ConnectionString, dbName);
                }
        }

        /// <summary>
        /// Проверяет существование базы данных
        /// </summary>
        /// <param name="connectionString">Строка подключения без указания БД</param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        static bool DatabaseExists(string connectionString, string dbName)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string checkDbQuery = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{dbName}';";
                MySqlCommand cmd = new MySqlCommand(checkDbQuery, con);
                object result = cmd.ExecuteScalar();
                return result != null && result.ToString() == dbName;
            }
        }

        /// <summary>
        /// Удаляет базу данных
        /// </summary>
        /// <param name="connectionString">Строка подключения без указания БД</param>
        /// <param name="dbName">Имя базы данных</param>
        static void DropDatabase(string connectionString, string dbName)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                string dropDbQuery = $"DROP DATABASE IF EXISTS {dbName};";
                MySqlCommand cmd = new MySqlCommand(dropDbQuery, con);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Функция создания БД
        /// </summary>
        /// <param name="connentionString"></param>
        /// <param name="dbName"></param>
        static void CreateDatabase(string connentionString, string dbName)
        {
            using (MySqlConnection con = new MySqlConnection(connentionString))
            {
                con.Open();
                string createDbQuery = $"CREATE DATABASE IF NOT EXISTS {dbName};";
                MySqlCommand cmd = new MySqlCommand(createDbQuery, con);
                cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Функция создания необходимых таблиц БД.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbName"></param>
        private void CreateTables(string connectionString, string dbName)
        {
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    if (TableExists(con, "Category"))
                    {
                        MessageBox.Show("Таблица Category уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createCategoryTable = @"
                    CREATE TABLE Category (
                      CategoryID int NOT NULL AUTO_INCREMENT,
                      CategoryName varchar(30) NOT NULL,
                      PRIMARY KEY (CategoryID));";
                        MySqlCommand cmdCategory = new MySqlCommand(createCategoryTable, con);
                        cmdCategory.ExecuteNonQuery();
                    }
                    if (TableExists(con, "Employeeee"))
                    {
                        MessageBox.Show("Таблица Employeeee уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createEmployeeTable = @"
                    CREATE TABLE Employeeee (
                      EmployeeID int NOT NULL AUTO_INCREMENT,
                      EmployeeF varchar(40) NOT NULL,
                      EmployeeI varchar(40) NOT NULL,
                      EmployeeO varchar(40) NOT NULL,
                      telephone varchar(20) NOT NULL,
                      status varchar(10) NOT NULL,
                      PRIMARY KEY (EmployeeID));";
                        MySqlCommand cmdEmployee = new MySqlCommand(createEmployeeTable, con);
                        cmdEmployee.ExecuteNonQuery();
                    }
                    if (TableExists(con, "ProductManufactur"))
                    {
                        MessageBox.Show("Таблица ProductManufactur уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createManufacturerTable = @"
                    CREATE TABLE `ProductManufactur` (
                      `ProductManufacturID` int NOT NULL AUTO_INCREMENT,
                      `ProductManufacturName` varchar(25) NOT NULL,
                      PRIMARY KEY (`ProductManufacturID`));";
                        MySqlCommand manufCommand = new MySqlCommand(createManufacturerTable, con);
                        manufCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "supplier"))
                    {
                        MessageBox.Show("Таблица Supplier уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createSupplierTable = @"
                    CREATE TABLE `Supplier` (
                      `SupplierID` int NOT NULL AUTO_INCREMENT,
                      `SupplierName` varchar(25) NOT NULL,
                      PRIMARY KEY (`SupplierID`));";
                        MySqlCommand suppCommand = new MySqlCommand(createSupplierTable, con);
                        suppCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "Role"))
                    {
                        MessageBox.Show("Таблица Role уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createRoleTable = @"
                    CREATE TABLE `Role` (
                      `RoleID` int NOT NULL AUTO_INCREMENT,
                      `Role` varchar(13) NOT NULL,
                      PRIMARY KEY (`RoleID`));";
                        MySqlCommand roleCommand = new MySqlCommand(createRoleTable, con);
                        roleCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "User"))
                    {
                        MessageBox.Show("Таблица User уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createUserTable = @"
                    CREATE TABLE `User` (
                      `UserID` int NOT NULL AUTO_INCREMENT,
                      `UserFIO` int(4) NOT NULL,
                      `RoleID` int(1) NOT NULL,
                      `Login` varchar(10) NOT NULL,
                      `Password` varchar(100) NOT NULL,
                      PRIMARY KEY (`UserID`),
                      FOREIGN KEY (`RoleID`) REFERENCES `Role` (`RoleID`),
                      FOREIGN KEY (`UserFIO`) REFERENCES `employeeee` (`EmployeeID`));";
                        MySqlCommand userCommand = new MySqlCommand(createUserTable, con);
                        userCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "Product"))
                    {
                        MessageBox.Show("Таблица Product уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createProductTable = @"
                    CREATE TABLE `Product` (
                      `ProductArticul` int NOT NULL,
                      `Name` varchar(45) NOT NULL,
                      `Description` varchar(256) NOT NULL,
                      `Cost` int(5) NOT NULL,
                      `Unit` varchar(20) NOT NULL,
                      `ProductQuantityInStock` int(3) NOT NULL,
                      `ProductCategory` int NOT NULL,
                      `ProductManufactur` int NOT NULL,
                      `ProductSupplier` int NOT NULL,
                      `ProductPhoto` varchar(145) DEFAULT NULL,
                      PRIMARY KEY (`ProductArticul`),
                      FOREIGN KEY (`ProductCategory`) REFERENCES `Category` (`CategoryID`),
                      FOREIGN KEY (`ProductManufactur`) REFERENCES `ProductManufactur` (`ProductManufacturID`),
                      FOREIGN KEY (`ProductSupplier`) REFERENCES `Supplier` (`SupplierID`));";
                        MySqlCommand prodCommand = new MySqlCommand(createProductTable, con);
                        prodCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "`Order`"))
                    {
                        MessageBox.Show("Таблица Order уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createOrderTable = @"
                    CREATE TABLE IF NOT EXISTS `Order` (
                      `OrderID` int NOT NULL AUTO_INCREMENT,
                      `OrderDate` datetime NOT NULL,
                      `OrderStatus` varchar(45) NOT NULL,
                      `OrderUser` int NOT NULL,
                      `OrderPrice` DOUBLE NOT NULL,
                      PRIMARY KEY (`OrderID`),
                      FOREIGN KEY (`OrderUser`) REFERENCES `employeeee` (`EmployeeID`));";
                        MySqlCommand orderCommand = new MySqlCommand(createOrderTable, con);
                        orderCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "ProductOrder"))
                    {
                        MessageBox.Show("Таблица ProductOrder уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createOrderProductTable = @"
                    CREATE TABLE `ProductOrder` (
                      `ProductID` int(9) NOT NULL,
                      `ProductCount` int(2) NOT NULL,
                      `OrderID` int NOT NULL,
                      PRIMARY KEY (`ProductID`,`OrderID`),
                      FOREIGN KEY (`OrderID`) REFERENCES `Order` (`OrderID`),
                      FOREIGN KEY (`ProductID`) REFERENCES `Product` (`ProductArticul`));";
                        MySqlCommand orderproductCommand = new MySqlCommand(createOrderProductTable, con);
                        orderproductCommand.ExecuteNonQuery();
                    }
                    if (TableExists(con, "Companyinfo"))
                    {
                        MessageBox.Show("Таблица Companyinfo уже существует.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string createCompanyTable = @"CREATE TABLE Companyinfo (
                      idcompanyinfo int NOT NULL AUTO_INCREMENT,
                      company_name varchar(30) NOT NULL,
                      company_adress varchar(45) NOT NULL,
                      company_phone varchar(16) NOT NULL,
                      company_inn varchar(12) NOT NULL,
                      company_ogrn varchar(13) NOT NULL,
                      company_bick varchar(9) NOT NULL,
                      company_ip varchar(29) NOT NULL,
                      PRIMARY KEY (idcompanyinfo)
                    )";
                        MySqlCommand companyCommand = new MySqlCommand(createCompanyTable, con);
                        companyCommand.ExecuteNonQuery();
                    }
                    MessageBox.Show("База данных восстановлена.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка восстановления БД " + ex.Message + Environment.NewLine);
                }
            }
        }
        /// <summary>
        /// Метод для проверки существования таблицы
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TableExists(MySqlConnection con, string tableName)
        {
            string query = $"SHOW TABLES LIKE '{tableName}';";
            MySqlCommand cmd = new MySqlCommand(query, con);
            var result = cmd.ExecuteScalar();
            return result != null;
        }
        /// <summary>
        /// Кнопка для выбора файла для импорта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Кнопка для импорта данных в БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportData_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите таблицу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                string tableName = comboBox1.SelectedItem.ToString();
                string filePath = textBox1.Text;
                ImportData(filePath, tableName);
            }
            catch (MySqlException)
            {
                MessageBox.Show("Базы данных не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        /// <summary>
        /// Импорт данных в БД
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="selectedTable"></param>
        private void ImportData(string filePath, string selectedTable)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Пожалуйста, выберите файл и таблицу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            try
            {
                using (var con = new MySqlConnection(Program1.ConnectionString))
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            using (var reader = new StreamReader(filePath, Encoding.UTF8))
                            {
                                string line;
                                if ((line = reader.ReadLine()) != null)
                                {
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        var values = line.Split(';');

                                        string insertCommand = GenerateInsertCommand(selectedTable, values);

                                        using (var command = new MySqlCommand(insertCommand, con, transaction))
                                        {
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
                            MessageBox.Show("Импорт данных завершен успешно.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при импорте данных: {ex.Message}");
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("Базы данных не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }
        /// <summary>
        /// Для генерации SQL-запроса для вставки данных в разные таблицы базы данных.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
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
                return $"INSERT INTO `order` (OrderID, OrderDate, OrderStatus, OrderUser, OrderPrice) VALUES (@param0, @param1, @param2, @param3, @param4)";
            }
            if (tableName == "productorder" || tableName == "ProductOrder")
            {
                return $"INSERT INTO productorder (ProductID, ProductCount, OrderID) VALUES (@param0, @param1, @param2)";
            }
            if (tableName == "product" || tableName == "Product")
            {
                return $"INSERT INTO product (ProductArticul, Name, Description, Cost, Unit, ProductQuantityInStock, ProductCategory, ProductManufactur, ProductSupplier, ProductPhoto) VALUES (@param0, @param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9)";
            }
            else if (tableName == "employeeee" || tableName == "Employeeee")
            {
                return $"INSERT INTO employeeee (EmployeeID, EmployeeF, EmployeeI, EmployeeO, telephone, status) VALUES (@param0, @param1,@param2, @param3, @param4, @param5)";
            }
            else if (tableName == "productmanufactur" || tableName == "ProductManufactur")
            {
                return $"INSERT INTO productmanufactur (ProductManufacturID, ProductManufacturName) VALUES (@param0, @param1)";
            }
            else if (tableName == "supplier" || tableName == "Supplier")
            {
                return $"INSERT INTO supplier (SupplierID, SupplierName) VALUES (@param0, @param1)";
            }
            else if (tableName == "category" || tableName == "Category")
            {
                return $"INSERT INTO category (CategoryID, CategoryName) VALUES (@param0, @param1)";
            }
            else if (tableName == "companyinfo" || tableName == "Companyinfo")
            {
                return $"INSERT INTO companyinfo (idcompanyinfo, company_name, company_adress, company_phone, company_inn, company_ogrn, company_bick, company_ip) VALUES (@param0, @param1, @param2, @param3, @param4, @param5, @param6, @param7)";
            }
            return "ошибкаааааа";
        }
        /// <summary>
        /// Кнопка для резервного копирования БД 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            AutomaticBackup();
        }
        /// <summary>
        /// Функция резервного копирования БД.
        /// </summary>
        public static void AutomaticBackup()
        {
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string backupsDir = Path.Combine(appDir, "ReservCopy"); 
                if (!Directory.Exists(backupsDir))
                {
                    Directory.CreateDirectory(backupsDir);
                }
                string backupFileName = $"PetsShop_backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                string backupFilePath = Path.Combine(backupsDir, backupFileName);
                try
                {
                    BackupDatabaseToSql(backupFilePath);
                    MessageBox.Show("Создана резервная копия!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1049)
                    {
                        MessageBox.Show($"База данных не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка: " + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: " + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка автоматического резервного копирования: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        /// <summary>
        /// Создание скрипта резервной копии
        /// </summary>
        /// <param name="filePath"></param>
        public static void BackupDatabaseToSql(string filePath)
        {
            using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
            {
                connection.Open();

                using (StreamWriter writer = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    writer.WriteLine("-- Резервная копия базы данных db45");
                    writer.WriteLine($"-- Создана: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine();
                    writer.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
                    writer.WriteLine();

                    List<string> tables = new List<string>();
                    using (MySqlCommand cmd = new MySqlCommand("SHOW TABLES", connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }

                    writer.WriteLine($"CREATE DATABASE IF NOT EXISTS db45 CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");
                    writer.WriteLine($"USE db45;");
                    writer.WriteLine();

                    foreach (string table in tables)
                    {
                        // Записываем структуру таблицы
                        writer.WriteLine($"-- Структура таблицы {table}");
                        writer.WriteLine($"DROP TABLE IF EXISTS `{table}`;");
                        using (MySqlCommand cmd = new MySqlCommand($"SHOW CREATE TABLE `{table}`", connection))
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                writer.WriteLine(reader.GetString("Create Table") + ";");
                            }
                        }
                        writer.WriteLine();

                        // Выполняем блокировку таблицы
                        using (MySqlCommand lockCmd = new MySqlCommand($"LOCK TABLES `{table}` WRITE;", connection))
                        {
                            lockCmd.ExecuteNonQuery();
                        }

                        // Записываем дамп данных таблицы
                        writer.WriteLine($"-- Дамп данных таблицы {table}");
                        writer.WriteLine($"/*!40000 ALTER TABLE `{table}` DISABLE KEYS */;");

                        using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM `{table}`", connection))
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                StringBuilder insertQuery = new StringBuilder($"INSERT INTO `{table}` VALUES (");
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (i > 0) insertQuery.Append(", ");

                                    string rawValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();

                                    // Обработка дат
                                    string formattedValue = TryParseAndFormatDate(rawValue);

                                    // Обработка чисел
                                    if (formattedValue != null)
                                    {
                                        formattedValue = FixNumberFormat(formattedValue);
                                    }

                                    if (rawValue == null)
                                    {
                                        insertQuery.Append("NULL");
                                    }
                                    else
                                    {
                                        insertQuery.Append($"'{formattedValue.Replace("'", "''")}'");
                                    }
                                }
                                insertQuery.Append(");");
                                writer.WriteLine(insertQuery.ToString());
                            }
                        }
                        // Разблокируем таблицу
                        using (MySqlCommand unlockCmd = new MySqlCommand($"UNLOCK TABLES;", connection))
                        {
                            unlockCmd.ExecuteNonQuery();
                        }
                        writer.WriteLine(); // разделитель между таблицами
                    }
                }
            }
        }
        /// <summary>
        /// Функция для преобразования числа
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FixNumberFormat(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Заменяем запятые на точки
            string corrected = value.Replace(',', '.');

            // Проверяем, что это число
            if (corrected.Split('.').Length <= 2 &&
                corrected.All(c => char.IsDigit(c) || c == '.' || c == '-'))
            {
                // Удаляем возможные лишние минусы и точки
                if (corrected.Count(c => c == '-') > 1 ||
                    corrected.Count(c => c == '.') > 1)
                {
                    return corrected;
                }
                // Пробуем разобрать число
                if (double.TryParse(corrected, out double temp))
                {
                    return temp.ToString().Replace(',', '.');
                }
            }
            return corrected;
        }
        /// <summary>
        /// Функция для преобразования даты при создании резервной копии
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TryParseAndFormatDate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            string[] formats = { "dd.MM.yyyy H:mm:ss", "dd.MM.yyyy HH:mm:ss" };

            if (DateTime.TryParseExact(value, formats, null, DateTimeStyles.None, out DateTime dt))
            {
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return value;
        }
        /// <summary>
        /// Восстановление БД
        /// </summary>
        public void RestoreDatabaseFromDump()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReservCopy");
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = folderPath,
                Filter = "SQL файлы (*.sql)|*.sql",
                Title = "Выберите дамп для восстановления"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string dumpFilePath = openFileDialog.FileName;
                try
                {
                    string sqlScript = File.ReadAllText(dumpFilePath);
                    using (MySqlConnection con = new MySqlConnection(Program1.ConnectionString))
                    {
                        con.Open();
                        MySqlScript script = new MySqlScript(con, sqlScript);
                        script.Execute();
                        MessageBox.Show("Восстановление завершено успешно!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1049)
                    {
                        MessageBox.Show($"База данных не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка: " + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: " + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        /// <summary>
        /// Кнопка выход из учетной записи.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Authorization ad = new Authorization();
            ad.Show();
            this.Close();
        }
        /// <summary>
        /// Кнопка для восстановления БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            RestoreDatabaseFromDump();
        }
        /// <summary>
        /// Кнопка для экспорта данных в БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            BackupAllTables();
        }
        /// <summary>
        /// Создает резервные копии всех указанных таблиц в базе данных.
        /// </summary>
        private void BackupAllTables()
        {
            try
            {
                // Проверяем существование базы данных
                if (!DatabaseExists())
                {
                    MessageBox.Show("База данных не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string[] tables = new string[]
                {
                "category",
                "companyinfo",
                "employeeee",
                "order",
                "product",
                "productmanufactur",
                "productorder",
                "role",
                "supplier",
                "user"
                };
                bool allTablesEmpty = true;
                bool hasErrors = false;

                foreach (var table in tables)
                {
                    // Проверяем существование и заполненность таблицы
                    if (TableExists(table))
                    {
                        if (TableHasData(table))
                        {
                            allTablesEmpty = false;
                            BackupTable(table);
                        }
                        else
                        {
                            MessageBox.Show($"Таблица '{table}' пуста и не будет экспортирована", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Таблица '{table}' не существует и не будет экспортирована", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        hasErrors = true;
                    }
                }
                if (allTablesEmpty)
                {
                    MessageBox.Show("Все таблицы пусты, резервная копия не создана", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (!hasErrors)
                {
                    MessageBox.Show($"Резервная копия всех таблиц успешно создана", "Резервное копирование", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        /// <summary>
        /// Проверяет, существует ли база данных.
        /// </summary>
        /// <returns></returns>
        private bool DatabaseExists()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
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
        /// Проверяет, существует ли таблица в базе данных.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TableExists(string tableName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT 1 FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = '{tableName}' LIMIT 1;";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    return command.ExecuteScalar() != null;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Проверяет, содержит ли таблица данные.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TableHasData(string tableName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT EXISTS (SELECT 1 FROM `{tableName}` LIMIT 1);";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    return Convert.ToInt32(command.ExecuteScalar()) == 1;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Создает резервную копию указанной таблицы в формате CSV.
        /// </summary>
        /// <param name="tableName"></param>
        private void BackupTable(string tableName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Program1.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM `{tableName}`";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    // Создаем директорию, если ее нет
                    Directory.CreateDirectory("./ReservCopy/");

                    string backupFilePath = Path.Combine("./ReservCopy/", $"{tableName}_backup_{DateTime.Now:yyyyMMddHHmmss}.csv");

                    using (StreamWriter writer = new StreamWriter(backupFilePath, false, Encoding.UTF8))
                    {
                        // Записываем заголовки столбцов
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            writer.Write(EscapeCsvField(dataTable.Columns[i].ColumnName));  // Экранируем имя столбца
                            if (i < dataTable.Columns.Count - 1)
                            {
                                writer.Write(";");
                            }
                        }
                        writer.WriteLine();

                        // Записываем строки данных
                        foreach (DataRow row in dataTable.Rows)
                        {
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                string cellValue = row[i] == DBNull.Value ? null : row[i].ToString();

                                if (tableName.ToLower() == "order" && dataTable.Columns[i].ColumnName.ToLower() == "orderdate")
                                {
                                    cellValue = TryParseAndFormatDate(cellValue);
                                }
                                if (tableName.ToLower() == "order" && dataTable.Columns[i].ColumnName.ToLower() == "orderprice")
                                {
                                    cellValue = FixNumberFormat(cellValue);
                                }
                                if (cellValue == null)
                                {
                                    writer.Write(""); 
                                }
                                else
                                {
                                    writer.Write(EscapeCsvField(cellValue));
                                }
                                if (i < dataTable.Columns.Count - 1)
                                {
                                    writer.Write(";");
                                }
                            }
                            writer.WriteLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при резервном копировании таблицы '{tableName}': {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Экранирует поле для записи в CSV файл
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "";
            }
            if (field.Contains(";") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                field = field.Replace("\"", "\"\"");
                return "\"" + field + "\"";
            }
            return field;
        }

        private void import_Load(object sender, EventArgs e)
        {

        }
    }
}

