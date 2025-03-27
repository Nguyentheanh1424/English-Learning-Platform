using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

using System.Text;

using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// 1. Cấu hình từ appsettings.json
// Đọc phần cấu hình MongoDB và JWT từ appsettings.json
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// 2. Đăng ký các dịch vụ (Services) với Dependency Injection (DI)
// Đăng ký MongoDbContext để quản lý kết nối MongoDB
builder.Services.AddSingleton<MongoDbContext>();

// Đăng ký UserService để xử lý logic liên quan đến người dùng
builder.Services.AddScoped<UserService>();

// Đăng ký AuthService để xử lý đăng nhập, tạo JWT
builder.Services.AddScoped<AuthService>();

// 3. Thêm Controller và Swagger (dùng cho tài liệu API)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Cấu hình Swagger để hỗ trợ JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// 5. Cấu hình Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    // Hiển thị Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔹 Xác thực người dùng
app.UseAuthentication();

// 🔹 Kiểm tra quyền truy cập của người dùng 
app.UseAuthorization(); 

// 🔹 Kiểm tra JWT Token trước khi vào Controller
app.UseMiddleware<JwtMiddleware>(); 

app.MapControllers();

app.Run();