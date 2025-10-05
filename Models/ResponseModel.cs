namespace DirectoryProject.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? data { get; set; }
    }
}
