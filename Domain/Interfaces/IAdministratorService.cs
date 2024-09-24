using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
        Administrator Create(Administrator administrator);
        List<Administrator> GetAllAdministrators(int? pages);

        Administrator? GetAdministratorById(int id);

    }
}
