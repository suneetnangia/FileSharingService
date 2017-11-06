using System;

namespace Microsoft.Demos.FileSharingService.API.Models
{
    public class FileAccessPolicy
    {
        public TimeSpan TTL { get; set; }

        public FilePermissions Permissions { get; set; }

        public bool UseSourceIPRestriction { get; set; }

        public FileAccessProtocol Protocol { get; set; }
    }
}