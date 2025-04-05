CREATE DATABASE  IF NOT EXISTS `db45` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db45`;
-- MySQL dump 10.13  Distrib 8.0.28, for Win64 (x86_64)
--
-- Host: 10.207.106.12    Database: db45
-- ------------------------------------------------------
-- Server version	8.0.31

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `CategoryID` int NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(30) NOT NULL,
  PRIMARY KEY (`CategoryID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (1,'Корм сухой '),(2,'Игрушки для кошек и собак'),(3,'Лежанка для кошек и собак'),(4,'Корм влажный '),(5,'Переноска для кошек и собак'),(6,'Миска для кошек и собак'),(7,'Витамины и добавки'),(8,'Ошейник для собак'),(9,'Лакомства для кошек и собак'),(10,'Поводок для собак'),(11,'Шлейка для кошек'),(12,'Когтеточка для кошек'),(13,'Лосьон для животных'),(14,'Пеленка для кошек и собак'),(15,'Клетка для животных');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee` (
  `EmployeeID` int NOT NULL AUTO_INCREMENT,
  `EmployeeFIO` varchar(45) NOT NULL,
  `telephone` varchar(20) NOT NULL,
  `pasport` bigint NOT NULL,
  PRIMARY KEY (`EmployeeID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

LOCK TABLES `employee` WRITE;
/*!40000 ALTER TABLE `employee` DISABLE KEYS */;
INSERT INTO `employee` VALUES (1,' Иванов Иван Иванович ',' +7920123-87-67 ',2346342334),(2,' Петрова Мария Николаевна ',' +7999234-56-78 ',2222233225),(3,' Сидоров Алексей Андреевич ',' +7909345-67-89 ',2222789012),(4,' Смирнова Анна Валерьевна ',' +7980456-78-90 ',2222567890),(5,' Васильев Константин Олегович ',' +7999567-22-01 ',2222901234),(6,' Морозова Анна Сергеевна ',' +7920789-01-23 ',2222737643),(7,' Тимофеева Арина Витальевна ',' +7921889-01-23 ',2222763674),(8,' Алешина Александра Алексеевна ',' +7919678-90-12 ',2200732786),(10,' Кузнецова Мария Алексеевна ',' +7915234-56-78 ',2002876542),(11,' Орлова Инна Николаевна ',' +7919456-78-90 ',2200543211),(12,' Федорова Елена Андреевна ',' +7920789-01-26 ',2020762651),(13,'Админ','+7(222)222-22-22',2222222222),(14,'Менеджерррр','+7(645)646-46-22',4444444444),(15,'Селллер','+7(377)777-77-77',3333333333);
/*!40000 ALTER TABLE `employee` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `employeeee`
--

DROP TABLE IF EXISTS `employeeee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employeeee` (
  `EmployeeID` int NOT NULL AUTO_INCREMENT,
  `EmployeeF` varchar(45) NOT NULL,
  `EmployeeI` varchar(45) NOT NULL,
  `EmployeeO` varchar(45) NOT NULL,
  `telephone` varchar(20) NOT NULL,
  `pasport` bigint NOT NULL,
  PRIMARY KEY (`EmployeeID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employeeee`
--

LOCK TABLES `employeeee` WRITE;
/*!40000 ALTER TABLE `employeeee` DISABLE KEYS */;
INSERT INTO `employeeee` VALUES (1,'Иванов','Иван','Иванович','+7(454)545-45-56',45345345345),(2,'Семенюк','Владимир','Сергеевич','+7(879)697-69-56',45645645654),(3,'Карпов','Дмитрий','Александрович','+7(987)698-75-99',2233232342);
/*!40000 ALTER TABLE `employeeee` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order`
--

DROP TABLE IF EXISTS `order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order` (
  `OrderID` int NOT NULL AUTO_INCREMENT,
  `OrderDate` datetime NOT NULL,
  `OrderStatus` varchar(45) NOT NULL,
  `OrderUser` int NOT NULL,
  `OrderPrice` int NOT NULL,
  PRIMARY KEY (`OrderID`),
  KEY `OrderUser` (`OrderUser`),
  CONSTRAINT `order_ibfk_1` FOREIGN KEY (`OrderUser`) REFERENCES `employee` (`EmployeeID`)
) ENGINE=InnoDB AUTO_INCREMENT=89 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order`
--

LOCK TABLES `order` WRITE;
/*!40000 ALTER TABLE `order` DISABLE KEYS */;
INSERT INTO `order` VALUES (39,'2025-03-12 17:00:46','Завершен',10,2280),(40,'2025-03-13 22:29:24','Завершен',12,1730),(41,'2025-03-13 22:30:46','Отменён',12,4412),(42,'2025-03-13 22:38:23','Отменён',12,346),(43,'2025-03-13 22:48:23','Завершен',12,346),(44,'2025-03-13 22:53:35','Отменён',12,346),(45,'2025-03-13 22:55:46','Завершен',12,269),(46,'2025-03-13 22:56:50','Завершен',12,269),(47,'2025-03-13 22:58:22','Завершен',12,269),(48,'2025-03-13 23:02:13','Отменён',12,269),(49,'2025-03-13 23:13:02','Завершен',12,269),(50,'2025-03-13 23:15:51','Завершен',12,441),(51,'2025-03-25 21:56:48','Завершен',12,172),(52,'2025-03-25 21:58:11','Завершен',12,172),(53,'2025-03-25 21:59:13','Завершен',12,172),(54,'2025-03-25 22:00:24','Завершен',12,172),(55,'2025-03-25 22:01:33','Завершен',12,172),(56,'2025-03-25 22:05:01','Завершен',12,393),(57,'2025-03-26 19:47:59','Завершен',12,346),(58,'2025-03-26 19:55:29','Завершен',12,269),(59,'2025-03-26 21:20:43','Отменён',12,681),(60,'2025-03-27 17:47:37','Завершен',11,3298),(61,'2025-03-28 08:56:56','Завершен',11,227),(62,'2025-03-28 08:59:48','Завершен',11,227),(63,'2025-03-28 09:02:52','Завершен',11,227),(64,'2025-03-28 17:35:38','Завершен',11,269),(65,'2025-03-28 17:57:07','Завершен',11,269),(66,'2025-03-28 18:07:12','Завершен',11,227),(67,'2025-03-29 16:15:41','Завершен',11,884),(68,'2025-03-29 16:18:19','Завершен',11,346),(69,'2025-03-29 17:12:10','Завершен',11,269),(70,'2025-03-29 17:23:35','Завершен',11,227),(71,'2025-03-29 17:25:14','Завершен',11,227),(72,'2025-03-29 17:26:33','Отменён',11,221),(73,'2025-03-29 17:26:44','Завершен',11,221),(74,'2025-03-29 17:26:48','Завершен',11,221),(75,'2025-03-29 18:24:32','Завершен',11,269),(76,'2025-03-29 18:52:58','Завершен',11,269),(77,'2025-03-29 20:30:16','Завершен',11,227),(78,'2025-03-29 20:34:53','Завершен',11,2474),(79,'2025-03-29 20:37:16','Завершен',11,269),(80,'2025-03-29 20:38:42','Отменён',11,1804),(81,'2025-03-29 20:53:43','Завершен',11,269),(82,'2025-03-29 21:02:07','Завершен',11,172),(83,'2025-03-29 21:22:12','Завершен',11,221),(84,'2025-03-30 19:05:17','Завершен',11,227),(85,'2025-03-30 19:12:38','Завершен',11,2281),(86,'2025-03-30 19:48:16','Завершен',11,269),(87,'2025-03-30 21:34:06','Завершен',11,269),(88,'2025-03-31 09:34:26','Завершен',11,11688);
/*!40000 ALTER TABLE `order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `product` (
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
  KEY `ProductCategory` (`ProductCategory`),
  KEY `ProductManufactur` (`ProductManufactur`),
  KEY `ProductSupplier` (`ProductSupplier`),
  CONSTRAINT `product_ibfk_1` FOREIGN KEY (`ProductCategory`) REFERENCES `category` (`CategoryID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `product_ibfk_2` FOREIGN KEY (`ProductManufactur`) REFERENCES `productmanufactur` (`ProductManufacturID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `product_ibfk_3` FOREIGN KEY (`ProductSupplier`) REFERENCES `supplier` (`SupplierID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `product`
--

LOCK TABLES `product` WRITE;
/*!40000 ALTER TABLE `product` DISABLE KEYS */;
INSERT INTO `product` VALUES (21684104,'Лосьон для чистки ушей носа для собак и кошек','Средство безопасно очищает ушные раковины и нос, бережно удаляет корочки, серу, выделения. ',427,'100мл',23,13,6,5,'21684104.png'),(34971168,'Миски для собак на подставке','Подставка для мисок для собак DOGGY S является лучшим решением для вашего питомца. ',1409,'1750 мл',17,6,16,14,'34971168.png'),(41359273,'Корм для собак сухой с курицей Мистер Баффало','Корм для собак сухой для средних и крупных пород Mr. Buffalo ADULT с курицей премиум. Сухой корм для собак Мистер Баффало содержит оптимальное количество белков, жиров и углеводов, что делает корм для собак подходящим для средних и крупных пород. ',4436,'14 кг',32,1,2,1,'41359273.png'),(41457555,'Корм для кошек стерилизованных сухой','для стерилизованных кошек и кастрированных котов, для кошек',346,'400г',28,1,2,2,'41457555.png'),(76289903,'Когтеточка для кошки','Размер основания 40*30 см, размер верхней лежанки 40*30 см, общая высота 55 см. Высота столбика 50 см. Основание джутового столбика - прочный сосновый брусок. Материал лежанки - мягкий искусственный мех.',919,'1 шт',3,12,12,10,'76289903.png'),(79948186,'Сухой корм для мелких собак для пищеварения',' Эксперты PRO PLAN разработали сухие корма специально для собак с чувствительным пищеварением. ',581,'700 г',32,1,1,14,'79948186.png'),(87486541,'Курица игрушка для собак и животных','кричащая курица, пищалка для собак курица',172,'1 шт',19,2,5,4,'87486541.png'),(87682829,'Шлейка прогулочная с поводком для кошек','Шлейка для собак и кошек мелких и средних пород поможет Вам всегда быть спокойным за безопасность Вашего питомца.',221,'1 шт',11,11,16,14,'87682829.png'),(98487134,'Пеленки для собак и кошек Mr.Fresh','Подстилки Mr.Fresh Expert Regular 60х60 см 24 шт в упаковке незаменимы для содержания в чистоте туалета и места кормления питомца, для комфортной перевозки, при посещении ветеринара, в период карантина во время вакцинации.',781,'60х60 см 24 шт',23,14,3,3,'98487134.png'),(105232371,'Корм влажный для стерилизованных кошек рыба','Эксперты Pro Plan разработали влажный корм для стерилизованных кошек с учетом особых потребностей Вашего питомца.',2051,'85 г 26шт',21,4,1,10,'105232371.png'),(144365326,'Влажный корм для собак Телятина  в соусе','Влажный полнорационный корм для собак с чувствительным пищеварением с телятиной и тыквой в соусе AlphaPet Superpremium 100г 1 упаковка',1370,'15 шт',43,4,8,7,'144365326.png'),(147596897,'Корм для хомяков ','для повседневного кормления, для хомяков, для грызунов',269,'900 гр',11,1,3,2,'147596897.png'),(151118518,'Ошейник для собак электронный дрессировочный','Ошейник для собак - идеальное решение для эффективной дрессировки и контроля вашего питомца.',1807,'1 шт',15,8,14,12,'151118518.png'),(153402124,'Витамины для собак для шерсти','Независимо от породы – крупной, мелкой или средней, витамины играют важную роль в поддержании ее здоровья и благополучия.',333,'150 мл',34,7,13,11,'153402124.png'),(154744229,'Мультивитаминное лакомство для шерсти кошек','Витамины для кошек МультиЛакомки Восхитительная шерсть — кормовая добавка для улучшения состояния кожи и шерсти. Витаминизированное лакомство МультиЛакомки — это прекрасное дополнение к ежедневному рациону кошки.',144,'70 таблеток',9,7,19,16,'154744229.png'),(155325540,'Клетка для собак','Металлическая клетка размер N 2 подходит для содержания и перевозки домашних животных для собак мелких и средних пород, для кошек и для разных грызунов. ',2875,'62х44х49',7,15,14,12,'155325540.png'),(156278877,'Феликс Двойная вкуснятина','Сухой корм Felix Двойная Вкуснятина для взрослых кошек, с мясом, Пакет, 3 кг. Любопытство вашего кота заложено природой!',980,'3 кг',23,1,10,8,'156278877.png'),(158074124,'Лосьон для глаз для собак и кошек очищающий','Лосьон для глаз для собак и кошек очищающий представляет собой натуральное средство с ионами серебра и экстрактами трав, разработанное специально для ухода за глазами домашних питомцев. ',412,'1 шт 100 мл',22,13,17,15,'158074124.png'),(159874927,'Влажный корм для котят Индейка нежные ломтики','Команда AlphaPet создает и производит полезные корма высшей категории качества Holistiс, давая возможность каждому владельцу быть уверенным в правильном выборе кормления для своего животного. ',903,'15 шт',23,4,8,7,'159874927.png'),(169881547,'Клетка для собак и кошек большая в квартиру','Ищете просторное и удобное жилье для своего питомца? Большая двухэтажная клетка - это идеальное решение для кошек, собак малых и средних пород, крупных грызунов',2738,'105х70х35',5,15,12,8,'169881547.png'),(170076131,'Витамины для кошек для шерсти','Питомец сильно линяет или его здоровье ухудшилось?Ищете качественные витамины для кошек для иммунитета? У нас есть отличное решение для вас! ',333,'250 таблеток',56,7,5,4,'170076131.png'),(173028971,'Ошейник для собак средних и крупных пород','Нейлоновый ошейник для собак средних и крупных пород обеспечит комфорт вашему питомцу во время прогулок.',419,'1 шт',44,8,13,11,'173028971.png'),(177681148,'Переноска для кошек большая','Переноска рюкзак для кошек и собак малых пород - это функциональный и практичный аксессуар для животных любых видов. ',1273,'41x33x45',12,5,12,10,'177681148.png'),(177965748,'Влажный корм для кошек с говядиной и морковью','Влажный корм для кошек Purina One поддерживает здоровый обмен веществ и оптимальный вес питомца. ',843,'26 шт по 75г',104,4,9,8,'177965748.png'),(180021567,'Влажный корм для котят с Курицей','Команда квалифицированных ветеринарных специалистов разработала влажный корм PURINA PRO PLAN HEALTHY START для здорового развития котенка. ',1883,'26шт',19,4,1,8,'180021567.png'),(180190918,'Корм для собак влажный Паучи','Полнорационные консервированные корма для собак ВКУСМЯСИНА - это сочные мясные кусочки в ароматном соусе.',772,'85 г 30 шт',10,4,6,5,'180190918.png'),(188433865,'Слинг для кошек и собак','Сумка переноска для животных – идеальный аксессуар для комфортной и безопасной перевозки ваших любимцев. ',952,'75х40х25 до 7 кг',23,5,5,4,'188433865.png'),(189840934,'Миска для кошки и собак двойная белая','Эта регулируемая двойная миск а для кошек на подставке объемом 400 мл для собачек и котов - идеальное решение для ваших домашних животных! ',505,'400 мл',23,6,16,14,'189840934.png'),(207388443,'Пробиотик FortiFlora для кошек','Пробиотик FortiFlora для кошек – ваш надежный помощник в поддержании здоровья и иммунитета вашего любимца. ',713,'10 шт 1 гр',34,7,1,6,'207388443.png'),(210960902,'Корм для стерилизованных кошек сухой','Корм для кошек Pro Plan это продукт, который разработан с использованием самых высококачественных ингредиентов, чтобы удовлетворить даже самых требовательных питомцев.',1580,'1 кг 500 г',20,1,1,5,'210960902.png'),(211044139,'Корм для стерилизованных кошек с курицей ','Корм сухой для кошек Purina Pro Plan Sterilised включает в себя специальную формулу, которая обеспечивает эффективную работу почек и правильный уровень кислотности мочи. Антиоксиданты укрепляют иммунитет взрослой кошки замедляют возрастные изменения.',2683,'3 кг',17,1,1,1,''),(214758654,'Когтеточка для кошки напольная лежанка','Когтеточка для кошки сочетает в себе функциональность, безопасность и лаконичный дизайн для безопасных игр и отдыха домашних животных. ',249,'1 шт',10,12,11,9,'214758654.png'),(218123339,'Тоннель для кошек большой','Универсальная кровать-туннель для кошек: Идеально подходит для комнатных кошек, предлагая уютное место для дремоты, игр, сна и даже игры в прятки.',1967,'1 шт',23,2,6,5,'218123339.png'),(219353452,'Пеленка для собак многоразовая','Многоразовая пеленка - это удобное и практичное средство для тренировки питомца и поддержания чистоты вашего дома.',736,'100х70',15,14,19,2,'219353452.png'),(219962054,'Лакомства для собак Микс','Предлагаем вашему вниманию натуральное и вкусное лакомство для собак - микс из говяжьей трахеи, бычьего корня и говяжьего легкого. ',1079,'300г',23,9,6,5,'219962054.png'),(221804692,'Лосьон для ушей собак и кошек','Бережно удаляет загрязнения и выделения.Снимает зуд и раздражение.Снижает риск возникновения заболеваний ушей.',374,'25 мл',21,13,6,5,'221804692.png'),(223359997,'Игрушка для кошек и собак мячик интерактивный','интерактивный мячик для котов, шарик для щенков, умный мячик с подсветкой',227,'1 шт',91,2,4,3,'223359997.png'),(237885006,'Лежанка пушистая для кошек и собак','Уютная и мягкая лежанка для кошек и собак обеспечивает комфортное место для отдыха вашего питомца, создавая уют и спокойствие.',610,'1 шт',0,3,7,6,'237885006.png'),(249148037,'Лежаки для кошек и собак крупных пород','Meowso заботится о здоровье домашних животных! Прямоугольная форма удобна тем, что ее легко разместить вдоль любой свободной стены или в углу комнаты.',2434,'1 шт',2,3,6,5,'249148037.png'),(250104391,'Лосьон для чистки ушей собак и кошек','Лосьон для ушей разработан специально для ухода за ушами собак и кошек. Он мягко очищает и помогает поддерживать гигиену, устраняя накопившуюся грязь и излишки серы.',265,'100 мл',25,13,13,11,'250104391.png'),(251710899,'Переноска для животных кошек и собак','Переноска для кота и собаки выполнена из прочных, но легких материалов, которые легко чистятся. Она подходит как для собак мелких пород, так и для более крупных животных.',1152,'шт',12,5,14,11,'251710899.png'),(262159566,'Лакомство: легкое говяжье для собак','Сушеное говяжье легкое для собак — это натуральное и вкусное угощение, которое станет отличным дополнением к рациону вашего питомца. ',794,'500 г',25,9,8,7,'262159566.png'),(262898008,'Поводок нейлоновый красный для собак','Вашему вниманию представлен поводок для средних пород собак нейлоновый с вращающимся карабином. ',661,'10м*20мм',21,10,16,14,'262898008.png'),(266658357,'Сумка переноска для кошек и собак 2 в 1 весен','Ищете удобное решение для перевозки вашего питомца? Наша сумка переноска для кошек и собак станет незаменимым аксессуаром!',3371,'до 10 кг',23,5,17,15,'266658357.png'),(268694126,'Поводок брезентовый усиленный для собак хаки','Вашему вниманию представлен поводок для средних и крупных пород собак брезентовый с мощным вертлюгом и карабином.',1881,'20м*35мм',13,10,16,14,'268694126.png'),(272887994,'Одноразовые пеленки для животных','Пеленки для животных улучшенного качества с липучками — это одноразовые товары, предназначенные для собак и кошек.',733,'70 штук 60х45',10,14,15,6,'272887994.png'),(274863286,'Клетка для собаки','Клетка для собак номер 2 двухдверная - подходит для содержания и перевозки собак средней величины, клетка станет настоящим вторым домом любимых питомцев. ',3268,'60х42х52',10,15,16,7,'274863286.png'),(316630648,'Шлейка анатомическая для кошек с крыльями','Представляем вам стильную и удобную шлейку KINGDOM pets с милыми крылышками, созданную специально для прогулок с вашими любимыми кошками и маленькими собачками! ',370,'1 шт',32,11,15,13,'316630648.png'),(338471164,'Поводок для собак мелких и средних пород','Поводок-рулетка для собак и домашних животных: идеальный аксессуар для комфортных и безопасных прогулок. ',517,'5 м',13,10,16,6,'338471164.png'),(355155012,'Лакомства для собак мясо утки','Вкусняшки для ваших питомцев. Лакомства для собак Мясо утки на сыромятных кольцах - это вкусный и полезный способ побаловать своего четвероногого друга.',1097,'500 г',32,9,6,5,'355155012.png');
/*!40000 ALTER TABLE `product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productmanufactur`
--

DROP TABLE IF EXISTS `productmanufactur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productmanufactur` (
  `ProductManufacturID` int NOT NULL AUTO_INCREMENT,
  `ProductManufacturName` varchar(45) NOT NULL,
  PRIMARY KEY (`ProductManufacturID`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productmanufactur`
--

LOCK TABLES `productmanufactur` WRITE;
/*!40000 ALTER TABLE `productmanufactur` DISABLE KEYS */;
INSERT INTO `productmanufactur` VALUES (1,'PRO PLAN'),(2,'Mr.BUFFALO'),(3,'HAPPY JUNGLE'),(4,'Uoconelle'),(5,'FixZone'),(6,'Meowso'),(7,'Веселые ушастики'),(8,'AlphaPet'),(9,'PURINA ONE'),(10,'Felix'),(11,'Довольный пушистик'),(12,'Мармариал'),(13,'PetsVector'),(14,'KINGSTAR'),(15,'KINGDOM pets'),(16,'nooza'),(17,'SILVERIA'),(18,'CitoDerm'),(19,'MultiЛакомки');
/*!40000 ALTER TABLE `productmanufactur` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productorder`
--

DROP TABLE IF EXISTS `productorder`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productorder` (
  `ProductID` int NOT NULL,
  `ProductCount` int NOT NULL,
  `OrderID` int NOT NULL,
  PRIMARY KEY (`ProductID`,`OrderID`),
  KEY `OrderID` (`OrderID`),
  CONSTRAINT `productorder_ibfk_1` FOREIGN KEY (`OrderID`) REFERENCES `order` (`OrderID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `productorder_ibfk_2` FOREIGN KEY (`ProductID`) REFERENCES `product` (`ProductArticul`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productorder`
--

LOCK TABLES `productorder` WRITE;
/*!40000 ALTER TABLE `productorder` DISABLE KEYS */;
/*!40000 ALTER TABLE `productorder` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `role`
--

DROP TABLE IF EXISTS `role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `role` (
  `RoleID` int NOT NULL AUTO_INCREMENT,
  `Role` varchar(45) NOT NULL,
  PRIMARY KEY (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `role`
--

LOCK TABLES `role` WRITE;
/*!40000 ALTER TABLE `role` DISABLE KEYS */;
INSERT INTO `role` VALUES (1,'Администратор'),(2,'Товаровед'),(3,'Продавец');
/*!40000 ALTER TABLE `role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier`
--

DROP TABLE IF EXISTS `supplier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier` (
  `SupplierID` int NOT NULL AUTO_INCREMENT,
  `SupplierName` varchar(25) NOT NULL,
  PRIMARY KEY (`SupplierID`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier`
--

LOCK TABLES `supplier` WRITE;
/*!40000 ALTER TABLE `supplier` DISABLE KEYS */;
INSERT INTO `supplier` VALUES (1,'БиллиБосс'),(2,'Neoterica GmbH'),(3,'Магазин низких цен'),(4,'FixZone'),(5,'Meowso'),(6,'Веселые ушастики'),(7,'AlphaPet'),(8,'Маркет Партнер'),(9,'Довольный пушистик'),(10,'Мармариал'),(11,'PetsVector'),(12,'KINGSTAR'),(13,'KINGDOM pets'),(14,'nooza'),(15,'SILVERIA'),(16,'Neoterica GmbH');
/*!40000 ALTER TABLE `supplier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `UserFIO` int NOT NULL,
  `RoleID` int NOT NULL,
  `Login` varchar(45) NOT NULL,
  `Password` varchar(100) NOT NULL,
  PRIMARY KEY (`UserID`),
  KEY `RoleID` (`RoleID`),
  KEY `UserFIO` (`UserFIO`),
  CONSTRAINT `user_ibfk_1` FOREIGN KEY (`RoleID`) REFERENCES `role` (`RoleID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `user_ibfk_2` FOREIGN KEY (`UserFIO`) REFERENCES `employee` (`EmployeeID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,1,1,'ivanov','5c00d8a50ce2679c308f5af180b01430282cd6c9df6afd0e7ccc90a2b3955488'),(2,2,2,'petrova','5789d96ec47a3b9a0c567576321328567ed963d6336df479942bafcac15188e4'),(3,3,3,'sidorov','6e6ccd85d4ec1491a30438eb7194b4fc819e05c2a71ec47b282171d36650ce2e'),(7,4,1,'admin','8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918'),(21,8,2,'manager','6ee4a469cd4e91053847f5d3fcb61dbcc91e8f0ef10be7748da4c4a1ba382d17'),(30,11,3,'seller','a4279eae47aaa7417da62434795a011ccb0ec870f7f56646d181b5500a892a9a'),(31,12,2,'fedorova','7ea3f0026224295dc3cfb35a1ae3febb3fa27a79c2a92d93fbd245915cbce27a'),(32,13,1,'ad','70ba33708cbfb103f1a8e34afef333ba7dc021022b2d9aaa583aabb8058d8d67'),(33,14,2,'mn','ea43de53dc947fdf3cedaa4abc519f7889d5cd61f66a5ae764eb30d32c6186f9'),(34,15,3,'se','ad9a67fefa847de87753df6794a0ae466431e76ad1fb4db58fbbe836d1dde0e7'),(35,1,1,'7','7902699be42c8a8e46fbbb4501726517e86b22c56a189f7625a6da49081b2451'),(36,1,1,'r','454349e422f05297191ead13e21d3db520e5abef52055e4964b82fb213f593a1'),(37,2,1,'rr','597c28c381ef1feee61f3e9677a628b4cbd41cfb2539c8938062e1df2a882d39'),(38,2,1,'66','3ada92f28b4ceda38562ebf047c6ff05400d4c572352a1142eedfef67d21e662');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userr`
--

DROP TABLE IF EXISTS `userr`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userr` (
  `UserID` int NOT NULL AUTO_INCREMENT,
  `UserF` int NOT NULL,
  `UserI` int NOT NULL,
  `UserO` int NOT NULL,
  `RoleID` int NOT NULL,
  `Login` varchar(45) NOT NULL,
  `Password` varchar(100) NOT NULL,
  PRIMARY KEY (`UserID`),
  KEY `employF_idx` (`UserF`),
  KEY `epI_idx` (`UserI`),
  KEY `err_idx` (`UserO`),
  KEY `rolee_idx` (`RoleID`),
  CONSTRAINT `employF` FOREIGN KEY (`UserF`) REFERENCES `employeeee` (`EmployeeID`),
  CONSTRAINT `epI` FOREIGN KEY (`UserI`) REFERENCES `employeeee` (`EmployeeID`),
  CONSTRAINT `err` FOREIGN KEY (`UserO`) REFERENCES `employeeee` (`EmployeeID`),
  CONSTRAINT `rolee` FOREIGN KEY (`RoleID`) REFERENCES `role` (`RoleID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userr`
--

LOCK TABLES `userr` WRITE;
/*!40000 ALTER TABLE `userr` DISABLE KEYS */;
INSERT INTO `userr` VALUES (1,1,1,1,1,'ааа','ааа'),(2,2,2,2,1,'hhhh','72b289ec78e0a928c565480a435453e30acb92eddb3b78ff168b28737cf6a849'),(3,3,3,3,2,'karpov','4caf42c30479e635f5c2ba025e6c81b42f196c421871d3dbe993349a52a944f5');
/*!40000 ALTER TABLE `userr` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-05  9:47:23
