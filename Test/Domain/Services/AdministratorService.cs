

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Infrasctructure.DB;

namespace Test.Domain.Services;

[TestClass]
public class AdministratorServiceTest
{
    private DbContextMinimal CreateContextForTest()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsetting.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("MySql");

        var options = new DbContextOptionsBuilder<DbContextMinimal>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new DbContextMinimal(configuration, options);
    }

    [TestMethod]
    public void TestTryingToSaveAdministrator()
    {
        // Arrange
        var adm = new Administrator();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Password = "teste";
        adm.Role = "Adm";

        // Act
        var context = new CreateContextForTest();

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Password);
        Assert.AreEqual("Adm", adm.Role);
    }
}
