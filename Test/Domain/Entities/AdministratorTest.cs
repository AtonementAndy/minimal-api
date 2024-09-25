
using minimal_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorTest
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange
        var adm = new Administrator();

        // Act
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Password = "teste";
        adm.Role = "Adm";

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Password);
        Assert.AreEqual("Adm", adm.Role);
    }
}
