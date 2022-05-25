ALTER TABLE "Books" ADD COLUMN "LastViewed" timestamp with time zone NULL;

CREATE OR REPLACE FUNCTION last_viewed() RETURNS trigger AS $$
DECLARE
    l_book_id integer;
BEGIN
    IF (TG_OP = 'DELETE') THEN
        SELECT "BookId" INTO l_book_id FROM "Chapters" WHERE "Id" = OLD."ChapterId";
        UPDATE "Books" SET "LastViewed" = null WHERE "Id" = l_book_id;
    ELSE
        SELECT "BookId" INTO l_book_id FROM "Chapters" WHERE "Id" = NEW."ChapterId";
        UPDATE "Books" SET "LastViewed" = CURRENT_TIMESTAMP WHERE "Id" = l_book_id;
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER last_viewed AFTER INSERT OR UPDATE OR DELETE ON "Bookmarks"
    FOR EACH ROW EXECUTE FUNCTION last_viewed();
