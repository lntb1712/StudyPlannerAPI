using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Permision;
using StudyPlannerAPI.Permission;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.FunctionRepository;
using StudyPlannerAPI.Repositories.GroupFunctionRepository;
using StudyPlannerAPI.Repositories.GroupManagementRepository;
using StudyPlannerAPI.Services.AccountManagementService;
using StudyPlannerAPI.Services.FunctionService;
using StudyPlannerAPI.Services.GroupFunctionService;
using StudyPlannerAPI.Services.GroupManagementService;
using StudyPlannerAPI.Services.JWTService;
using StudyPlannerAPI.Services.LoginService;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Thêm đoạn này để Render biết dùng PORT nó cấp
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port)); // lắng nghe trên PORT mà Render cấp
});


// Add services to the container.
// Đăng ký DBContext
builder.Services.AddDbContext<StudyPlannerContext>
    (option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

builder.Services.AddScoped<IAccountManagementRepository, AccountManagementRepository>();
builder.Services.AddScoped<IGroupFunctionRepository, GroupFunctionRepository>();
builder.Services.AddScoped<IGroupManagementRepository, GroupManagementRepository>();
builder.Services.AddScoped<IFunctionRepository,FunctionRepository>();
//Add service
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();
builder.Services.AddScoped<IGroupManagementService, GroupManagementService>();
builder.Services.AddScoped<IGroupFunctionService, GroupFunctionService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();

// Đăng ký IHttpContextAccessor để thực hiện sử dụng HttpCookie
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull; // bỏ qua các giá trị null khi api trả về kết quả
    }); // câu lệnh này có công dụng giữ nguyên tên thuộc tính được định nghĩa trong class C# (nhớ phải cài newtonsoftJson)

// Cấu hình Authentication JWT
var secretKey = builder.Configuration["AppSettings:SecretKey"];

// Map chuỗi SecretKey thành mảng byte để dùng thuật toán xét đối xứng
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey!);

// Add authentication JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.RequireHttpsMetadata = false; // Phát triển trên HTTP
        option.SaveToken = true;
        option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            // tự cấp token
            ValidateIssuer = false,
            ValidateAudience = false,

            // Ký vào token
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // dùng thuật toán SymmetricSecurityKey để kiểm tra đối xứng mảng secretKeyBytes

            ClockSkew = TimeSpan.Zero // Để giảm độ trễ khi kiểm tra token hết hạn

            //// Bảo mật hơn
            //ValidateIssuer = true, // Kiểm tra đúng API cấp token
            //ValidIssuer = issuer,

            //ValidateAudience = true, // Kiểm tra đúng frontend gửi request 
            //ValidAudience = audience,

            //// Ký vào token
            //ValidateIssuerSigningKey = true,
            //IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes), // dùng thuật toán SymmetricSecurityKey để kiểm tra đối xứng mảng secretKeyBytes

            //ValidateLifetime = true, // Kiểm tra hạn sử dụng token
            //ClockSkew = TimeSpan.Zero // Giảm độ trễ khi kiểm tra token hết hạn

        };
    });


// Add authorization JWT Bearer
//Thêm một policy duy nhất để kiểm tra quyền dựa trên claim "permissions":
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PermissionPolicy", policy =>
    {
        policy.Requirements.Add(new PermissionRequirement());
    });
});

/*
 * Singleton nghĩa là gì?
 * ASP.NET Core sẽ chỉ tạo một instance duy nhất của PermissionHandler trong suốt vòng đời của ứng dụng.
 * Tất cả các request đều dùng chung một instance này.
 * Giúp tối ưu hiệu suất và giảm chi phí tạo mới nhiều lần.
 */

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build
        
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // ✅ BỔ SUNG THÔNG TIN PHIÊN BẢN
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Chrome API",
        Description = "API documentation for Chrome"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey, // MUST be ApiKey for Swagger 2.0
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new string[] {}
        }
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
app.UseSwaggerUI(c => c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None));
}

app.UseHttpsRedirection();

app.UseCors("MyCors");

// sử dụng authen trước author
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
