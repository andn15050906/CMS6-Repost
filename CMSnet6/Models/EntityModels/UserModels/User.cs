using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSnet6.Models.EntityModels.UserModels
{
    public class User
    {
        [Column(TypeName = "VARCHAR(45)")]
        public string? Id { get; set; }

        [Column(TypeName = "NVARCHAR(45)")]
        public string? UserName { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string? Password { get; set; }

        [Column(TypeName = "VARCHAR(45)")]
        public string? Email { get; set; }

        [Column(TypeName = "VARCHAR(15)")]
        public string? Phone { get; set; }

        public DateTime JoinDate { get; set; }

        public Role Role { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string? Avatar { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string? Token { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string? Refresh { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsConfirmed { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
