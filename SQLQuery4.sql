
CREATE DATABASE Jord;

USE Jord;

-- Create the Member table
CREATE TABLE Member (
    MemberID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50),
    Email VARCHAR(100),
    Password VARCHAR(50)
);

-- Create the Orders table
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATE,
    TotalPrice DECIMAL(10, 2),
    MemberID INT,
    FOREIGN KEY (MemberID) REFERENCES Member (MemberID)
);

-- Create the OrderItem table
CREATE TABLE OrderItemsss (
    OrderID INT,
    Item_ID INT,
    Quantity INT,
    PRIMARY KEY (OrderID, Item_ID),
    FOREIGN KEY (OrderID) REFERENCES Orders (OrderID),
    FOREIGN KEY (Item_ID) REFERENCES Item (Item_ID)
);

-- Create the Item table
CREATE TABLE Item (
    Item_Name VARCHAR(50),
    Item_ID INT IDENTITY(1,1) PRIMARY KEY,
    Item_Price Float(10, 2),
    Item_Description VARCHAR(500),
    Item_Type VARCHAR(30),
    Item_Img VARCHAR(500)
);
