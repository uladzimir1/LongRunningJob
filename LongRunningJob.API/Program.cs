using LongRunningJob.API.Hubs;
using LongRunningJob.API.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Logging.AddConsole();
builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);
builder.Services.AddLogging();

builder.Services.AddSingleton<IJobService, JobService>();
builder.Services.AddSingleton<IDelayService, DelayService>();
builder.Services.AddSignalR();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
    options => options.AddPolicy(
        "CorsPolicy",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapGet("/ping", () => "pong");

app.MapHub<ProcessingHub>("/processing");

app.Run();

