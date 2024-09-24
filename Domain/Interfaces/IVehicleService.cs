using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null);
        Vehicle? GetVehicleById(int id);
        void CreateVehicle(Vehicle veiculo);
        void UpdateVehicle(Vehicle veiculo);
        void DeleteVehicleById(Vehicle veiculo);
    }
}
