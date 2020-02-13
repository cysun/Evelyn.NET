﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Files" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NULL,
    "ContentType" text NULL,
    "Length" bigint NOT NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "Content" bytea NULL,
    CONSTRAINT "PK_Files" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" text NOT NULL,
    "Hash" text NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "AK_Users_Name" UNIQUE ("Name")
);

CREATE TABLE "Books" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Title" text NOT NULL,
    "Author" text NULL,
    "Notes" text NULL,
    "ChaptersCount" integer NOT NULL,
    "MarkdownFileId" integer NOT NULL,
    "EBookFileId" integer NULL,
    "CoverFileId" integer NULL,
    "ThumbnailFileId" integer NULL,
    "LastUpdated" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsDeleted" boolean NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_Books" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Books_Files_CoverFileId" FOREIGN KEY ("CoverFileId") REFERENCES "Files" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Books_Files_EBookFileId" FOREIGN KEY ("EBookFileId") REFERENCES "Files" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Books_Files_MarkdownFileId" FOREIGN KEY ("MarkdownFileId") REFERENCES "Files" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Books_Files_ThumbnailFileId" FOREIGN KEY ("ThumbnailFileId") REFERENCES "Files" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Chapters" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "BookId" integer NOT NULL,
    "Number" integer NOT NULL,
    "Name" text NULL,
    "MarkdownFileId" integer NOT NULL,
    "HtmlFileId" integer NOT NULL,
    "LastUpdated" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_Chapters" PRIMARY KEY ("Id"),
    CONSTRAINT "AK_Chapters_BookId_Number" UNIQUE ("BookId", "Number"),
    CONSTRAINT "FK_Chapters_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Chapters_Files_HtmlFileId" FOREIGN KEY ("HtmlFileId") REFERENCES "Files" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Chapters_Files_MarkdownFileId" FOREIGN KEY ("MarkdownFileId") REFERENCES "Files" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Bookmarks" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "UserId" integer NOT NULL,
    "ChapterId" integer NOT NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsManual" boolean NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_Bookmarks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Bookmarks_Chapters_ChapterId" FOREIGN KEY ("ChapterId") REFERENCES "Chapters" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Bookmarks_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Bookmarks_ChapterId" ON "Bookmarks" ("ChapterId");

CREATE INDEX "IX_Bookmarks_UserId" ON "Bookmarks" ("UserId");

CREATE INDEX "IX_Books_CoverFileId" ON "Books" ("CoverFileId");

CREATE INDEX "IX_Books_EBookFileId" ON "Books" ("EBookFileId");

CREATE INDEX "IX_Books_MarkdownFileId" ON "Books" ("MarkdownFileId");

CREATE INDEX "IX_Books_ThumbnailFileId" ON "Books" ("ThumbnailFileId");

CREATE INDEX "IX_Chapters_HtmlFileId" ON "Chapters" ("HtmlFileId");

CREATE INDEX "IX_Chapters_MarkdownFileId" ON "Chapters" ("MarkdownFileId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200213065231_InitialSchema', '3.1.1');

