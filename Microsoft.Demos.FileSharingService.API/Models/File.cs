namespace Microsoft.Demos.FileSharingService.API.Models
{
    public class File
    {
        public string Container { get; set; }

        public string Path { get; set; }

        public FilePermissions Permissions { get; set; }
    }
}