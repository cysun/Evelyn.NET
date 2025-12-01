ALTER TABLE "Files"
  ALTER COLUMN "Name" TYPE character varying(255),
  ALTER COLUMN "Name" SET NOT NULL;

ALTER TABLE "Files"
  ALTER COLUMN "ContentType" TYPE character varying(255),
  ALTER COLUMN "ContentType" SET NOT NULL;

ALTER TABLE "Users"
  ALTER COLUMN "Name" TYPE character varying(255);
ALTER TABLE "Users"
  ALTER COLUMN "Hash" TYPE character varying(1000);

ALTER TABLE "Books"
  ALTER COLUMN "Title" TYPE character varying(255);
ALTER TABLE "Books"
  ALTER COLUMN "Author" TYPE character varying(255);
ALTER TABLE "Books"
  ALTER COLUMN "Notes" TYPE character varying(8000);

ALTER TABLE "Chapters"
  ALTER COLUMN "Name" TYPE character varying(255);

DELETE
FROM "__EFMigrationsHistory";
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251201001634_InitialSchema', '10.0.0');
