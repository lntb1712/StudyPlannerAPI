using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyPlannerAPI.DTOs;
using StudyPlannerAPI.Hubs;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Permision;
using StudyPlannerAPI.Permission;
using StudyPlannerAPI.Repositories.AccountManagementRepository;
using StudyPlannerAPI.Repositories.AssignmentDetailRepository;
using StudyPlannerAPI.Repositories.AssignmentRepository;
using StudyPlannerAPI.Repositories.ClassRepository;
using StudyPlannerAPI.Repositories.FunctionRepository;
using StudyPlannerAPI.Repositories.GroupFunctionRepository;
using StudyPlannerAPI.Repositories.GroupManagementRepository;
using StudyPlannerAPI.Repositories.MessagingRepository;
using StudyPlannerAPI.Repositories.NotificationRepository;
using StudyPlannerAPI.Repositories.ReminderRepository;
using StudyPlannerAPI.Repositories.ScheduleRepository;
using StudyPlannerAPI.Repositories.StudentClassRepository;
using StudyPlannerAPI.Repositories.TaskManagementRepository;
using StudyPlannerAPI.Repositories.TeacherClassRepository;
using StudyPlannerAPI.Services.AccountManagementService;
using StudyPlannerAPI.Services.AssignmentDetailService;
using StudyPlannerAPI.Services.AssignmentService;
using StudyPlannerAPI.Services.ClassService;
using StudyPlannerAPI.Services.CloudinaryService;
using StudyPlannerAPI.Services.DashboardService;
using StudyPlannerAPI.Services.EmailService;
using StudyPlannerAPI.Services.FunctionService;
using StudyPlannerAPI.Services.GroupFunctionService;
using StudyPlannerAPI.Services.GroupManagementService;
using StudyPlannerAPI.Services.JWTService;
using StudyPlannerAPI.Services.LoginService;
using StudyPlannerAPI.Services.MessagingService;
using StudyPlannerAPI.Services.MonitorService;
using StudyPlannerAPI.Services.NotificationService;
using StudyPlannerAPI.Services.ReminderService;
using StudyPlannerAPI.Services.ScheduleService;
using StudyPlannerAPI.Services.StudentClassService;
using StudyPlannerAPI.Services.TaskManagementService;
using StudyPlannerAPI.Services.TeacherClassService;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

////// Thêm đoạn này để Render biết dùng PORT nó cấp
//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(int.Parse(port)); // lắng nghe trên PORT mà Render cấp
//});


// Add services to the container.
// Đăng ký DBContext
//builder.Services.AddDbContext<StudyPlannerContext>
//    (option =>
//    {
//        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//    });
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DB_CONNECTION");
builder.Services.AddDbContext<StudyPlannerContext>(options =>
    options.UseSqlServer(connectionString));
//// Load Email Settings
//builder.Services.Configure<EmailSettings>(options =>
//{
//    options.SmtpServer = builder.Configuration["EmailSettings:SmtpServer"]
//        ?? Environment.GetEnvironmentVariable("EmailSettings__SmtpServer") ?? "smtp.gmail.com";

//    options.SmtpPort = int.TryParse(
//        builder.Configuration["EmailSettings:SmtpPort"]
//        ?? Environment.GetEnvironmentVariable("EmailSettings__SmtpPort"), out var port) ? port : 587;

//    options.SenderEmail = builder.Configuration["EmailSettings:SenderEmail"]!
//        ?? Environment.GetEnvironmentVariable("EmailSettings__SenderEmail")!;

//    options.SenderPassword = builder.Configuration["EmailSettings:SenderPassword"]!
//        ?? Environment.GetEnvironmentVariable("EmailSettings__SenderPassword")!;
//});

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
var CloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
CloudinarySettings!.CloudName = Environment.GetEnvironmentVariable("CloudinarySettings__CloudName")!;
CloudinarySettings!.ApiKey = Environment.GetEnvironmentVariable("CloudinarySettings__ApiKey")!;
CloudinarySettings!.ApiSecret = Environment.GetEnvironmentVariable("CloudinarySettings__ApiSecret")!;

// Add SignalR
builder.Services.AddSignalR();

// Thiết lập Redis Cache (IDistributedCache)
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccountManagementRepository, AccountManagementRepository>();
builder.Services.AddScoped<IGroupFunctionRepository, GroupFunctionRepository>();
builder.Services.AddScoped<IGroupManagementRepository, GroupManagementRepository>();
builder.Services.AddScoped<IFunctionRepository,FunctionRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IStudentClassRepository, StudentClassRepository>();
builder.Services.AddScoped<ITeacherClassRepository, TeacherClassRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentDetailRepository, AssignmentDetailRepository>();
builder.Services.AddScoped<ITaskManagementRepository,TaskManagementRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMessagingRepository, MessagingRepository>();

//Add service
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IAccountManagementService, AccountManagementService>();
builder.Services.AddScoped<IGroupManagementService, GroupManagementService>();
builder.Services.AddScoped<IGroupFunctionService, GroupFunctionService>();
builder.Services.AddScoped<IFunctionService, FunctionService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IClassService,ClassService>();
builder.Services.AddScoped<IStudentClassService,StudentClassService>();
builder.Services.AddScoped<ITeacherClassService, TeacherClassService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IAssignmentService,  AssignmentService>();
builder.Services.AddScoped<IAssignmentDetailService, AssignmentDetailService>();
builder.Services.AddScoped<ITaskManagementService,TaskManagementService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

//Add Monitor service
builder.Services.AddHostedService<ScheduleMonitorService>();
builder.Services.AddHostedService<ReminderMonitorService>();
builder.Services.AddHostedService<AssignmentMonitorService>();
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
    build.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader();
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
        Title = "StudyPlanner API",
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

// Map hub
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
