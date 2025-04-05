SELECT * FROM db45.employeeee;
SELECT * FROM db45.userr;

SELECT userr.UserID AS 'id',
empF.EmployeeF AS 'Фамилия',
left(empF.EmployeeI,1) AS 'Имя',
left(empO.EmployeeO,1) AS 'Отчество',
`role`.Role AS 'Роль',
userr.Login AS 'Логин',
userr.Password AS 'Пароль'
FROM userr
INNER JOIN employeeee AS empF ON userr.UserF = empF.EmployeeID
INNER JOIN employeeee AS empI ON userr.UserI = empI.EmployeeID
INNER JOIN employeeee AS empO ON userr.UserO = empO.EmployeeID
INNER JOIN `role` ON userr.RoleID = `role`.RoleID;
                
 SELECT EmployeeF,EmployeeI,EmployeeO  FROM employeeee;       
 
 SELECT EmployeeID FROM employee WHERE EmployeeFIO = " Петрова Мария Николаевна ";
 SELECT EmployeeID FROM employeeee WHERE EmployeeF LIKE "Иванов" AND EmployeeI LIKE "Иван" AND EmployeeO LIKE "Иванович";                
                
SELECT EmployeeID AS 'id', 
EmployeeF AS 'Фамилия',
left(EmployeeI,1) AS 'Имя',
left(EmployeeO,1) AS 'Отчетство',
concat(left(telephone,7),"***", right(telephone,5)) AS 'Номер телефона',
concat(left(pasport,5),"****") AS 'Паспорт'
FROM  employeeee; 