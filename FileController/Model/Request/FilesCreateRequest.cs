namespace FileController.Model.Request
{
    public class FilesCreateRequest
    {
        public string Token { get; set; }
        public List<IFormFile> Files { get; set; }
        public string Path { get; set; }
    }
}
