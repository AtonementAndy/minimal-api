
using minimal_api.Domain.DTOs;
using System.Text;
using System.Text.Json;
using Test.Helpers;
using minimal_api.Domain.ModelViews;
using System.Net;
using minimal_api.Domain.Entities;
using Test.Mocks;
using minimal_api.Infrasctructure.Services;

namespace Test.Requests
{
    [TestClass]
    public class AdministratotRequestTest
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
        public async Task TestLogin()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com",
                Password = "123456"
            };
            
            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            // Act
            var response = await Setup.client.PostAsync("/administrators/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var loggedAdm = JsonSerializer.Deserialize<LoggedAdministrator>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(loggedAdm?.Email);
            Assert.IsNotNull(loggedAdm.Role);
            Assert.IsNotNull(loggedAdm.Token);
        }

        [TestMethod]
        public void TestGetAdministratorById()
        {
            //Assert
            var adm = new Administrator { Id = 1 };

            var admServiceMock = new AdministratorServiceMock();

            //Act
            var admFound = admServiceMock.GetAdministratorById(adm.Id);

            //Assert
            Assert.AreEqual(adm.Id, admFound.Id);
        }

        [TestMethod]
        public void TestGetAllAdministratorsMock()
        {
            //Assert
            var admServiceMock = new AdministratorServiceMock();

            // Act

            // Assert
            Assert.AreEqual(2, admServiceMock.GetAllAdministrators(2).Count);
        }

        [TestMethod]
        public void TestCreateMock()
        {
            //Arrange
            var adm = new Administrator
            {
                Id = 1,
                Email = "teste@teste.com",
                Password = "teste",
                Role = "Adm"
            };

            var admServiceMock = new AdministratorServiceMock();

            // Act
            var createdAdm = admServiceMock.Create(adm);

            // Assert
            Assert.AreEqual(adm, createdAdm);
        }
    }
}
