CREATE TABLE Board
(
    Id        TEXT PRIMARY KEY,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    Name      TEXT NOT NULL
);

CREATE TABLE Category
(
    Id          TEXT PRIMARY KEY,
    CreatedAt   DATETIME,
    UpdatedAt   DATETIME,
    BoardId     TEXT NOT NULL,
    Name        TEXT NOT NULL,
    Description TEXT,
    FOREIGN KEY (BoardId) REFERENCES Board (Id)
);

CREATE TABLE Card
(
    Id         TEXT PRIMARY KEY,
    CreatedAt  DATETIME,
    UpdatedAt  DATETIME,
    CategoryId TEXT NOT NULL,
    Name       TEXT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Category (Id)
);

CREATE TABLE Attachment
(
    Id        TEXT PRIMARY KEY,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    CardId    TEXT    NOT NULL,
    Name      TEXT    NOT NULL,
    Size      INTEGER NOT NULL,
    MimeType  TEXT    NOT NULL,
    FOREIGN KEY (CardId) REFERENCES Card (Id)
);

CREATE TABLE Comment
(
    Id        TEXT PRIMARY KEY,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    Content   TEXT NOT NULL,
    CardId    TEXT NOT NULL,
    FOREIGN KEY (CardId) REFERENCES Card (Id)
);

----- INSERT -----

CREATE TRIGGER UpdateCreatedAtOnAttachment
    AFTER INSERT
    ON Attachment
    FOR EACH ROW
BEGIN
    UPDATE Attachment SET UpdatedAt = datetime('now') WHERE Id = new.Id;
END;

CREATE TRIGGER UpdateCreatedAtOnBoard
    AFTER INSERT
    ON Board
    FOR EACH ROW
BEGIN
    UPDATE Board SET UpdatedAt = datetime('now') WHERE Id = new.Id;
END;

CREATE TRIGGER UpdateCreatedAtOnCard
    AFTER INSERT
    ON Card
    FOR EACH ROW
BEGIN
    UPDATE Card SET UpdatedAt = datetime('now') WHERE Id = new.Id;
END;

CREATE TRIGGER UpdateCreatedAtOnCategory
    AFTER INSERT
    ON Category
    FOR EACH ROW
BEGIN
    UPDATE Category SET UpdatedAt = datetime('now') WHERE Id = new.Id;
END;

CREATE TRIGGER UpdateCreatedAtOnComment
    AFTER INSERT
    ON Comment
    FOR EACH ROW
BEGIN
    UPDATE Comment SET UpdatedAt = datetime('now') WHERE Id = new.Id;
END;

----- UPDATE -----

CREATE TRIGGER UpdateUpdatedAtOnAttachment
    AFTER UPDATE
    ON Attachment
    FOR EACH ROW
BEGIN
    UPDATE Attachment SET UpdatedAt = datetime('now') WHERE Id = old.Id;
END;

CREATE TRIGGER UpdateUpdatedAtOnBoard
    AFTER UPDATE
    ON Board
    FOR EACH ROW
BEGIN
    UPDATE Board SET UpdatedAt = datetime('now') WHERE Id = old.Id;
END;

CREATE TRIGGER UpdateUpdatedAtOnCard
    AFTER UPDATE
    ON Card
    FOR EACH ROW
BEGIN
    UPDATE Card SET UpdatedAt = datetime('now') WHERE Id = old.Id;
END;

CREATE TRIGGER UpdateUpdatedAtOnCategory
    AFTER UPDATE
    ON Category
    FOR EACH ROW
BEGIN
    UPDATE Category SET UpdatedAt = datetime('now') WHERE Id = old.Id;
END;

CREATE TRIGGER UpdateUpdatedAtOnComment
    AFTER UPDATE
    ON Comment
    FOR EACH ROW
BEGIN
    UPDATE Comment SET UpdatedAt = datetime('now') WHERE Id = old.Id;
END;