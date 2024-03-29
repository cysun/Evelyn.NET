namespace Evelyn.Services;

public class FileService
{
    private readonly AppDbContext _db;

    public FileService(AppDbContext db)
    {
        _db = db;
    }

    public Models.File GetFile(int? id)
    {
        if (id == null) throw new ArgumentNullException();
        return _db.Files.Find(id);
    }

    public void DeleteFiles(List<int> ids)
    {
        var filesToDelete = ids.Select(id => new Models.File { Id = id });
        _db.Files.RemoveRange(filesToDelete);
        _db.SaveChanges();
    }

    public void SaveChanges() => _db.SaveChanges();
}
