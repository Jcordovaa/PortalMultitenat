global using ApiPortal.Security.UserService;
using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.Security.Extensions;
using ApiPortal.Security.TenantService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//JCA CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("MyAllowedOrigins",
//        builder =>
//        {
//            builder.AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader();
//        });
//});

//builder.Services.AddCors();


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMemoryCache, MemoryCache>();

builder.Services.AddHttpContextAccessor();



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Portal Clientes", Version = "V1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Ingrese JSON Web Token solicitado por autenticación",        
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"         
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

//BASE DE DATOS

builder.Services.AddDbContext<PortalAdministracionSoftlandContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("TenantAdmin")));
builder.Services.AddDbContext<PortalClientesSoftlandContext>();
builder.Services.AddTransient<ITenantAccessor<ApiPortal.Security.Tenant>, TenantAccessor>();

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddMultiTenancy()
    .WithResolutionStrategy<HostResolutionStrategy>()
    .WithStore<DbContextTenantStore>();




var app = builder.Build();

//app.UseCors("MyAllowedOrigins");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();


//JCA CORS
//app.UseRouting();
//app.UseCors(builder =>
//{
//    builder.WithOrigins("https://tenat1.intgra.cl")
//           .AllowAnyMethod()
//           .AllowAnyHeader();
//});

app.UseMultiTenancy();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//    );

AppDomain.CurrentDomain.SetData("ContentRootPath", app.Environment.ContentRootPath);
AppDomain.CurrentDomain.SetData("WebRootPath", app.Environment.WebRootPath);

app.Run();
