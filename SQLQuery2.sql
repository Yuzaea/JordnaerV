-- Use the database
USE Jord;



-- Create the OrderItem table
CREATE TABLE OrderItem (
    OrderID INT,
    ID INT,
    Quantity INT,
    PRIMARY KEY (OrderID, ID),
    FOREIGN KEY (OrderID) REFERENCES Orders (OrderID),
    FOREIGN KEY (ID) REFERENCES Item (ID)
);

