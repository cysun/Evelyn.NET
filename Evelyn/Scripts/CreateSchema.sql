CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "Files" (
    "FileId" serial NOT NULL,
    "Name" text NULL,
    "ContentType" text NULL,
    "Length" bigint NOT NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "Content" bytea NULL,
    CONSTRAINT "PK_Files" PRIMARY KEY ("FileId")
);

CREATE TABLE "Users" (
    "UserId" serial NOT NULL,
    "Name" text NOT NULL,
    "Hash" text NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId"),
    CONSTRAINT "AK_Users_Name" UNIQUE ("Name")
);

CREATE TABLE "Books" (
    "BookId" serial NOT NULL,
    "Title" text NOT NULL,
    "Author" text NULL,
    "Notes" text NULL,
    "MarkdownFileId" integer NOT NULL,
    "EBookFileId" integer NULL,
    "CoverFileId" integer NULL,
    "ThumbnailFileId" integer NULL,
    "LastUpdated" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "IsDeleted" boolean NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_Books" PRIMARY KEY ("BookId"),
    CONSTRAINT "FK_Books_Files_CoverFileId" FOREIGN KEY ("CoverFileId") REFERENCES "Files" ("FileId") ON DELETE RESTRICT,
    CONSTRAINT "FK_Books_Files_EBookFileId" FOREIGN KEY ("EBookFileId") REFERENCES "Files" ("FileId") ON DELETE RESTRICT,
    CONSTRAINT "FK_Books_Files_MarkdownFileId" FOREIGN KEY ("MarkdownFileId") REFERENCES "Files" ("FileId") ON DELETE CASCADE,
    CONSTRAINT "FK_Books_Files_ThumbnailFileId" FOREIGN KEY ("ThumbnailFileId") REFERENCES "Files" ("FileId") ON DELETE RESTRICT
);

CREATE TABLE "Bookmarks" (
    "BookmarkId" serial NOT NULL,
    "UserId" integer NOT NULL,
    "BookId" integer NOT NULL,
    "ChapterNumber" integer NOT NULL,
    "Position" integer NOT NULL,
    "Timestamp" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_Bookmarks" PRIMARY KEY ("BookmarkId"),
    CONSTRAINT "AK_Bookmarks_UserId_BookId" UNIQUE ("UserId", "BookId"),
    CONSTRAINT "FK_Bookmarks_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("BookId") ON DELETE CASCADE,
    CONSTRAINT "FK_Bookmarks_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "Chapters" (
    "BookId" integer NOT NULL,
    "Number" integer NOT NULL,
    "Name" text NULL,
    "MarkdownFileId" integer NOT NULL,
    "HtmlFileId" integer NOT NULL,
    "LastUpdated" timestamp without time zone NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "PK_Chapters" PRIMARY KEY ("BookId", "Number"),
    CONSTRAINT "FK_Chapters_Books_BookId" FOREIGN KEY ("BookId") REFERENCES "Books" ("BookId") ON DELETE CASCADE,
    CONSTRAINT "FK_Chapters_Files_HtmlFileId" FOREIGN KEY ("HtmlFileId") REFERENCES "Files" ("FileId") ON DELETE CASCADE,
    CONSTRAINT "FK_Chapters_Files_MarkdownFileId" FOREIGN KEY ("MarkdownFileId") REFERENCES "Files" ("FileId") ON DELETE CASCADE
);

CREATE INDEX "IX_Bookmarks_BookId" ON "Bookmarks" ("BookId");

CREATE INDEX "IX_Books_CoverFileId" ON "Books" ("CoverFileId");

CREATE INDEX "IX_Books_EBookFileId" ON "Books" ("EBookFileId");

CREATE INDEX "IX_Books_MarkdownFileId" ON "Books" ("MarkdownFileId");

CREATE INDEX "IX_Books_ThumbnailFileId" ON "Books" ("ThumbnailFileId");

CREATE INDEX "IX_Chapters_HtmlFileId" ON "Chapters" ("HtmlFileId");

CREATE INDEX "IX_Chapters_MarkdownFileId" ON "Chapters" ("MarkdownFileId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190823021837_InitialSchema', '2.2.4-servicing-10062');

