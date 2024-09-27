using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using minimal_api;
using minimal_api.Domain.Interfaces;
using Test.Mocks;

namespace Test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        
        public static void ClassInit(TestContext testContext)
        {
            Setup.testContext = testContext;
            http = new WebApplicationFactory<Startup>();

            http = http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", PORT).UseEnvironment("testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministratorService, AdministratorServiceMock>();
                    services.AddScoped<IVehicleService, VehicleServiceMock>();

                    //Caso queira deixar o teste com conexão diferente
                    //var connection = "Server=localhost;Database=desafio21dias_dotnet7_test;Uid=root;Pwd=root";
                    //services.AddDbContext<DbContexto>(options =>
                    //{
                    //    options.UseMySql(connection, ServerVersion.AutoDetect(connection));
                    //});
                });
            });

            client = http.CreateClient();
        }

        public static void ClassCleanUp()
        {
            http.Dispose();
        }
        
        
        //public static async Task ExecuteCommandSqlAsync(string sql)
        //{
        //    await new DbContexto().Database.ExecuteSqlRawAsync(sql);
        //}

        //public static async Task<int> ExecuteCommandSqlAsync(int id, string name)
        //{
        //    return await new DbContexto().Clientes.Where(c => c.Id == id && c.Name == name).ExecuteAsync();
        //}

        //public static async Task FakeClientAsync()
        //{
        //    await new DbContexto.Database.ExecuteSqlRawAsync("""insert clientes(Nome, Telefone, Email, DataCriacao" values('Danilo', '(11)11111-1111', 'email@teste.com', '2024-09-26 06:09:00'""");
               
        //}
    }
}
