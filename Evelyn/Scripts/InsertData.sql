ALTER SEQUENCE "Users_Id_seq" RESTART WITH 1000;
ALTER SEQUENCE "Books_Id_seq" RESTART WITH 1000;
ALTER SEQUENCE "Files_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Chapters_Id_seq" RESTART WITH 1000000;
ALTER SEQUENCE "Bookmarks_Id_seq" RESTART WITH 1000000;

INSERT INTO "Users" ("Name", "Hash") VALUES ('cysun', '$2a$11$Y87nF62c7gvg7fbAa6IYA.xgEaYssPTew0JysYlqM3agW/Yeecc2u');

ALTER TABLE "Books" ADD COLUMN tsv tsvector;

CREATE OR REPLACE FUNCTION books_ts_trigger_function() RETURNS TRIGGER AS $$
DECLARE
    l_content TEXT;
BEGIN
    SELECT LEFT(CONVERT_FROM("Content", 'UTF8'), 240000) INTO l_content FROM "Files"
        WHERE "Id" = new."MarkdownFileId";
    new.tsv = SETWEIGHT(TO_TSVECTOR(new."Title"), 'A') ||
              SETWEIGHT(TO_TSVECTOR(new."Author"), 'A') ||
              SETWEIGHT(TO_TSVECTOR(l_content), 'D');
    RETURN new;
END
$$ LANGUAGE plpgsql;

CREATE TRIGGER books_ts_trigger
    BEFORE INSERT OR UPDATE ON "Books"
    FOR EACH ROW EXECUTE PROCEDURE books_ts_trigger_function();

CREATE INDEX books_ts_index on "Books" using gin(tsv);
