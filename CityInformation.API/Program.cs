using CityInformation.API;
using CityInformation.API.DBContext;
using CityInformation.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

// Create Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinformation.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// change logging provider(changes)
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(options => {
        // if Accept is not supported(formatters) then we return
        // a message saying it's not. (406 Not Acceptable)
        // improves reliability
        // Default formatters should be the first one in the list
        options.ReturnHttpNotAcceptable = true;
    }).AddNewtonsoftJson() // replaces input/output formatters
    .AddXmlDataContractSerializerFormatters(); // added support for XML

builder.Services.AddProblemDetails(); // needs to be injected when a Exception Middleware is used to show the exception
// builder.Services.AddProblemDetails(options => {
//     // Adds an attribute in the response body of ProblemDetails
//     options.CustomizeProblemDetails = ctx => {
//         ctx.ProblemDetails.Extensions.Add("additionalInfo", "Additional info example");
//         ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//     };
// });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>(); // this is injectable to other parts in code,
                                                                   // gets the extension of file.
// Compiler directive to choose which service to create
#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(dbContextOptions 
    => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>(); // Create once per request
builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    // How are we going to validate the token
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"] ?? throw new ArgumentNullException()))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Order of Middleware Request pipeline is important
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // sequence matters, authenticate first

app.UseAuthorization();

app.MapControllers(); // new approach

app.Run();
