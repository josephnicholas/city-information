using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => {
        // if Accept is not supported(formatters) then we return
        // a message saying it's not. (406 Not Acceptable)
        // improves reliability
        // Default formatters should be the first one in the list
        options.ReturnHttpNotAcceptable = true;
    }).AddNewtonsoftJson() // replaces input/output formatters
    .AddXmlDataContractSerializerFormatters(); // added support for XML

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

var app = builder.Build();

// Configure the HTTP request pipeline.
// Order of Middleware Request pipeline is important
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers(); // new approach

app.Run();
