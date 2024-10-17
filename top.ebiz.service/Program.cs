using Microsoft.EntityFrameworkCore;
using top.ebiz.service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// อ่าน connection string จาก appsettings.json
var connectionString = builder.Configuration.GetConnectionString("eBizConnection");

// กำหนดให้ใช้ TOPEBizEntities และเชื่อมต่อกับ Oracle Database
builder.Services.AddDbContext<TOPEBizEntities>(options =>
    options.UseOracle(connectionString));


// โหลดค่า settings จาก appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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
