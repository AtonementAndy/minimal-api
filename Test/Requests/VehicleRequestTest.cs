using minimal_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Helpers;
using Test.Mocks;

namespace Test.Requests
{
    [TestClass]
    public class VehicleRequestTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Setup.ClassInit(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanUp();
        }

        [TestMethod]
        public void TestCreateMock()
        {
            //Arrange
            var vehicle = new Vehicle
            {
                Id = 4,
                Name = "Fusca",
                Brand = "Volkswagen",
                Year = 1970
            };

            var vehicleServiceMock = new VehicleServiceMock();

            // Act
            vehicleServiceMock.CreateVehicle(vehicle);

            // Assert
            Assert.AreEqual(vehicle, vehicleServiceMock.GetVehicleById(4));
        }

        [TestMethod]
        public void TestGetAllVehiclesMock()
        {
            //Assert
            var vehicleServiceMock = new VehicleServiceMock();

            // Act

            // Assert
            Assert.AreEqual(3, vehicleServiceMock.GetAllVehicles(1).Count);
        }

        [TestMethod]
        public void TestDeleteMock()
        {
            //Arrange
            var vehicle = new Vehicle
            {
                Id = 4,
                Name = "Fusca",
                Brand = "Volkswagen",
                Year = 1970
            };

            var vehicleServiceMock = new VehicleServiceMock();

            // Act
            vehicleServiceMock.DeleteVehicleById(vehicle);

            var deleteResult = vehicleServiceMock.GetVehicleById(vehicle.Id);

            // Assert
            Assert.AreNotEqual(vehicle, deleteResult);
        }

        [TestMethod]
        public void TestGetVehicleById()
        {
            //Arrange
            var vehicle = new Vehicle { Id = 1 };

            var vehicleServiceMock = new VehicleServiceMock();

            //Act
            var vehicleById = vehicleServiceMock.GetVehicleById(vehicle.Id);

            //Assert
            Assert.AreEqual(vehicle.Id, vehicleById.Id);
        }

        [TestMethod]
        public void TestUpdateVehicle()
        {
            //Arrange
            var vehicle = new Vehicle
            {
                Id = 1,
                Name = "Civic",
                Brand = "Honda",
                Year = 2022
            };

            var vehicleServiceMock = new VehicleServiceMock();

            //Act
            vehicleServiceMock.UpdateVehicle(vehicle);

            //Assert
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Civic", vehicle.Name);
            Assert.AreEqual("Honda", vehicle.Brand);
            Assert.AreEqual(2022, vehicle.Year);
        }
    }
}
