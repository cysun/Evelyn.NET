using Evelyn.Services;
using Microsoft.AspNetCore.Mvc;

namespace Evelyn.Controllers;

public class FilesController : Controller
{
    private readonly FileService _fileService;

    public FilesController(FileService fileService)
    {
        _fileService = fileService;
    }

    public IActionResult View(int id)
    {
        var file = _fileService.GetFile(id);
        return File(file.OpenReadStream(), file.ContentType);
    }

    public IActionResult Download(int id)
    {
        var file = _fileService.GetFile(id);
        return File(file.OpenReadStream(), file.ContentType, file.Name);
    }
}
