namespace FileController.Model.Response
{
    public class FileCreateInfo
    {
        public Guid Id { get; set; }
        public string CreatedFileName { get; set; }
        public string Path { get; set; }
    }
}