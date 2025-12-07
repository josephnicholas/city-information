using Microsoft.AspNetCore.Authorization;

namespace CityInformation.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

[Route("api/files")]
[Authorize]
[ApiController]
public class FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider) : ControllerBase {
    readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider 
                                                                                  ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));

    [HttpGet("{fileId:int}")]
    public ActionResult Get(int fileId) {
        // Look up the actual file, depending on fileId
        // In this case let's just we are hardcoding it
        var pathToFile = "Joseph_Nicholas_Alcantara_CV.pdf";
        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound();
        }

        // Parses the content-type
        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
        {
            // default media type for binary data
            contentType = "application/octet-stream";
        }
        
        var bytes = System.IO.File.ReadAllBytes(pathToFile); // we stream the read data
        return File(bytes, contentType, Path.GetFileName(pathToFile));
    }

    // uploading files
    [HttpPost]
    public async Task<ActionResult> CreateFile(IFormFile file) {
        // Validate the input. Put a limit on the fileSize to avoid large uploads attach
        // only accept .pdf files (check content type)
        if (file.Length == 0 || file.Length > (1024 * 1024 * 1024) || file.ContentType != "application/pdf")
        {
            return BadRequest("No file or invalid file format.");
        }
        
        // Create the file path. Avoid using file.FileName as an attacker can provide a malicious one, including full paths,
        // and relative paths
        var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file_{Guid.NewGuid()}.pdf");

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        
        // test with form-data as Body
        return Ok("File uploaded successfully.");
    }

}
