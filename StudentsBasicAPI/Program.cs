using Microsoft.EntityFrameworkCore;
using StudentsBasicAPI.Configurations;
using StudentsBasicAPI.Data;
using StudentsBasicAPI.IRepository;
using StudentsBasicAPI.Models;
using StudentsBasicAPI.Repository;
using AutoMapper;
using StudentsBasicAPI;
using StudentsBasicAPI.Services;
using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureIdentity();

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

var producerConfig = new ProducerConfig();
builder.Configuration.Bind("producer", producerConfig);
builder.Services.AddSingleton<ProducerConfig>(producerConfig);

builder.Services.AddAutoMapper(typeof(MapperInitializer));

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    SeedData.Initialize(services);
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
