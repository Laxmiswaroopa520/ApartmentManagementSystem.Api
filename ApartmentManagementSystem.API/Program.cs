
using FastEndpoints;
using FastEndpoints.Swagger;
using ApartmentManagementSystem.API.Extensions;
using ApartmentManagementSystem.API.Filters;
using ApartmentManagementSystem.API.Middlewares;
using ApartmentManagementSystem.API.Policies;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Infrastructure;
using ApartmentManagementSystem.Infrastructure.Email;
using ApartmentManagementSystem.Infrastructure.OTP;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//UNIT OF WORK
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//INFRASTRUCTURE SERVICES
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();  // needed by UnitOfWork → StaffMemberRepository
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();

// OtpService is in Infrastructure but now also uses IUnitOfWork
builder.Services.AddScoped<IOtpService, OtpService>();

// APPLICATION SERVICES
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

//JWT AUTHENTICATION
builder.Services.AddJwtAuthentication(builder.Configuration);

//AUTHORIZATION POLICIES
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});

//FASTENDPOINTS
builder.Services.AddFastEndpoints();

//CONTROLLERS 
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

//SWAGGER
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Apartment Management System API";
        s.Version = "v1";
        s.Description = "Complete API Documentation - Controllers and FastEndpoints";
    };

    o.EndpointFilter = ep => true;
    o.EnableJWTBearerAuth = true;
    o.ShortSchemaNames = true;
    o.RemoveEmptyRequestSchema = true;
    o.TagCase = TagCase.TitleCase;
    o.AutoTagPathSegmentIndex = 1;
});

/*
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["WebAppUrl"] ?? "http://localhost:5002")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
*/

//HTTP CONTEXT
builder.Services.AddHttpContextAccessor();

//BUILD APP
var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowWebApp");
app.UseAuthentication();
app.UseAuthorization();

//SWAGGER UI (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

// ENDPOINTS
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});

app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Apply any pending EF migrations first
    dbContext.Database.Migrate();

    // Then seed master data — safe on every startup
    var seeder = new DatabaseSeeder(dbContext);
    await seeder.SeedAsync();
}
/*
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
*/
app.Run();

// Makes Program accessible for integration tests
public partial class Program { }

















/*
using FastEndpoints;
using FastEndpoints.Swagger;
using ApartmentManagementSystem.API.Extensions;
using ApartmentManagementSystem.API.Filters;
using ApartmentManagementSystem.API.Middlewares;
using ApartmentManagementSystem.API.Policies;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Infrastructure;
using ApartmentManagementSystem.Infrastructure.Email;
using ApartmentManagementSystem.Infrastructure.OTP;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── DATABASE ───────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── UNIT OF WORK ───────────────────────────────────────────────────
// ONE registration — replaces all 12 individual AddScoped<IRepo, Repo>() calls.
// All services inject IUnitOfWork. All repos share one DbContext per request.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── REPO BRIDGES FOR FASTENDPOINTS ─────────────────────────────────
// FastEndpoints that still inject individual repositories directly
// resolve them from the existing IUnitOfWork — same DbContext, no duplication.
builder.Services.AddScoped<IRoleRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Roles);
builder.Services.AddScoped<IUserRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Users);
builder.Services.AddScoped<IApartmentRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Apartments);
builder.Services.AddScoped<IFlatRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Flats);
builder.Services.AddScoped<IFloorRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Floors);
builder.Services.AddScoped<IUserFlatMappingRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().UserFlatMappings);
builder.Services.AddScoped<IUserInviteRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().UserInvites);
builder.Services.AddScoped<IUserOtpRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().UserOtps);
builder.Services.AddScoped<ICommunityMemberRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().CommunityMembers);
builder.Services.AddScoped<IStaffMemberRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().StaffMembers);
builder.Services.AddScoped<IResidentManagementRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Residents);
builder.Services.AddScoped<IEnhancedDashboardRepository>(sp =>
    sp.GetRequiredService<IUnitOfWork>().Dashboard);

// ── INFRASTRUCTURE SERVICES ────────────────────────────────────────
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();  // needed by UnitOfWork → StaffMemberRepository
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();

// OtpService is in Infrastructure but now also uses IUnitOfWork
builder.Services.AddScoped<IOtpService, OtpService>();

// ── APPLICATION SERVICES ───────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

// ── JWT AUTHENTICATION ─────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ── AUTHORIZATION POLICIES ─────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});

// ── FASTENDPOINTS ──────────────────────────────────────────────────
builder.Services.AddFastEndpoints();

// ── CONTROLLERS ────────────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// ── SWAGGER ────────────────────────────────────────────────────────
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Apartment Management System API";
        s.Version = "v1";
        s.Description = "Complete API Documentation - Controllers and FastEndpoints";
    };

    o.EndpointFilter = ep => true;
    o.EnableJWTBearerAuth = true;
    o.ShortSchemaNames = true;
    o.RemoveEmptyRequestSchema = true;
    o.TagCase = TagCase.TitleCase;
    o.AutoTagPathSegmentIndex = 1;
});

// ── CORS ───────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["WebAppUrl"] ?? "http://localhost:5002")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ── HTTP CONTEXT ───────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();

// ── BUILD APP ──────────────────────────────────────────────────────
var app = builder.Build();

// ── MIDDLEWARE PIPELINE ────────────────────────────────────────────
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowWebApp");
app.UseAuthentication();
app.UseAuthorization();

// ── SWAGGER UI (dev only) ──────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

// ── ENDPOINTS ─────────────────────────────────────────────────────
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});

app.MapControllers();

// ── AUTO MIGRATIONS ────────────────────────────────────────────────
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

// Makes Program accessible for integration tests
public partial class Program { }


*/












/*using FastEndpoints;
using FastEndpoints.Swagger;
using ApartmentManagementSystem.API.Extensions;
using ApartmentManagementSystem.API.Filters;
using ApartmentManagementSystem.API.Middlewares;
using ApartmentManagementSystem.API.Policies;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Infrastructure.Email;
using ApartmentManagementSystem.Infrastructure.OTP;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Repositories;
using ApartmentManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPOSITORIES
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IFlatRepository, FlatRepository>();
builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IEnhancedDashboardRepository, EnhancedDashboardRepository>();
builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
builder.Services.AddScoped<IResidentManagementRepository, ResidentManagementRepository>();

// SERVICES
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

// JWT AUTHENTICATION
builder.Services.AddJwtAuthentication(builder.Configuration);

// AUTHORIZATION POLICIES
builder.Services.AddAuthorization(options =>
{
AuthorizationPolicies.AddPolicies(options);
});

builder.Services.AddFastEndpoints();

// API Versioning for Controllers
// Controllers
builder.Services.AddControllers(options =>
{
options.Filters.Add<ValidationFilter>();
});

// SWAGGER MUST BE CONFIGURED FOR BOTH CONTROLLERS AND FASTENDPOINTS
//builder.Services.AddEndpointsApiExplorer();

// FastEndpoints Swagger Document - This generates docs for FastEndpoints
builder.Services.SwaggerDocument(o =>
{
o.DocumentSettings = s =>
{
s.DocumentName = "v1";
s.Title = "Apartment Management System API";
s.Version = "v1";
s.Description = "Complete API Documentation - Controllers and FastEndpoints";
};

//This tells Swagger to include BOTH FastEndpoints AND Controllers
o.EndpointFilter = ep => true; // Include all endpoints

// Enable JWT in Swagger
o.EnableJWTBearerAuth = true;

// Clean display
o.ShortSchemaNames = true;
o.RemoveEmptyRequestSchema = true;

// Tag configuration
o.TagCase = TagCase.TitleCase;

//  Set this to help with auto-tagging
o.AutoTagPathSegmentIndex = 1; // Use second segment for tag (after "api")
});

// CORS
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowWebApp", policy =>
{
policy
    .WithOrigins(builder.Configuration["WebAppUrl"] ?? "http://localhost:5002")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
});
});

// HTTP CONTEXT
builder.Services.AddHttpContextAccessor();

// BUILD APP
var app = builder.Build();

// MIDDLEWARE PIPELINE
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// AUTHENTICATION & AUTHORIZATION - BEFORE ENDPOINTS
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();      // FastEndpoints OpenAPI
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});

// CONTROLLERS (for backward compatibility)
app.MapControllers();

// AUTO MIGRATIONS (DEV ONLY)
if (app.Environment.IsDevelopment())
{
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();
}
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

// At the end of Program.cs to make it accessible for integration tests
public partial class Program { }

*/



































