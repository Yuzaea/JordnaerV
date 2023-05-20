-- Use the database
USE Jord;




CREATE TABLE Item (
    Item_Name VARCHAR(50) not null,
    Item_ID INT IDENTITY(1,1) PRIMARY KEY,
    Item_Price float,
    Item_Description VARCHAR(500) NOT NULL,
    Item_Type VARCHAR(30) NOT NULL ,
    Item_Img VARCHAR(500) NOT NULL
);
