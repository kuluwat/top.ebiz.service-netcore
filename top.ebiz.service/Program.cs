using Microsoft.EntityFrameworkCore;
using top.ebiz.service;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//// เพิ่มการตั้งค่า Swagger/OpenAPI
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "eBiz API",
//        Version = "v1"
//    });
//});

//// อ่าน connection string จาก appsettings.json
//var connectionString = builder.Configuration.GetConnectionString("eBizConnection");

//// กำหนดให้ใช้ TOPEBizEntities และเชื่อมต่อกับ Oracle Database
//builder.Services.AddDbContext<TOPEBizCreateTripEntities>(options =>
//    options.UseOracle(connectionString));

//// โหลดค่า settings จาก appsettings.json
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//var app = builder.Build();

//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
//app.UseDeveloperExceptionPage(); // สำหรับการแสดงผลข้อผิดพลาดในโหมดพัฒนา
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "eBiz API V1");
//    c.RoutePrefix = string.Empty;  // ตั้งค่า Swagger ให้ทำงานที่ root URL (https://localhost:7098/)
//    c.DefaultModelsExpandDepth(-1); // ไม่ขยาย models ในหน้า Swagger
//});
////}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("https://localhost:7098/") // Update this with the Angular app URL
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Add Swagger generation
builder.Services.AddSwaggerGen(); // Register Swagger generator

// Add controllers and other services
builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

app.UseSwagger(); // Enable Swagger before UseCors and UseAuthorization
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "eBiz API V1");
    c.RoutePrefix = string.Empty;
    c.DefaultModelsExpandDepth(-1); // ไม่ขยาย models ในหน้า Swagger
});

app.UseCors("AllowAngularApp"); // Apply the CORS policy
//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();