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
namespace kursovoy
{
    public partial class import : Form
    {
        public import()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Authorization a = new Authorization();
            this.Hide();
            a.Show();
        }

        private void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
            string dbName = $"db45";
            CreateDatabase(Authorization.Program.ConnectionString, dbName);
            CreateTables(Authorization.Program.ConnectionString, dbName);
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
            string fullConnectionString = $"{connectionString};";
            //database={dbName}
            using (MySqlConnection connection = new MySqlConnection(fullConnectionString))
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
                      KEY `Role_idx` (`RoleID`),
                      KEY `userrrr_idx` (`UserFIO`),
                      CONSTRAINT `Role` FOREIGN KEY (`RoleID`) REFERENCES `Role` (`RoleID`) ON DELETE CASCADE ON UPDATE CASCADE,
                      CONSTRAINT `userrrr` FOREIGN KEY (`UserFIO`) REFERENCES `Employee` (`EmployeeID`) ON DELETE CASCADE ON UPDATE CASCADE);";

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
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка восстановления БД " + ex.Message + Environment.NewLine);
                }
            }
        }
    }
}
