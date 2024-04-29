namespace FileController.Model.Response
{
    public class FileCreateResponse
    {
        public string Token { get; set; }
        public string Path { get; set; }
        public Dictionary<Guid, string> CreatedFiles { get; set; }
    }
}
