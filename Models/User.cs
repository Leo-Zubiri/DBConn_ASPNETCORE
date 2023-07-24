using System.ComponentModel.DataAnnotations.Schema;

namespace DBConn_ASPNETCORE.Models
{
    public class User
    {
        public int Id_Usuario { get; set; }
        public string? UserName { get; set; }
        public string? Clave { get; set; }

        [NotMapped] public bool KeepActive { get; set; }
    }
}
