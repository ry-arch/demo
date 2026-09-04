CREATE TABLE Roles(
    RoleID INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users(
    UserID INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL,
    Login NVARCHAR(255) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    RoleID INT NOT NULL,

    FOREIGN KEY(RoleID)
        REFERENCES Roles(RoleID)
);

CREATE TABLE Categories(
    CategoryID INT IDENTITY PRIMARY KEY,
    CategoryName NVARCHAR(255) NOT NULL
);

CREATE TABLE Manufacturers(
    ManufacturerID INT IDENTITY PRIMARY KEY,
    ManufacturerName NVARCHAR(255) NOT NULL
);

CREATE TABLE Units(
    UnitID INT IDENTITY PRIMARY KEY,
    UnitName NVARCHAR(50) NOT NULL
);

CREATE TABLE Products(
    Article NVARCHAR(20) PRIMARY KEY,

    ProductName NVARCHAR(500) NOT NULL,

    UnitID INT NOT NULL,
    ManufacturerID INT NOT NULL,
    CategoryID INT NOT NULL,

    Supplier NVARCHAR(255) NOT NULL,

    Price DECIMAL(10,2) NOT NULL,

    Discount INT NOT NULL,

    StockQuantity INT NOT NULL,

    Description NVARCHAR(MAX),

    PhotoPath NVARCHAR(255),

    FOREIGN KEY(UnitID)
        REFERENCES Units(UnitID),

    FOREIGN KEY(ManufacturerID)
        REFERENCES Manufacturers(ManufacturerID),

    FOREIGN KEY(CategoryID)
        REFERENCES Categories(CategoryID)
);

CREATE TABLE PickupPoints(
    PickupPointID INT IDENTITY PRIMARY KEY,
    Address NVARCHAR(255) NOT NULL
);

CREATE TABLE OrderStatuses(
    StatusID INT IDENTITY PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL
);

CREATE TABLE Orders(
    OrderID INT IDENTITY PRIMARY KEY,

    OrderDate DATE NOT NULL,

    DeliveryDate DATE NOT NULL,

    PickupPointID INT NOT NULL,

    UserID INT NOT NULL,

    ReceiveCode INT NOT NULL,

    StatusID INT NOT NULL,

    FOREIGN KEY(PickupPointID)
        REFERENCES PickupPoints(PickupPointID),

    FOREIGN KEY(UserID)
        REFERENCES Users(UserID),

    FOREIGN KEY(StatusID)
        REFERENCES OrderStatuses(StatusID)
);

CREATE TABLE OrderProducts(
    OrderProductID INT IDENTITY PRIMARY KEY,

    OrderID INT NOT NULL,

    ProductArticle NVARCHAR(20) NOT NULL,

    Quantity INT NOT NULL,

    FOREIGN KEY(OrderID)
        REFERENCES Orders(OrderID),

    FOREIGN KEY(ProductArticle)
        REFERENCES Products(Article)
);

