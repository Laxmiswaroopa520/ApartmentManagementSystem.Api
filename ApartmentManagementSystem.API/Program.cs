//using FastEndpoints;
//using FastEndpoints.Swagger;
//using ApartmentManagementSystem.API.Configuration;
//using ApartmentManagementSystem.API.Extensions;
//using ApartmentManagementSystem.API.Filters;
//using ApartmentManagementSystem.API.Middlewares;
//using ApartmentManagementSystem.API.Policies;
//using ApartmentManagementSystem.Application.Interfaces.Repositories;
//using ApartmentManagementSystem.Application.Interfaces.Services;
//using ApartmentManagementSystem.Application.Services;
//using ApartmentManagementSystem.Infrastructure.Email;
//using ApartmentManagementSystem.Infrastructure.OTP;
//using ApartmentManagementSystem.Infrastructure.Persistence;
//using ApartmentManagementSystem.Infrastructure.Repositories;
//using ApartmentManagementSystem.Infrastructure.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;

//var builder = WebApplication.CreateBuilder(args);

//// ========================================
//// DATABASE
//// ========================================
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// ========================================
//// REPOSITORIES - Phase 1
//// ========================================
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
//builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();

//// ========================================
//// REPOSITORIES - Phase 2
//// ========================================
//builder.Services.AddScoped<IManagerService, ManagerService>();
//builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
//builder.Services.AddScoped<IFlatRepository, FlatRepository>();
//builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();
//builder.Services.AddScoped<IFloorRepository, FloorRepository>();
//builder.Services.AddScoped<IEnhancedDashboardRepository, EnhancedDashboardRepository>();
//builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
//builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
//builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
//builder.Services.AddScoped<IResidentManagementRepository, ResidentManagementRepository>();

//// ========================================
//// SERVICES - Phase 1
//// ========================================
//builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IOnboardingService, OnboardingService>();
//builder.Services.AddScoped<IOtpService, OtpService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<ISmsService, SmsService>();
//builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

//// ========================================
//// SERVICES - Phase 2
//// ========================================
//builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
//builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
//builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
//builder.Services.AddScoped<IDashboardService, DashboardService>();
//builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

//// ========================================
//// JWT AUTHENTICATION
//// ========================================
//builder.Services.AddJwtAuthentication(builder.Configuration);

//// ========================================
//// FASTENDPOINTS (MUST BE BEFORE SWAGGER)
//// ========================================
//builder.Services.AddFastEndpoints();
//// Required for Controller endpoints in Swagger
////builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerDocument();


//// ========================================
//// SWAGGER - USING FASTENDPOINTS SWAGGER ONLY!
//// DO NOT USE AddSwaggerGen() - IT CONFLICTS!
//// ========================================
////builder.Services.AddSwaggerDocument(
////{
////    /*  o.DocumentSettings = s =>
////      {
////          s.DocumentName = "v1";
////          s.Title = "Apartment Management System API";
////          s.Version = "v1";
////          s.Description = "Complete API Documentation - Controllers and FastEndpoints";
////      };
////      // Enable JWT Bearer authentication in Swagger UI
////      o.EnableJWTBearerAuth = true;
////      // Use short schema names for cleaner Swagger UI
////      o.ShortSchemaNames = true;
////      // Tag endpoints by the 3rd path segment (e.g., "onboarding" from /api/v1/fast/onboarding/...)
////      // Index: 0=api, 1=v1, 2=fast, 3=onboarding
////      o.AutoTagPathSegmentIndex = 3;
////      // Remove empty request schemas
////      o.RemoveEmptyRequestSchema = true;
////    */


////});

//// ========================================
//// AUTHORIZATION POLICIES
//// ========================================
//builder.Services.AddAuthorization(options =>
//{
//    AuthorizationPolicies.AddPolicies(options);
//});

//// ========================================
//// CORS
//// ========================================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowWebApp", policy =>
//    {
//        policy
//            .WithOrigins(builder.Configuration["WebAppUrl"] ?? "http://localhost:5002")
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .AllowCredentials();
//    });
//});

//// CONTROLLERS + FILTERS
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<ValidationFilter>();
//});



//// ========================================
//// API VERSIONING
//// ========================================
//builder.Services.AddApiVersioningConfiguration();

//// ========================================
//// HTTP CONTEXT
//// ========================================
//builder.Services.AddHttpContextAccessor();

//// ========================================
//// BUILD APP
//// ========================================
//var app = builder.Build();

//// ========================================
//// MIDDLEWARE PIPELINE
//// ========================================

//// Custom middlewares first
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

//app.UseHttpsRedirection();
//app.UseCors("AllowWebApp");

//// ========================================
//// SWAGGER - PROPERLY CONFIGURED
//// ========================================
//if (app.Environment.IsDevelopment())
//{
//    // FastEndpoints OpenAPI - generates /swagger/v1/swagger.json
//    //app.UseOpenApi();
//    //// Swagger UI
//    //app.UseSwaggerUi(c =>
//    //{
//    //    c.ConfigureDefaults();
//    //    c.Path = "/swagger";
//    //});
//}


//// ========================================
//// AUTHENTICATION & AUTHORIZATION
//// CRITICAL: Must be BEFORE FastEndpoints!
//// ========================================
//app.UseAuthentication();
//app.UseAuthorization();

//app.UseFastEndpoints(c=>
//{
//    c.Endpoints.RoutePrefix = "api";
//});
//app.UseOpenApi();
//app.UseSwaggerUi(c =>
//{
//    c.ConfigureDefaults();
//    c.Path = "/swagger";    
//});

//// ========================================
//// FASTENDPOINTS
//// ========================================
////app.UseFastEndpoints(c =>
////{
////    // Add "api" prefix to all FastEndpoint routes
////    // Your endpoints define: "/v1/fast/onboarding/roles"
////    // Final route becomes: "/api/v1/fast/onboarding/roles"
////    c.Endpoints.RoutePrefix = "api";
////    // Optional: Serializer settings
////    c.Serializer.Options.PropertyNamingPolicy = null; // PascalCase
////});

//// ========================================
//// CONTROLLERS (For backward compatibility)
//// ========================================
//app.MapControllers();

//// ========================================
//// AUTO MIGRATIONS (DEV ONLY)
//// ========================================
//if (app.Environment.IsDevelopment())
//{

//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}

//app.Run();



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
*/
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



























///*
//using FastEndpoints;
//using FastEndpoints.Swagger;
//using ApartmentManagementSystem.API.Configuration;
//using ApartmentManagementSystem.API.Extensions;
//using ApartmentManagementSystem.API.Filters;
//using ApartmentManagementSystem.API.Middlewares;
//using ApartmentManagementSystem.API.Policies;
//using ApartmentManagementSystem.Application.Interfaces.Repositories;
//using ApartmentManagementSystem.Application.Interfaces.Services;
//using ApartmentManagementSystem.Application.Services;
//using ApartmentManagementSystem.Infrastructure.Email;
//using ApartmentManagementSystem.Infrastructure.OTP;
//using ApartmentManagementSystem.Infrastructure.Persistence;
//using ApartmentManagementSystem.Infrastructure.Repositories;
//using ApartmentManagementSystem.Infrastructure.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;

//var builder = WebApplication.CreateBuilder(args);

//// ========================================
//// CONTROLLERS (Keep for backward compatibility)
//// ========================================
//builder.Services.AddControllers();

//// DATABASE
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// REPOSITORIES - Phase 1
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
//builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();

//// REPOSITORIES - Phase 2
//builder.Services.AddScoped<IManagerService, ManagerService>();
//builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
//builder.Services.AddScoped<IFlatRepository, FlatRepository>();
//builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();
//builder.Services.AddScoped<IFloorRepository, FloorRepository>();
//builder.Services.AddScoped<IEnhancedDashboardRepository, EnhancedDashboardRepository>();
//builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
//builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
//builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
//builder.Services.AddScoped<IResidentManagementRepository, ResidentManagementRepository>();

//// SERVICES - Phase 1
//builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IOnboardingService, OnboardingService>();
//builder.Services.AddScoped<IOtpService, OtpService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<ISmsService, SmsService>();
//builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

//// SERVICES - Phase 2
//builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
//builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
//builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
//builder.Services.AddScoped<IDashboardService, DashboardService>();
//builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();

//// JWT AUTHENTICATION
//builder.Services.AddJwtAuthentication(builder.Configuration);

//// ========================================
//// FASTENDPOINTS + SWAGGER (CRITICAL SECTION!)
//// ========================================

//builder.Services.AddFastEndpoints();

//builder.Services.SwaggerDocument(o =>
//{
//    o.DocumentSettings = s =>
//    {
//        s.DocumentName = "v1";
//        s.Title = "Apartment Management System API";
//        s.Version = "v1";
//    };

//    o.EnableJWTBearerAuth = true;
//});


///*
//builder.Services.AddFastEndpoints();

//// IMPORTANT: Use FastEndpoints' Swagger ONLY
//// Remove any calls to AddSwaggerGen() or AddSwaggerDocumentation()
//builder.Services.SwaggerDocument(o =>
//{
//    o.DocumentSettings = s =>
//    {
//        s.DocumentName = "v1";
//        s.Title = "Apartment Management System API";
//        s.Version = "v1";
//        s.Description = "Complete API - Both Controllers and FastEndpoints";
//    };

//    // Enable JWT Authentication in Swagger
//    o.EnableJWTBearerAuth = true;

//    // This helps organize endpoints by tags
//    o.AutoTagPathSegmentIndex = 0;

//    // Include XML comments if you have them
//    // o.IncludeXmlComments = true;
//});
//------
//// AUTHORIZATION POLICIES
//builder.Services.AddAuthorization(options =>
//{
//    AuthorizationPolicies.AddPolicies(options);
//});

//// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowWebApp", policy =>
//    {
//        policy
//            .WithOrigins(builder.Configuration["WebAppUrl"] ?? "http://localhost:5002")
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .AllowCredentials();
//    });
//});

//// CONTROLLERS + FILTERS
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<ValidationFilter>();
//});

//// API VERSIONING
//builder.Services.AddApiVersioningConfiguration();

//// HTTP CONTEXT
//builder.Services.AddHttpContextAccessor();

//// ========================================
//// BUILD APP
//// ========================================
//var app = builder.Build();

//// ========================================
//// MIDDLEWARE PIPELINE
//// ========================================

//// Custom middlewares
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

//app.UseHttpsRedirection();
//app.UseCors("AllowWebApp");

//// ========================================
//// SWAGGER - USE FASTENDPOINTS' SWAGGER ONLY!
//// ========================================
//if (app.Environment.IsDevelopment())
//{
//    // DO NOT USE: app.UseSwaggerDocumentation()
//    // DO NOT USE: app.UseSwaggerGen()

//    // Use FastEndpoints' OpenApi
//    app.UseOpenApi();           //generates /swagger/v1/swagger.json
//    app.UseSwaggerUi(c =>
//    {
//        c.ConfigureDefaults();
//        c.Path = "/swagger";
//    });
//}

//// ========================================
//// AUTHENTICATION & AUTHORIZATION
//// CRITICAL: Must be before FastEndpoints!
//// ========================================
//app.UseAuthentication();
//app.UseAuthorization();

//// ========================================
//// FASTENDPOINTS
//// ========================================
//app.UseFastEndpoints(c =>
//{
//    c.Endpoints.RoutePrefix = "api";
//    c.Versioning.Prefix = "v";
//    c.Versioning.PrependToRoute = false;
//});

//// ========================================
//// CONTROLLERS (For backward compatibility)
//// ========================================
//app.MapControllers();

//// AUTO MIGRATIONS (DEV ONLY)
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}

//app.Run();

//*/











///*
//using FastEndpoints;
//using ApartmentManagementSystem.API.Configuration;
//using ApartmentManagementSystem.API.Extensions;
//using ApartmentManagementSystem.API.Filters;
//using ApartmentManagementSystem.API.Middlewares;
//using ApartmentManagementSystem.API.Policies;
//using ApartmentManagementSystem.Application.Interfaces.Repositories;
//using ApartmentManagementSystem.Application.Interfaces.Services;
//using ApartmentManagementSystem.Application.Services;
//using ApartmentManagementSystem.Infrastructure.Email;
//using ApartmentManagementSystem.Infrastructure.OTP;
//using ApartmentManagementSystem.Infrastructure.Persistence;
//using ApartmentManagementSystem.Infrastructure.Repositories;
//using ApartmentManagementSystem.Infrastructure.Services;
//using FastEndpoints.Swagger;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;

//var builder = WebApplication.CreateBuilder(args);
////Add services to the container
//builder.Services.AddControllers();
////builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddSwaggerGen();
//// DATABASE
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// REPOSITORIES
//// Phase 1
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IUserOtpRepository, UserOtpRepository>();
//builder.Services.AddScoped<IUserInviteRepository, UserInviteRepository>();

//// Phase 2
//builder.Services.AddScoped<IManagerService, ManagerService>();
//builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
//builder.Services.AddScoped<IFlatRepository, FlatRepository>();
//builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();
//builder.Services.AddScoped<IFlatRepository, FlatRepository>();
//builder.Services.AddScoped<IFloorRepository, FloorRepository>();
//builder.Services.AddScoped<IEnhancedDashboardRepository, EnhancedDashboardRepository>();
//// Register AdminResidentService
//builder.Services.AddScoped<IAdminResidentService, AdminResidentService>();
////builder.Services.AddScoped<IUserFlatMappingRepository, UserFlatMappingRepository>();

//// SERVICES
//// Phase 1
//builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IOnboardingService, OnboardingService>();
//builder.Services.AddScoped<IOtpService, OtpService>();
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<ISmsService, SmsService>();
//builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();






////Register new services in your API project
//builder.Services.AddScoped<ICommunityMemberService, CommunityMemberService>();
//builder.Services.AddScoped<IStaffMemberService, StaffMemberService>();
//builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
//builder.Services.AddScoped<IEnhancedDashboardService, EnhancedDashboardService>();
//// Phase 2
//builder.Services.AddScoped<IDashboardService, DashboardService>();
//builder.Services.AddScoped<ICommunityMemberRepository, CommunityMemberRepository>();
//builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
//builder.Services.AddScoped<IResidentManagementRepository, ResidentManagementRepository>();
//builder.Services.AddScoped<IResidentManagementService, ResidentManagementService>();
////Newly added for apartment management service;;
//builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();
//// JWT AUTHENTICATION
//// Moved to extension (internally same logic as your previous code)
//builder.Services.AddJwtAuthentication(builder.Configuration);

////Add Fast End Points
//builder.Services.AddFastEndpoints();            // Add this
//builder.Services.SwaggerDocument(o =>           // Add this
//{
//    o.DocumentSettings = s =>
//    {
//        s.Title = "Apartment Management API";
//        s.Version = "v1";
//    };
//    o.EnableJWTBearerAuth = true;
//});






//// AUTHORIZATION (POLICIES – PHASE 2)
//builder.Services.AddAuthorization(options =>
//{
//    AuthorizationPolicies.AddPolicies(options);
//});

//// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowWebApp", policy =>
//    {
//        policy
//            .WithOrigins(builder.Configuration["WebAppUrl"] ?? "http://localhost:5002")
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .AllowCredentials();
//    });
//});

//// CONTROLLERS + FILTERS
//builder.Services.AddControllers(options =>
//{
//    options.Filters.Add<ValidationFilter>();
//});

//// SWAGGER
////builder.Services.AddSwaggerDocumentation();
////add api version configuration file
//builder.Services.AddApiVersioningConfiguration();
//// HTTP CONTEXT
//builder.Services.AddHttpContextAccessor();

//var app = builder.Build();

//// MIDDLEWARE PIPELINEa
//if (app.Environment.IsDevelopment())
//{
//    // app.UseSwaggerDocumentation();
//    app.UseSwaggerGen();


//}

//// Custom middlewares (Phase 1 – preserved)
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

//app.UseHttpsRedirection();

//app.UseCors("AllowWebApp");

//app.UseAuthentication();
//app.UseAuthorization();

////adding fast end points here.,
//app.UseFastEndpoints(c =>       // Add this
//{
//    c.Endpoints.RoutePrefix = "api"; // Optional: sets /api prefix
//});

//app.MapControllers();

//// AUTO MIGRATIONS (DEV ONLY)
//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    dbContext.Database.Migrate();
//}

//app.Run();


//*/
/*
#region
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
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region REPOSITORIES
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
#endregion

#region SERVICES
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
#endregion

#region AUTHENTICATION & AUTHORIZATION
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.AddPolicies(options);
});
#endregion

#region FASTENDPOINTS + SWAGGER (ONLY ONE SWAGGER)
builder.Services.AddFastEndpoints(o =>
{
    o.Assemblies = new[]
    {
        typeof(ApartmentManagementSystem.API.Endpoints.V1.Onboarding.GetRolesEndpoint).Assembly
    };
});
//builder.Services.SwaggerDocument(o =>
//{
//    o.DocumentSettings = s =>
//    {
//        s.Title = "Apartment Management System API";
//        s.Version = "v1";
//    };

//    o.EnableJWTBearerAuth = true;
//    o.ShortSchemaNames = true;

//});

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Apartment Management System API";
        s.Description = "FastEndpoints";
    };
    o.EnableJWTBearerAuth = true;
    o.ShortSchemaNames = true;
    o.ExcludeNonFastEndpoints = false;
});
#endregion

#region CORS
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
#endregion

#region CONTROLLERS
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
#endregion

#region API VERSIONING & CONTEXT
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddHttpContextAccessor();
#endregion

var app = builder.Build();

#region MIDDLEWARE
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");
#endregion

#region AUTH
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region FASTENDPOINTS (REGISTER BEFORE SWAGGER)
app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api/fast";
});
#endregion

#region SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); // /swagger/v1/swagger.json
    app.UseSwaggerUi(c =>
    {
        c.ConfigureDefaults();
        c.Path = "/swagger";
    });
}
#endregion

#region CONTROLLERS
app.MapControllers();
#endregion

#region AUTO MIGRATION (DEV ONLY)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
#endregion

app.Run();

#endregion

*/