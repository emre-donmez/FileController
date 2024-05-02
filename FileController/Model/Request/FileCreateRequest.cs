namespace FileController.Model
{
    public class FileCreateRequest
    {
        public string Token { get; set; }
        public IFormFile File { get; set; }

    }
}
