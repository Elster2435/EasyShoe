using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyShoeDemo.Lib.Entities
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("login")]
        public string Login { get; set; } = null!;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;
        [Column("full_name")]
        public string FullName { get; set; } = null!;
        [Column("role_id")]
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
        public List<Order> Orders { get; set; } = new();
    }
}
