using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Infrasctructure.DB;
using minimal_api.Infrasctructure.Services;
using System.Reflection;
using Test.Mocks;

namespace Test.Domain.Services
{
    [TestClass]
    public class VehicleServiceTest
    {

        private DbContexto CreateContextForTest()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(path ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var context = builder.Build();

            return new DbContexto(context);
        }

        [TestMethod]
        public void TestCreateVehicle()
        {
            // Arrange
            var context = CreateContextForTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "BMW I8",
                Brand = "BMW",
                Year = 2018
            };

            var vehicleService = new VehicleService(context);
            vehicleService.CreateVehicle(vehicle);

            Assert.AreEqual(1, vehicleService.GetAllVehicles().Count);
        }

        [TestMethod]
        public void TestVehicleGetById()
        {
            // Arrange
            var context = CreateContextForTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "BMW I8",
                Brand = "BMW",
                Year = 2018
            };

            var vehicleService = new VehicleService(context);

            // Act
            vehicleService.CreateVehicle(vehicle);
            var vehicleFromBank = vehicleService.GetVehicleById(vehicle.Id);

            // Assert
            Assert.AreEqual(1, vehicleFromBank.Id);
        }

        [TestMethod]
        public void TestGetAllVehicles()
        {
            // Arrange
            var context = CreateContextForTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "BMW I8",
                Brand = "BMW",
                Year = 2018
            };

            var vehicleService = new VehicleService(context);

            // Act
            vehicleService.CreateVehicle(vehicle);

            // Assert
            Assert.AreEqual(1, vehicleService.GetAllVehicles(1).Count);
        }

        [TestMethod]
        public void TestDeleteVehicle()
        {
            // Arrange
            var context = CreateContextForTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "BMW I8",
                Brand = "BMW",
                Year = 2018
            };

            var vehicleService = new VehicleService(context);

            //Act
            vehicleService.DeleteVehicleById(vehicle);

            var deleteResult = vehicleService.GetVehicleById(vehicle.Id);

            //Assert
            Assert.IsNull(deleteResult);
        }

        [TestMethod]
        public void TestUpdateVehicle()
        {
            // Arrange
            var context = CreateContextForTest();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "BMW I8",
                Brand = "BMW",
                Year = 2018
            };

            var vehicleService = new VehicleService(context);

            //Act
            vehicleService.UpdateVehicle(vehicle);

            //Assert
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("BMW I8", vehicle.Name);
            Assert.AreEqual("BMW", vehicle.Brand);
            Assert.AreEqual(2018, vehicle.Year);
        }
    }
}
