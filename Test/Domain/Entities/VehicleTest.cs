using minimal_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Test.Domain.Entities
{
    [TestClass]
    public class VehicleTest
    {
        [TestMethod]
        public void TestGetSetProperties()
        {
            // Arrange
            var vehicle = new Vehicle();

            // Act
            vehicle.Id = 1;
            vehicle.Name = "Corolla";
            vehicle.Brand = "Toyota";
            vehicle.Year = 2000;

            // Assert
            Assert.AreEqual(1, vehicle.Id);
            Assert.AreEqual("Corolla", vehicle.Name);
            Assert.AreEqual("Toyota", vehicle.Brand);
            Assert.AreEqual(2000, vehicle.Year);
        }
    }
}
