using minimal_api.Domain.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Opa, somente Teste!");

app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if(loginDTO.Email == "adm@test.com" && loginDTO.Senha == "123456")
    {
        return Results.Ok("Login com sucesso.");
    }
    else
    {
        return Results.Unauthorized();
    }
});

app.Run();

