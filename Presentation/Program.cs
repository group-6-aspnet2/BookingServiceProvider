using Azure.Messaging.ServiceBus;
using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Presentation.GrpcServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddGrpc();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddScoped<IBookingStatusRepository, BookingStatusRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddSingleton<ServiceBusClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
});

builder.Services.AddTransient<IInvoiceServiceBusHandler, InvoiceServiceBusHandler>();
builder.Services.AddScoped<IBookingServiceBusListener, BookingServiceBusListener>();

var app = builder.Build();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapGrpcService<BookingGrpcService>();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthorization();
app.MapControllers();
app.Run();



//builder.Services.AddGrpcClient<EventContract.EventContractClient>(x =>
//{
//    x.Address = new Uri(builder.Configuration["GrpcClients:EventService"]!);
//});
//builder.Services.AddGrpcClient<UserContract.UserContractClient>(x =>
//{
//    x.Address = new Uri(builder.Configuration["GrpcClients:UserService"]!);
//});