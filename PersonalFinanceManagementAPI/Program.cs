using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using PersonalFinanceManagementAPI.Database;
using PersonalFinanceManagementAPI.Database.Repositories;
using PersonalFinanceManagementAPI.Models;
using PersonalFinanceManagementAPI.Services;
using System.Reflection;
using System.Text.Json.Serialization;

var myLocalHostPolicy = "MyCORSPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myLocalHostPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader();
    }

    );
}
);

// Add services to the container.


builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();

builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();




// AutoMapper definition
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase)
        );
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Personal Finance Management API",
        Version = "v1",
        Description = "Personal Finance Management API allows analyze of a client's spending patterns against pre-defined budgets over time",
       
});

   

});


//DBContext registration
builder.Services.AddDbContext<PFMDbContext>(opt =>
{
    opt.UseNpgsql(CreateConnectionString(builder.Configuration));
});


var app = builder.Build();

app.UseRouting();






app.UseAuthorization();
app.MapControllers();
app.UseCors(myLocalHostPolicy);

/*
app.UseEndpoints(endpoints =>
{


    endpoints.MapControllerRoute(
        name: "transactions",
        pattern: "transactions/{action=GetTransactions}/{id?}",
        defaults: new { controller = "Transactions" });

    endpoints.MapControllerRoute(
        name: "categories",
        pattern: "categories/{action=GetCategories}/{id?}",
        defaults: new { controller = "Categories" });


    endpoints.MapControllers();
});
*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
    scope.ServiceProvider.GetRequiredService<PFMDbContext>().Database.Migrate();

}


app.Run();

string CreateConnectionString(IConfiguration configuration)
{
    var username = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "sa";
    var pass = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "Pass";
    var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "pfm";
    var host = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";

    var connBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = host,
        Port = int.Parse(port),
        Username = username,
        Database = databaseName,
        Password = pass,
        Pooling = true,

    };

    return connBuilder.ConnectionString;
}
