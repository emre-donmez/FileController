namespace FileController.Model.Response
{
    public class FileCreateResponse
    {
        public string Token { get; set; }
        public List<FileCreateInfo> Files { get; set; }
    }
}

