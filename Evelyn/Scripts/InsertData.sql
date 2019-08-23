ALTER SEQUENCE "Users_UserId_seq" START WITH 1000;
ALTER SEQUENCE "Books_BookId_seq" START WITH 1000;
ALTER SEQUENCE "Files_FileId_seq" START WITH 1000000;

INSERT INTO "Users" ("Name", "Hash") VALUES ('cysun', '$2a$11$Y87nF62c7gvg7fbAa6IYA.xgEaYssPTew0JysYlqM3agW/Yeecc2u');
