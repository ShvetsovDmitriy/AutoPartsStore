CREATE Database [REST service];
GO

USE [REST service];
GO

CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(100),
    PhoneNumber NVARCHAR(10)
);
GO

CREATE TABLE ProductTypes (
    ProductTypeId INT PRIMARY KEY IDENTITY,
    TypeName NVARCHAR(100)
);
GO
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY,
    ProductTypeId INT FOREIGN KEY REFERENCES ProductTypes(ProductTypeId),
    ProductName NVARCHAR(100),
    Price DECIMAL(18, 2),
    AvailableQuantity INT
);
GO
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    OrderDate DATETIME
);
GO
CREATE TABLE OrderItems (
    OrderItemId INT PRIMARY KEY IDENTITY,
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ProductId INT FOREIGN KEY REFERENCES Products(ProductId),
    Price DECIMAL(18, 2),
    Quantity INT
);
GO
INSERT INTO Customers (FullName, PhoneNumber)
VALUES 
    (N'Швецов Дмитрий Вадимович', '9032228891'),
    (N'Алексев Петр Иванович', '9232328841'),
    (N'Сидоров Иван Валерьевич', '9112348841');
GO
INSERT INTO ProductTypes (TypeName)
VALUES 
    (N'Запчасти ходовой'),
    (N'Запчасти для кузова'),
    (N'Моторные масла и фильтры'),
	(N'Оптика');
GO
INSERT INTO Products (ProductTypeId, ProductName, Price, AvailableQuantity)
VALUES 
    (1, N'Амортизатор передний', 1500.00, 50),
    (1, N'Пружина передняя', 500.00, 100),
    (1, N'Шаровая опора', 700.00, 80);
GO

INSERT INTO Products (ProductTypeId, ProductName, Price, AvailableQuantity)
VALUES 
    (2, N'Бампер передний', 3000.00, 30),
    (2, N'Крыло переднее левое', 2500.00, 20),
    (2, N'Фара передняя', 2000.00, 40);
GO

INSERT INTO Products (ProductTypeId, ProductName, Price, AvailableQuantity)
VALUES 
    (3, N'Моторное масло 5W-40', 1000.00, 60),
    (3, N'Воздушный фильтр', 500.00, 70),
    (3, N'Масляный фильтр', 400.00, 80);
GO

INSERT INTO Products (ProductTypeId, ProductName, Price, AvailableQuantity)
VALUES 
    (4, N'Фара задняя', 1500.00, 50),
    (4, N'Фонарь задний', 1000.00, 60),
    (4, N'Поворотник', 800.00, 70);
GO
INSERT INTO Orders (CustomerId, OrderDate)
VALUES 
    (1, GETDATE()),
    (2, GETDATE()),
    (3, GETDATE());
GO
INSERT INTO OrderItems (OrderId, ProductId, Price, Quantity)
VALUES 
    (1, 1, 1500.00, 2),
    (1, 2, 500.00, 4),
    (2, 4, 3000.00, 1),
    (2, 6, 2000.00, 2),
    (3, 8, 1000.00, 3),
    (3, 10, 1500.00, 1);
GO