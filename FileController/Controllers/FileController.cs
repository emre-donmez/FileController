using FileController.Model;
using FileController.Model.Request;
using FileController.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace FileController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] FileCreateRequest fileCreateRequest)
        {
            if (fileCreateRequest == null || fileCreateRequest.Token == null || fileCreateRequest.File == null)
                return BadRequest("Missing required fields: Token, File, Path");

            try
            {
                Guid fileId = Guid.NewGuid();

                FileInfo fileInfo = new FileInfo(fileCreateRequest.File.FileName);

                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileCreateRequest.Token, fileCreateRequest.Path.TrimStart('/'));

                if (!Directory.Exists(folderPath))
                    return BadRequest("There is no folder for this path, create a path first.");

                string fileName = $"{fileId}{fileInfo.Extension}";

                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileCreateRequest.File.CopyToAsync(stream);
                }

                var filesDictionary = new Dictionary<Guid, string> { { fileId, fileCreateRequest.File.FileName } };
                var response = new FileCreateResponse
                {
                    Token = fileCreateRequest.Token,
                    Path = fileCreateRequest.Path,
                    CreatedFiles = filesDictionary
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while processing the request: {e.Message}");
            }
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles([FromForm] FilesCreateRequest fileCreateRequest)
        {
            if (fileCreateRequest == null || fileCreateRequest.Token == null || fileCreateRequest.Files.Count == 0)
                return BadRequest("Missing required fields: Token, Files, Path");

            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileCreateRequest.Token, fileCreateRequest.Path.TrimStart('/'));

                if (!Directory.Exists(folderPath))
                    return BadRequest("There is no folder for this path, create a path first.");

                var filesDictionary = new Dictionary<Guid, string>();

                foreach (var file in fileCreateRequest.Files)
                {
                    Guid fileId = Guid.NewGuid();

                    FileInfo fileInfo = new FileInfo(file.FileName);

                    string fileName = $"{fileId}{fileInfo.Extension}";

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    filesDictionary.Add(fileId, file.FileName);
                }

                var response = new FileCreateResponse
                {
                    Token = fileCreateRequest.Token,
                    Path = fileCreateRequest.Path,
                    CreatedFiles = filesDictionary
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while processing the request: {e.Message}");
            }
        }

        [HttpPost("CreatePath")]
        public async Task<IActionResult> CreatePath([FromBody] PathCreateRequest pathCreateRequest)
        {
            string tokenPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathCreateRequest.Token);

            if (!Directory.Exists(tokenPath))
                return BadRequest("There is no folder for this token, create a token first.");

            string folderPath = Path.Combine(tokenPath, pathCreateRequest.Path.TrimStart('/'));

            if (Directory.Exists(folderPath))
                return BadRequest("This path already exists");

            var result = Directory.CreateDirectory(folderPath);

            return Ok(pathCreateRequest.Path);
        }

        [HttpPost("CreateToken")]
        public async Task<IActionResult> CreateToken()
        {
            Guid guid = Guid.NewGuid();

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", guid.ToString());

            Directory.CreateDirectory(folderPath);

            return Ok(guid);
        }
        private async Task<IActionResult> GetFolderContent(string token, string path = "")
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", token, path);

            if (!Directory.Exists(folderPath))
                return NotFound();

            var files = Directory.GetFiles(folderPath).Select(Path.GetFileName).ToArray();
            var directories = Directory.GetDirectories(folderPath).Select(Path.GetFileName).ToArray();

            var folderContent = new
            {
                Files = files,
                Directories = directories
            };

            return Ok(folderContent);
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetFile(string token)
        {
            return await GetFolderContent(token);
        }

        [HttpGet("{token}/{path}")]
        public async Task<IActionResult> GetFile(string token, string path)
        {
            return await GetFolderContent(token, path);
        }

        [HttpGet("{token}/{path}/{id}")]
        public async Task<IActionResult> GetFile(string token, string path, string id)
        {
            try
            {
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", token, path);

                if (!Directory.Exists(directoryPath))
                    return NotFound("Directory not found.");

                string[] fileNames = Directory.GetFiles(directoryPath, $"{id}.*");

                if (fileNames.Length == 0)
                    return NotFound("File not found.");

                string filePath = fileNames[0];

                var fileInfo = new FileInfo(filePath);             

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fileInfo.Name, out string mimeType))
                    mimeType = "application/octet-stream";

                return File(fileStream, mimeType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}