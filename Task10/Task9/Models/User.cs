using System.ComponentModel.DataAnnotations;

namespace Task9.Models {
    public class User {
        [Key]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Salt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenTimer { get; set; }
    }
}
