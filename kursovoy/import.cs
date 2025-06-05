
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
using System.Diagnostics;
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
        /// Заполнение ComboBox названиями таблиц
        /// </summary>
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
        /// <summary>
        /// Кнопка для восстановления структуры БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
            string dbName = $"db45";
            CreateDatabase(Authorization.Program.ConnectionStringNotDB, dbName);
            CreateTables(Authorization.Program.ConnectionString, dbName);
        }
        /// <summary>
        /// Функция создания БД
        /// </summary>
        /// <param name="connentionString"></param>
        /// <param name="dbName"></param>
        static void CreateDatabase(string connentionString, string dbName)
        {
            using (MySqlConnection connection = new MySqlConnection(connentionString))
            {
                connection.Open();
                string createDbQuery = $"CREATE DATABASE IF NOT EXISTS {dbName};";
                MySqlCommand command = new MySqlCommand(createDbQuery, connection);
                command.ExecuteNonQuery();
               /// MessageBox.Show($"База данных {dbName} создана или уже существует.");
            }
        }
        /// <summary>
        /// Функция создания необходимых таблиц БД.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbName"></param>
        private void CreateTables(string connectionString, string dbName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    if (TableExists(connection, "Category"))
                    {
                        MessageBox.Show("Таблица Category уже существует.");
                    }
                    else
                    {
                        string createCategoryTable = @"
                    CREATE TABLE Category (
                      CategoryID int NOT NULL AUTO_INCREMENT,
                      CategoryName varchar(30) NOT NULL,
                      PRIMARY KEY (CategoryID));";
                        MySqlCommand cmd = new MySqlCommand(createCategoryTable, connection);
                        cmd.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "Employeeee"))
                    {
                        MessageBox.Show("Таблица Employeeee уже существует.");
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
                        MySqlCommand cmd = new MySqlCommand(createEmployeeTable, connection);
                        cmd.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "ProductManufactur"))
                    {
                        MessageBox.Show("Таблица ProductManufactur уже существует.");
                    }
                    else
                    {
                        string createManufacturerTable = @"
                    CREATE TABLE `ProductManufactur` (
                      `ProductManufacturID` int NOT NULL AUTO_INCREMENT,
                      `ProductManufacturName` varchar(25) NOT NULL,
                      PRIMARY KEY (`ProductManufacturID`));";
                        MySqlCommand manufCommand = new MySqlCommand(createManufacturerTable, connection);
                        manufCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "supplier"))
                    {
                        MessageBox.Show("Таблица Supplier уже существует.");
                    }
                    else
                    {
                        string createSupplierTable = @"
                    CREATE TABLE `Supplier` (
                      `SupplierID` int NOT NULL AUTO_INCREMENT,
                      `SupplierName` varchar(25) NOT NULL,
                      PRIMARY KEY (`SupplierID`));";
                        MySqlCommand suppCommand = new MySqlCommand(createSupplierTable, connection);
                        suppCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "Role"))
                    {
                        MessageBox.Show("Таблица Role уже существует.");
                    }
                    else
                    {
                        string createRoleTable = @"
                    CREATE TABLE `Role` (
                      `RoleID` int NOT NULL AUTO_INCREMENT,
                      `Role` varchar(13) NOT NULL,
                      PRIMARY KEY (`RoleID`));";
                        MySqlCommand roleCommand = new MySqlCommand(createRoleTable, connection);
                        roleCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "User"))
                    {
                        MessageBox.Show("Таблица User уже существует.");
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
                      FOREIGN KEY (`RoleID`) REFERENCES `Role` (`RoleID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`UserFIO`) REFERENCES `employeeee` (`EmployeeID`) ON DELETE CASCADE ON UPDATE CASCADE);";
                        MySqlCommand userCommand = new MySqlCommand(createUserTable, connection);
                        userCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "Product"))
                    {
                        MessageBox.Show("Таблица Product уже существует.");
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
                      FOREIGN KEY (`ProductCategory`) REFERENCES `Category` (`CategoryID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`ProductManufactur`) REFERENCES `ProductManufactur` (`ProductManufacturID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`ProductSupplier`) REFERENCES `Supplier` (`SupplierID`) ON DELETE CASCADE ON UPDATE CASCADE);";
                        MySqlCommand prodCommand = new MySqlCommand(createProductTable, connection);
                        prodCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "`Order`"))
                    {
                        MessageBox.Show("Таблица Order уже существует.");
                    }
                    else
                    {
                        string createOrderTable = @"
                    CREATE TABLE IF NOT EXISTS `Order` (
                      `OrderID` int NOT NULL AUTO_INCREMENT,
                      `OrderDate` datetime(4) NOT NULL,
                      `OrderStatus` varchar(45) NOT NULL,
                      `OrderUser` int NOT NULL,
                      `OrderPrice` DOUBLE NOT NULL,
                      PRIMARY KEY (`OrderID`),
                      FOREIGN KEY (`OrderUser`) REFERENCES `employeeee` (`EmployeeID`));";
                        MySqlCommand orderCommand = new MySqlCommand(createOrderTable, connection);
                        orderCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "ProductOrder"))
                    {
                        MessageBox.Show("Таблица ProductOrder уже существует.");
                    }
                    else
                    {
                        string createOrderProductTable = @"
                    CREATE TABLE `ProductOrder` (
                      `ProductID` int(9) NOT NULL,
                      `ProductCount` int(2) NOT NULL,
                      `OrderID` int NOT NULL,
                      PRIMARY KEY (`ProductID`,`OrderID`),
                      FOREIGN KEY (`OrderID`) REFERENCES `Order` (`OrderID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      FOREIGN KEY (`ProductID`) REFERENCES `Product` (`ProductArticul`) ON DELETE CASCADE ON UPDATE CASCADE);";
                        MySqlCommand orderproductCommand = new MySqlCommand(createOrderProductTable, connection);
                        orderproductCommand.ExecuteNonQuery();
                    }

                    if (TableExists(connection, "Companyinfo"))
                    {
                        MessageBox.Show("Таблица Companyinfo уже существует.");
                    }
                    else
                    {
                        string createCompanyTable = @"CREATE TABLE Companyinfo (
                      idcompanyinfo int NOT NULL AUTO_INCREMENT,
                      company_name varchar(24) NOT NULL,
                      company_adress varchar(45) NOT NULL,
                      company_phone varchar(16) NOT NULL,
                      company_inn varchar(12) NOT NULL,
                      company_ogrn varchar(13) NOT NULL,
                      company_bick varchar(9) NOT NULL,
                      company_ip varchar(29) NOT NULL,
                      PRIMARY KEY (idcompanyinfo)
                    )";
                        MySqlCommand companyCommand = new MySqlCommand(createCompanyTable, connection);
                        companyCommand.ExecuteNonQuery();
                        ComboBoxTables();
                    }

                    MessageBox.Show("База данных восстановлена.");

                    connection.Close();
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
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool TableExists(MySqlConnection connection, string tableName)
        {
            string query = $"SHOW TABLES LIKE '{tableName}';";
            MySqlCommand cmd = new MySqlCommand(query, connection);
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
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите таблицу");
                return;
            }
            string tableName = comboBox1.SelectedItem.ToString();

            string filePath = textBox1.Text;
            ImportData(filePath, tableName);
        }

        /// <summary>
        /// Импорт данных в БД
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="selectedTable"></param>
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
                string backupsDir = Path.Combine(appDir, "ReservCopy"); // папка ReservCopy

                if (!Directory.Exists(backupsDir))
                {
                    Directory.CreateDirectory(backupsDir);
                }

                string backupFileName = $"PetsShop_backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                string backupFilePath = Path.Combine(backupsDir, backupFileName);

                try
                {
                    BackupDatabaseToSql(backupFilePath);
                    MessageBox.Show("Создана резервная копия!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1049) // Error 1049 - Unknown database
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
            using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
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
            double temp;
            if (double.TryParse(corrected, NumberStyles.Any, CultureInfo.InvariantCulture, out temp))
            {
                return temp.ToString(CultureInfo.InvariantCulture);
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

            DateTime dt;

            // Попытка парсинга с форматом 'dd.MM.yyyy H:mm:ss' (один или два символа для часа)
            string[] formats = { "dd.MM.yyyy H:mm:ss", "dd.MM.yyyy HH:mm:ss" };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }

            // Можно добавить дополнительные форматы, если есть
            return value;
        }

        /// <summary>
        /// Восстановление БД
        /// </summary>
        public void RestoreDatabaseFromDump()
        {
            // Путь к папке с дампами
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReservCopy");

            // Открываем диалог выбора файла
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
                    // Читаем содержимое файла
                    string sqlScript = File.ReadAllText(dumpFilePath);

                    using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                    {
                        connection.Open();

                        // Создаем объект MySqlScript для выполнения скрипта
                        MySqlScript script = new MySqlScript(connection, sqlScript);
                        script.Execute();

                        MessageBox.Show("Восстановление завершено успешно!", "Успех");
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1049) // Error 1049 - Unknown database
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

        ////Метод проверки существования таблиц
        //private bool AreAllTablesPresent(string[] tableNames)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            foreach (string tableName in tableNames)
        //            {
        //                string query = $"SELECT 1 FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = '{tableName}'";
        //                MySqlCommand command = new MySqlCommand(query, connection);
        //                object result = command.ExecuteScalar();

        //                if (result == null) // Таблица не найдена
        //                {
        //                    return false;
        //                }
        //            }
        //            return true; // Все таблицы найдены
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Произошла ошибка при проверке существования таблиц: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return false; // Предполагаем, что таблица не существует в случае ошибки
        //        }
        //    }
        //}

        ////Метод проверки заполненности таблиц
        //private bool AreAllTablesPopulated(string[] tableNames)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            foreach (string tableName in tableNames)
        //            {
        //                string query = $"SELECT COUNT(*) FROM `{tableName}`"; // Обратите внимание на обратные кавычки
        //                MySqlCommand command = new MySqlCommand(query, connection);
        //                long count = (long)command.ExecuteScalar();

        //                if (count == 0) // Таблица пуста
        //                {
        //                    return false;
        //                }
        //            }
        //            return true; // Все таблицы не пусты
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Произошла ошибка при проверке заполненности таблиц: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return false; // Предполагаем, что таблица пуста в случае ошибки
        //        }
        //    }
        //}

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

        private void import_Load(object sender, EventArgs e)
        {

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
        /// Экспорт данных в таблицы БД.
        /// </summary>
        private void BackupAllTables()
        {
            string[] tables = new string[]
            {
           "category",
            "companyinfo",
            "employeeee",
            "`order`",
            "product",
            "productmanufactur",
            "productorder",
            "role",
            "supplier",
            "user"
            };

            foreach (var table in tables)
            {
                BackupTable(table);
            }
            MessageBox.Show($"Резервная копия всех таблиц успешно создана", "Резервное копирование", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BackupTable(string tableName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Authorization.Program.ConnectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM {tableName}";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    string backupFilePath = Path.Combine("./ReservCopy/", $"{tableName}_backup_{DateTime.Now:yyyyMMddHHmmss}.csv");

                    using (StreamWriter writer = new StreamWriter(backupFilePath))
                    {
                        // Записываем заголовки столбцов
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            writer.Write(dataTable.Columns[i]);
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
                                writer.Write(row[i]);
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
    }
}
