using FastEndpoints;
using FastEndpoints;
using FastEndpoints.Swagger;
//using ApartmentManagementSystem.API.Configuration;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

// FASTENDPOINTS - Add this BEFORE controllers
builder.Services.AddFastEndpoints();

// API Versioning for Controllers
//builder.Services.AddApiVersioningConfiguration();

// Controllers
builder.Services.AddControllers(options =>
{
options.Filters.Add<ValidationFilter>();
});

// CRITICAL FIX: SWAGGER MUST BE CONFIGURED FOR BOTH CONTROLLERS AND FASTENDPOINTS
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

// CRITICAL: This tells Swagger to include BOTH FastEndpoints AND Controllers
o.EndpointFilter = ep => true; // Include all endpoints

// Enable JWT in Swagger
o.EnableJWTBearerAuth = true;

// Clean display
o.ShortSchemaNames = true;
o.RemoveEmptyRequestSchema = true;

// Tag configuration
o.TagCase = TagCase.TitleCase;

// IMPORTANT: Set this to help with auto-tagging
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
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// AUTHENTICATION & AUTHORIZATION - BEFORE ENDPOINTS
app.UseAuthentication();
app.UseAuthorization();

// SWAGGER - BEFORE FASTENDPOINTS

/*
if (app.Environment.IsDevelopment())
{
// Use OpenApi from FastEndpoints (not the default ASP.NET Core one)
app.UseOpenApi();
app.UseSwaggerUi(c =>
{
c.ConfigureDefaults();
c.Path = "/swagger";
});
}
*/
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

// FASTENDPOINTS CONFIGURATION - CRITICAL ORDER
/*app.UseFastEndpoints(c =>
{
c.Endpoints.RoutePrefix = "api";

// Optional: Configure serialization
c.Serializer.Options.PropertyNamingPolicy = null; // Keep PascalCase
});

*/
// CONTROLLERS (for backward compatibility)
app.MapControllers();

// AUTO MIGRATIONS (DEV ONLY)
if (app.Environment.IsDevelopment())
{
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();
}

app.Run();

// At the end of Program.cs to make it accessible for integration tests
public partial class Program { }


/*recent working one 03-02-2026
using FastEndpoints;
using FastEndpoints.Swagger;
using ApartmentManagementSystem.API.Configuration;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

// CRITICAL FIX: PROPER VERSIONING SETUP

// 1. Add FastEndpoints FIRST (with versioning)
builder.Services.AddFastEndpoints();
/* 
builder.Services.AddFastEndpoints(options =>
{
    // IMPORTANT: Disable FastEndpoints' own versioning
    // We'll handle it manually in the routes
    options.Versioning.IsEnabled = false;
});
----------------------
// 2. Add API Versioning for Controllers
builder.Services.AddApiVersioningConfiguration();

// 3. Add Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// 4. Configure Swagger to see BOTH Controllers and FastEndpoints
builder.Services.AddEndpointsApiExplorer();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Apartment Management System API";
        s.Version = "v1";
        s.Description = "Complete API Documentation - Controllers and FastEndpoints";
    };

    // Enable JWT in Swagger
    o.EnableJWTBearerAuth = true;

    // Clean display
    o.ShortSchemaNames = true;
    o.RemoveEmptyRequestSchema = true;
    
    // CRITICAL: Tag all FastEndpoints properly
    o.TagCase = TagCase.TitleCase;
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

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// SWAGGER - BEFORE AUTH
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();

// FASTENDPOINTS CONFIGURATION - CRITICAL FIX

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});
/*
app.UseFastEndpoints(config =>
{
    // Add "api" prefix to all routes
    config.Endpoints.RoutePrefix = "api";
    
    // DISABLE automatic versioning - we'll handle it in endpoint routes
    config.Versioning.IsEnabled = false;
});
-------------------
// CONTROLLERS (for backward compatibility)
app.MapControllers();

// AUTO MIGRATIONS (DEV ONLY)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();



// At the end of Program.cs  to make it accessible for integration tests
public partial class Program { }

*/




















/*    Now Using

 using FastEndpoints;
using FastEndpoints.Swagger;
using ApartmentManagementSystem.API.Configuration;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// CONTROLLERS (Keep for backward compatibility)
builder.Services.AddControllers();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPOSITORIES - Phase 1
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

//// SERVICES - Phase 1
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

//// SERVICES - Phase 2
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

// FASTENDPOINTS + SWAGGER - CORRECTED

// 1. Add FastEndpoints FIRST
builder.Services.AddFastEndpoints();

// 2. Add Controllers endpoint explorer (for Swagger to see Controllers)
builder.Services.AddEndpointsApiExplorer();

// 3. Configure Swagger with BOTH FastEndpoints and Controllers

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Apartment Management System API";
        s.Version = "v1";
        s.Description = "Complete API Documentation - Controllers and FastEndpoints";
    };

    // Enable JWT Authentication in Swagger
    o.EnableJWTBearerAuth = true;

    // CRITICAL: This makes FastEndpoints appear in Swagger
    o.MaxEndpointVersion = 1;
    o.MinEndpointVersion = 1;

    // Use short schema names for cleaner display
    o.ShortSchemaNames = true;

    // Remove empty request schemas
    o.RemoveEmptyRequestSchema = true;

    // Include XML comments if you have them (optional)
    // o.IncludeXmlComments = true;
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

// CONTROLLERS + FILTERS
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// API VERSIONING
builder.Services.AddApiVersioningConfiguration();

// HTTP CONTEXT
builder.Services.AddHttpContextAccessor();

// BUILD APP
var app = builder.Build();

// MIDDLEWARE PIPELINE

// Custom middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// SWAGGER - MUST BE BEFORE AUTH
if (app.Environment.IsDevelopment())
{
    // Generate OpenAPI document
    app.UseOpenApi();

    // Swagger UI
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();

// FASTENDPOINTS - Route Configuration
app.UseFastEndpoints(c =>
{
    // Add "api" prefix to all FastEndpoint routes
    c.Endpoints.RoutePrefix = "api";

    // Versioning configuration
    c.Versioning.Prefix = "v{version}";
    c.Versioning.PrependToRoute = true; // Changed to true

    // This will make routes: api/v1/fast/onboarding/...
});

// CONTROLLERS (For backward compatibility)
app.MapControllers();

// AUTO MIGRATIONS (DEV ONLY)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

*/
























