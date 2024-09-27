using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Helpers;

namespace Test.Mocks
{
    public class VehicleServiceMock : IVehicleService
    {
        private static List<Vehicle> vehicles = new List<Vehicle>()
        {
            new Vehicle
            {
                Id = 1,
                Name = "Corolla",
                Brand = "Toyota",
                Year = 2022
            },

            new Vehicle
            {
                Id = 2,
                Name = "Civic",
                Brand = "Honda",
                Year = 2001
            },

            new Vehicle
            {
                Id = 3,
                Name = "Elantra",
                Brand = "Hyandai",
                Year = 2013
            },

        };

        public void CreateVehicle(Vehicle veiculo)
        {
            veiculo.Id = vehicles.Count() + 1;
            vehicles.Add(veiculo);
        }

        public void DeleteVehicleById(Vehicle veiculo)
        {
            vehicles.Remove(veiculo);
        }

        public List<Vehicle> GetAllVehicles(int? page = 1 + 1, string? name = null, string? brand = null)
        {
            var query = vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name}%"));
            }

            int itemsPerPage = 10;

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);
            }


            return query.ToList();
        }

        public Vehicle? GetVehicleById(int id)
        {
            return vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public void UpdateVehicle(Vehicle veiculo)
        {
            var existingVehicle = vehicles.FirstOrDefault(v => v.Id == veiculo.Id);
            if (existingVehicle == null)
            {
                throw new InvalidOperationException("Veículo não encontrado");
            }

            existingVehicle.Id = veiculo.Id;
            existingVehicle.Name = veiculo.Name;
            existingVehicle.Brand = veiculo.Brand;
            existingVehicle.Year = veiculo.Year;
        }
    }
}
