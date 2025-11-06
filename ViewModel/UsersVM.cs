namespace DirectoryProject.ViewModel
{
    public class UsersVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? DOB { get; set; }
        public string? Qualification { get; set; }
        public string? RegNo { get; set; }
        public string? GTMDANo { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? file { get; set; }
        public bool IsAdmin { get; set; }
    }
}
