CREATE TABLE Server (
    Id INTEGER PRIMARY KEY,
    Name TEXT
);

CREATE TABLE Material (
    Id INTEGER PRIMARY KEY,
    Name TEXT,
    ServerId INTEGER,
    FOREIGN KEY (ServerId) REFERENCES Server(Id)
);

CREATE TABLE Product (
    Id INTEGER PRIMARY KEY,
    Name TEXT,
    Target INTEGER,
    MaterialId INTEGER,
    FOREIGN KEY (MaterialId) REFERENCES Material(Id)
);

CREATE TABLE Move (
    Id INTEGER PRIMARY KEY,
    SEQ INTEGER NOT NULL,
    Delta REAL,
    ProductId INTEGER,
    FOREIGN KEY (ProductId) REFERENCES Product(Id)
);