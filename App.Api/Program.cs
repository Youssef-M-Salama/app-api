using App.Api.StartupExtensions;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning.ApiExplorer;
var builder = WebApplication.CreateBuilder(args);

//Enable versioning in Web API controllers
// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));// specify that all action methods in the project will return json response
    options.Filters.Add(new ConsumesAttribute("application/json"));// specify that all action methods in the project will accept json request body
});


builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
})
.AddApiExplorer(options =>          
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;  
});

builder.Services.AddEndpointsApiExplorer();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();  

builder.Services.ConfigureServices(builder.Configuration);
//Swagger
builder.Services.AddEndpointsApiExplorer(); // generates description of all endpoints in the project 
builder.Services.AddSwaggerGen(
    options =>
    {
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
        options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo()
        {
            Version = "1.0",
            Title = "App.Api",
        });
       
    });
//generate open api specification document in json format for all endpoints in the project
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();//
}

app.UseHttpsRedirection();//redirect http request to https
app.UseSwagger();   //create endpoint for swagger json  
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
}); //create swagger ui for testing all web api endpoints /action methods
app.UseAuthorization();   //add authorization middleware to the pipeline
app.MapControllers();//map all controller action methods to the routing system

app.Run();