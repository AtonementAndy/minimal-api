using minimal_api.Enums;

namespace minimal_api.Domain.DTOs
{
    public class AdministratorDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Roles? Role { get; set; }
    }
}
