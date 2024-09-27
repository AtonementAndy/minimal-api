using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Infrasctructure.DB;
using minimal_api.Infrasctructure.Services;
using System.Reflection;

namespace Test.Domain.Services;

[TestClass]
public class AdministratorServiceTest
{
    private DbContexto CreateContextForTest()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestCreateAdministrator()
    {
        // Arrange
        var context = CreateContextForTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator
        {
            Id = 1,
            Email = "teste@teste.com",
            Password = "teste",
            Role = "Adm"
        };

        var administratorService = new AdministratorService(context);

        // Act
        var creatingAdm = administratorService.Create(adm);

        // Assert
        Assert.AreEqual(adm, creatingAdm);
    }

    [TestMethod]
    public void TestGetById()
    {
        // Arrange
        var context = CreateContextForTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator
        {
            Id = 1,
            Email = "teste@teste.com",
            Password = "teste",
            Role = "Adm"
        };

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Create(adm);
        var admFromBank = administratorService.GetAdministratorById(adm.Id);

        // Assert
        Assert.AreEqual(1, admFromBank.Id);
    }

    [TestMethod]
    public void TestGeAllAdministrators()
    {
        // Arrange
        var context = CreateContextForTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

        var adm = new Administrator
        {
            Id = 1,
            Email = "teste@teste.com",
            Password = "teste",
            Role = "Adm"
        };

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Create(adm);

        // Assert
        Assert.AreEqual(1, administratorService.GetAllAdministrators(1).Count());
    }
}
