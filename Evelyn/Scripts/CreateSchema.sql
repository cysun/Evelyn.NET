CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Files` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8mb4 NULL,
    `ContentType` varchar(255) CHARACTER SET utf8mb4 NULL,
    `Length` bigint NOT NULL,
    `Timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `Content` longblob NULL,
    CONSTRAINT `PK_Files` PRIMARY KEY (`Id`)
);

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Hash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`),
    CONSTRAINT `AK_Users_Name` UNIQUE (`Name`)
);

CREATE TABLE `Books` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Author` varchar(255) CHARACTER SET utf8mb4 NULL,
    `Notes` longtext CHARACTER SET utf8mb4 NULL,
    `MarkdownFileId` int NOT NULL,
    `EBookFileId` int NULL,
    `CoverFileId` int NULL,
    `ThumbnailFileId` int NULL,
    `LastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsDeleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_Books` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Books_Files_CoverFileId` FOREIGN KEY (`CoverFileId`) REFERENCES `Files` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Books_Files_EBookFileId` FOREIGN KEY (`EBookFileId`) REFERENCES `Files` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Books_Files_MarkdownFileId` FOREIGN KEY (`MarkdownFileId`) REFERENCES `Files` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Books_Files_ThumbnailFileId` FOREIGN KEY (`ThumbnailFileId`) REFERENCES `Files` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Chapters` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `BookId` int NOT NULL,
    `Number` int NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8mb4 NULL,
    `MarkdownFileId` int NOT NULL,
    `HtmlFileId` int NOT NULL,
    `LastUpdated` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `PK_Chapters` PRIMARY KEY (`Id`),
    CONSTRAINT `AK_Chapters_BookId_Number` UNIQUE (`BookId`, `Number`),
    CONSTRAINT `FK_Chapters_Books_BookId` FOREIGN KEY (`BookId`) REFERENCES `Books` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Chapters_Files_HtmlFileId` FOREIGN KEY (`HtmlFileId`) REFERENCES `Files` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Chapters_Files_MarkdownFileId` FOREIGN KEY (`MarkdownFileId`) REFERENCES `Files` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bookmarks` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UserId` int NOT NULL,
    `ChapterId` int NOT NULL,
    `Paragraph` int NOT NULL,
    `Timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsManual` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_Bookmarks` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Bookmarks_Chapters_ChapterId` FOREIGN KEY (`ChapterId`) REFERENCES `Chapters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bookmarks_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_Bookmarks_ChapterId` ON `Bookmarks` (`ChapterId`);

CREATE INDEX `IX_Bookmarks_UserId` ON `Bookmarks` (`UserId`);

CREATE INDEX `IX_Books_CoverFileId` ON `Books` (`CoverFileId`);

CREATE INDEX `IX_Books_EBookFileId` ON `Books` (`EBookFileId`);

CREATE INDEX `IX_Books_MarkdownFileId` ON `Books` (`MarkdownFileId`);

CREATE INDEX `IX_Books_ThumbnailFileId` ON `Books` (`ThumbnailFileId`);

CREATE INDEX `IX_Chapters_HtmlFileId` ON `Chapters` (`HtmlFileId`);

CREATE INDEX `IX_Chapters_MarkdownFileId` ON `Chapters` (`MarkdownFileId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200228065047_InitialSchema', '3.1.2');

