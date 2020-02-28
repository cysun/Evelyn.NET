ALTER TABLE `Users` AUTO_INCREMENT=1000;
ALTER TABLE `Books` AUTO_INCREMENT=1000;
ALTER TABLE `Files` AUTO_INCREMENT=1000000;
ALTER TABLE `Chapters` AUTO_INCREMENT=1000000;
ALTER TABLE `Bookmarks` AUTO_INCREMENT=1000000;

INSERT INTO `Users` (`Name`, `Hash`) VALUES ('cysun', '$2a$11$Y87nF62c7gvg7fbAa6IYA.xgEaYssPTew0JysYlqM3agW/Yeecc2u');

CREATE TABLE `BookMarkdownFiles` (
    `BookId` int NOT NULL REFERENCES `Books`(`Id`) ON DELETE CASCADE,
    `Title` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Author` varchar(255) CHARACTER SET utf8mb4 NULL,
    `MarkdownFileId` int NOT NULL REFERENCES `Files`(`Id`) ON DELETE CASCADE,
    `Content` longtext NULL,
    PRIMARY KEY (`BookId`),
    FULLTEXT (`Title`, `Author`, `Content`) WITH PARSER ngram
);

DELIMITER $$

CREATE TRIGGER `BooksFTS_INSERT` AFTER INSERT ON `Books` FOR EACH ROW
BEGIN
    IF NEW.`MarkdownFileId` IS NOT NULL THEN
        INSERT INTO `BookMarkdownFiles` (`BookId`, `Title`, `Author`, `MarkdownFileId`, `Content`) VALUES
            (NEW.`Id`, NEW.`Title`, NEW.`Author`, NEW.`MarkdownFileId`,
            (SELECT CONVERT(`Content` USING utf8mb4) FROM `Files` WHERE `Id` = NEW.`MarkdownFileId`)) AS `Book`
        ON DUPLICATE KEY UPDATE `Title`=`Book`.`Title`, `Author`=`Book`.`Author`, `Content`=`Book`.`Content`;
    END IF;
END; $$

CREATE TRIGGER `BooksFTS_UPDATE` AFTER UPDATE ON `Books` FOR EACH ROW
BEGIN
    IF NEW.`MarkdownFileId` IS NOT NULL THEN
        INSERT INTO `BookMarkdownFiles` (`BookId`, `Title`, `Author`, `MarkdownFileId`, `Content`) VALUES
            (NEW.`Id`, NEW.`Title`, NEW.`Author`, NEW.`MarkdownFileId`,
            (SELECT CONVERT(`Content` USING utf8mb4) FROM `Files` WHERE `Id` = NEW.`MarkdownFileId`)) AS `Book`
        ON DUPLICATE KEY UPDATE `Title`=`Book`.`Title`, `Author`=`Book`.`Author`, `Content`=`Book`.`Content`;
    END IF;
END; $$

DELIMITER ;
