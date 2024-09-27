using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrasctructure.DB;

namespace minimal_api.Infrasctructure.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DbContexto _context;
        public VehicleService(DbContexto context)
        {
            _context = context;
        }

        public void CreateVehicle(Vehicle veiculo)
        {
            _context.Veiculos.Add(veiculo);
            _context.SaveChanges();
        }

        public void DeleteVehicleById(Vehicle veiculo)
        {
            var existingVehicle = _context.Veiculos.Find(veiculo.Id);
            if (existingVehicle != null)
            {
                _context.Veiculos.Remove(veiculo);
                _context.SaveChanges();
            }
                
        }

        public List<Vehicle> GetAllVehicles(int? page = 1, string? name = null, string? brand = null)

        {
            var query = _context.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
            }

            int itemsPerPage = 10;

            if(page != null)
            {
                query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
            }
            
            
            return query.ToList();
        }

        public Vehicle? GetVehicleById(int id)
        {
            return _context.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public void UpdateVehicle(Vehicle veiculo)
        {
            var existingVehicle = _context.Veiculos.Find(veiculo.Id);
            if (existingVehicle != null)
            {
                _context.Veiculos.Update(veiculo);
                _context.SaveChanges();
            }
        }
    }
}
