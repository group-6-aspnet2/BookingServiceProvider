using Azure.Messaging.ServiceBus;
using Business;
using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Presentation.GrpcServices;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v.1.0",
        Title = "BookingService API Documentation",
        Description = "Official documentation for Booking Service Provider API."

    });

    o.EnableAnnotations();
    o.ExampleFilters();

});

builder.Services.AddGrpc();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddScoped<IBookingStatusRepository, BookingStatusRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingStatusService, BookingStatusService>();
builder.Services.AddSingleton<ServiceBusClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ServiceBusClient(configuration["AzureServiceBusSettings:ConnectionString"]);
});

//builder.Services.AddHostedService<UpdateBookingQueueBackgroundService>(); // listener måste vara bortkommenterad för att kunna komma till swaggerUI. SB är borttagen från azure
builder.Services.AddScoped<IInvoiceServiceBusHandler, InvoiceServiceBusHandler>(); // publisher
builder.Services.AddScoped<ITicketServiceBusHandler, TicketServiceBusHandler>(); // publisher
builder.Services.AddMemoryCache();

builder.Services.AddGrpcClient<EventContract.EventContractClient>(x =>
{
    x.Address = new Uri(builder.Configuration["GrpcClients:EventService"]!);
});
builder.Services.AddHttpClient<IEmailRestService, EmailRestService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["HttpClients:EmailService"]!);
});
builder.Services.AddHttpClient<IAccountRestService, AccountRestService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["HttpClients:AccountService"]!);
});
builder.Services.AddHttpClient<IProfileRestService, ProfileRestService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["HttpClients:ProfileService"]!);
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapGrpcService<BookingGrpcService>();
app.MapOpenApi();
app.UseSwagger();// kom ihåg efter mapOpenApi
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking Service API - v.1.0");
    c.RoutePrefix = string.Empty; // kommer direkt till swaggersidan när jag startar upp applikationen
});
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.MapControllers();
app.Run();


