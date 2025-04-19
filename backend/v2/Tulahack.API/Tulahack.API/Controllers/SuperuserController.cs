using System.Globalization;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tulahack.API.Controllers;

[ApiController]
[Authorize(Policy = "Superuser")]
[Route("api/[controller]")]
public class SuperuserController : ControllerBase
{
    [HttpGet("getDatabaseCopy")]
    public FileResult GetDatabaseCopy()
    {
        var databaseFiles = new List<string>()
        {
            "/app/tulahack.db", "/app/tulahack.db-shm", "/app/tulahack.db-wal"
        };
        using var ms = new MemoryStream();

        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var path in databaseFiles)
            {
                ZipArchiveEntry entry = archive.CreateEntry(Path.GetFileName(path), CompressionLevel.Fastest);
                using Stream zipStream = entry.Open();
                var bytes = System.IO.File.ReadAllBytes(path);
                zipStream.Write(bytes, 0, bytes.Length);
            }
        }

        return File(ms.ToArray(), "application/zip",
            $"tulahack-{DateTime.Now.ToString(CultureInfo.InvariantCulture)}.zip");
    }
}
