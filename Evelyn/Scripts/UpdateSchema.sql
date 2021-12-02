ALTER TABLE "Files" ALTER COLUMN "Timestamp" TYPE timestamp with time zone;
ALTER TABLE "Books" ALTER COLUMN "LastUpdated" TYPE timestamp with time zone;
ALTER TABLE "Chapters" ALTER COLUMN "LastUpdated" TYPE timestamp with time zone;
ALTER TABLE "Bookmarks" ALTER COLUMN "Timestamp" TYPE timestamp with time zone;
