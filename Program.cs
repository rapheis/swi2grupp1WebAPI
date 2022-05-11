var builder = WebApplication.CreateBuilder(args);

// Variablen aus appsetting.json holen
swi2grupp1WebAPI.AppConfiguration.AzureServiceBusConnectionString = builder.Configuration["AzureServiceBusConnectionString"];
swi2grupp1WebAPI.AppConfiguration.AzureServiceBusRequests = builder.Configuration["AzureServiceBusRequests"];
swi2grupp1WebAPI.AppConfiguration.AzureServiceBusResponse = builder.Configuration["AzureServiceBusResponse"];

// für CosmosDB

swi2grupp1WebAPI.AppConfiguration.CosmosEndpointUri = builder.Configuration["CosmosEndpointUri"];
swi2grupp1WebAPI.AppConfiguration.CosmosPrimaryKey = builder.Configuration["CosmosPrimaryKey"];


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
