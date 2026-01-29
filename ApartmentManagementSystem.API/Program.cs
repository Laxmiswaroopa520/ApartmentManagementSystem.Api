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

// ========================================
// CONTROLLERS (Keep for backward compatibility)
// ========================================
builder.Services.AddControllers();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPOSITORIES - Phase 1
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();

// REPOSITORIES - Phase 2
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

// SERVICES - Phase 1
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// SERVICES - Phase 2
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

// JWT AUTHENTICATION
builder.Services.AddJwtAuthentication(builder.Configuration);

// ========================================
// FASTENDPOINTS + SWAGGER (PROPERLY CONFIGURED!)
// ========================================

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// Add endpoint explorer for Controllers (needed for Swagger)
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to show BOTH Controllers AND FastEndpoints
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

    // Use short schema names for cleaner display
    o.ShortSchemaNames = true;

    // Organize endpoints by tags
    o.AutoTagPathSegmentIndex = 0;

    // Remove servers section for cleaner UI
    o.RemoveEmptyRequestSchema = true;
});

// AUTHORIZATION POLICIES
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
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

// ========================================
// BUILD APP
// ========================================
var app = builder.Build();

// ========================================
// MIDDLEWARE PIPELINE
// ========================================

// Custom middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// ========================================
// SWAGGER - PROPERLY CONFIGURED FOR BOTH!
// ========================================
if (app.Environment.IsDevelopment())
{
    // Use FastEndpoints' OpenApi middleware
    app.UseOpenApi();  // Generates /swagger/v1/swagger.json

    // Use Swagger UI with proper configuration
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

// ========================================
// AUTHENTICATION & AUTHORIZATION
// CRITICAL: Must be before FastEndpoints!
// ========================================
app.UseAuthentication();
app.UseAuthorization();

// ========================================
// FASTENDPOINTS - CONFIGURED WITH "API" PREFIX
// ========================================
app.UseFastEndpoints(c =>
{
    // Add "api" prefix to all FastEndpoint routes
    // Your endpoints define routes like "/v1/fast/..." 
    // This will make them accessible at "api/v1/fast/..."
   // c.Endpoints.RoutePrefix = "api";

    // Configure versioning (optional, not actively used since you define versions in routes)
    c.Versioning.Prefix = "v";
    c.Versioning.PrependToRoute = false;
});

// ========================================
// CONTROLLERS (For backward compatibility)
// ========================================
app.MapControllers();

// AUTO MIGRATIONS (DEV ONLY)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();












/*
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

// ========================================
// CONTROLLERS (Keep for backward compatibility)
// ========================================
builder.Services.AddControllers();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPOSITORIES - Phase 1
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();

// REPOSITORIES - Phase 2
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

// SERVICES - Phase 1
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// SERVICES - Phase 2
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

// JWT AUTHENTICATION
builder.Services.AddJwtAuthentication(builder.Configuration);

// ========================================
// FASTENDPOINTS + SWAGGER (CRITICAL SECTION!)
// ========================================

builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Apartment Management System API";
        s.Version = "v1";
    };

    o.EnableJWTBearerAuth = true;
});


/*
builder.Services.AddFastEndpoints();

// IMPORTANT: Use FastEndpoints' Swagger ONLY
// Remove any calls to AddSwaggerGen() or AddSwaggerDocumentation()
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "Apartment Management System API";
        s.Version = "v1";
        s.Description = "Complete API - Both Controllers and FastEndpoints";
    };

    // Enable JWT Authentication in Swagger
    o.EnableJWTBearerAuth = true;

    // This helps organize endpoints by tags
    o.AutoTagPathSegmentIndex = 0;

    // Include XML comments if you have them
    // o.IncludeXmlComments = true;
});
------
// AUTHORIZATION POLICIES
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
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

// ========================================
// BUILD APP
// ========================================
var app = builder.Build();

// ========================================
// MIDDLEWARE PIPELINE
// ========================================

// Custom middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// ========================================
// SWAGGER - USE FASTENDPOINTS' SWAGGER ONLY!
// ========================================
if (app.Environment.IsDevelopment())
{
    // DO NOT USE: app.UseSwaggerDocumentation()
    // DO NOT USE: app.UseSwaggerGen()

    // Use FastEndpoints' OpenApi
    app.UseOpenApi();           //generates /swagger/v1/swagger.json
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}

// ========================================
// AUTHENTICATION & AUTHORIZATION
// CRITICAL: Must be before FastEndpoints!
// ========================================
app.UseAuthentication();
app.UseAuthorization();

// ========================================
// FASTENDPOINTS
// ========================================
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
    c.Versioning.PrependToRoute = false;
});

// ========================================
// CONTROLLERS (For backward compatibility)
// ========================================
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











/*
using FastEndpoints;
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
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
//Add services to the container
builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// REPOSITORIES
// Phase 1
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();

// Phase 2
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<IFlatRepository, FlatRepository>();
builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();
builder.Services.AddScoped<IFlatRepository, FlatRepository>();
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IEnhancedDashboardRepository, EnhancedDashboardRepository>();
// Register AdminResidentService
builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
//builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();

// SERVICES
// Phase 1
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();






//Register new services in your API project
builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
// Phase 2
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
builder.Services.AddScoped<IResidentManagementRepository, ResidentManagementRepository>();
builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
//Newly added for apartment management service;;
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();
// JWT AUTHENTICATION
// Moved to extension (internally same logic as your previous code)
builder.Services.AddJwtAuthentication(builder.Configuration);

//Add Fast End Points
builder.Services.AddFastEndpoints();            // Add this
builder.Services.SwaggerDocument(o =>           // Add this
{
    o.DocumentSettings = s =>
    {
        s.Title = "Apartment Management API";
        s.Version = "v1";
    };
    o.EnableJWTBearerAuth = true;
});






// AUTHORIZATION (POLICIES – PHASE 2)
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
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

// SWAGGER
//builder.Services.AddSwaggerDocumentation();
//add api version configuration file
builder.Services.AddApiVersioningConfiguration();
// HTTP CONTEXT
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// MIDDLEWARE PIPELINE
if (app.Environment.IsDevelopment())
{
    // app.UseSwaggerDocumentation();
    app.UseSwaggerGen();


}

// Custom middlewares (Phase 1 – preserved)
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowWebApp");

app.UseAuthentication();
app.UseAuthorization();

//adding fast end points here.,
app.UseFastEndpoints(c =>       // Add this
{
    c.Endpoints.RoutePrefix = "api"; // Optional: sets /api prefix
});

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





















