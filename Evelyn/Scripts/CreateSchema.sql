IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Files] (
    [FileId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [ContentType] nvarchar(max) NULL,
    [Length] bigint NOT NULL,
    [Timestamp] datetime2 NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    [Content] varbinary(max) NULL,
    [Type] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY ([FileId])
);

GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Name] nvarchar(450) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [AK_Users_Name] UNIQUE ([Name])
);

GO

CREATE TABLE [Books] (
    [BookId] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Author] nvarchar(max) NULL,
    [MarkdownFileId] int NULL,
    [EBookFileId] int NULL,
    [CoverFileId] int NULL,
    [ThumbnailFileId] int NULL,
    [IsDeleted] bit NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Books] PRIMARY KEY ([BookId]),
    CONSTRAINT [FK_Books_Files_CoverFileId] FOREIGN KEY ([CoverFileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Books_Files_EBookFileId] FOREIGN KEY ([EBookFileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Books_Files_MarkdownFileId] FOREIGN KEY ([MarkdownFileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Books_Files_ThumbnailFileId] FOREIGN KEY ([ThumbnailFileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Bookmarks] (
    [BookmarkId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [BookId] int NOT NULL,
    [ChapterNumber] int NOT NULL,
    [Position] int NOT NULL,
    [Timestamp] datetime2 NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT [PK_Bookmarks] PRIMARY KEY ([BookmarkId]),
    CONSTRAINT [FK_Bookmarks_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Bookmarks_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Chapters] (
    [Number] int NOT NULL,
    [BookId] int NOT NULL,
    [Name] nvarchar(max) NULL,
    [MarkdownFileId] int NULL,
    [HtmlFileId] int NULL,
    CONSTRAINT [PK_Chapters] PRIMARY KEY ([BookId], [Number]),
    CONSTRAINT [FK_Chapters_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([BookId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Chapters_Files_HtmlFileId] FOREIGN KEY ([HtmlFileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Chapters_Files_MarkdownFileId] FOREIGN KEY ([MarkdownFileId]) REFERENCES [Files] ([FileId]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_Bookmarks_BookId] ON [Bookmarks] ([BookId]);

GO

CREATE INDEX [IX_Bookmarks_UserId] ON [Bookmarks] ([UserId]);

GO

CREATE INDEX [IX_Books_CoverFileId] ON [Books] ([CoverFileId]);

GO

CREATE INDEX [IX_Books_EBookFileId] ON [Books] ([EBookFileId]);

GO

CREATE INDEX [IX_Books_MarkdownFileId] ON [Books] ([MarkdownFileId]);

GO

CREATE INDEX [IX_Books_ThumbnailFileId] ON [Books] ([ThumbnailFileId]);

GO

CREATE INDEX [IX_Chapters_HtmlFileId] ON [Chapters] ([HtmlFileId]);

GO

CREATE INDEX [IX_Chapters_MarkdownFileId] ON [Chapters] ([MarkdownFileId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190805221026_InitialSchema', N'2.2.1-servicing-10028');

GO

