using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;  // Constructor that initializes the configuration
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Configures the database context to use PostgreSQL
        services.AddDbContext<LibraryContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        // Adds controllers
        services.AddControllers();

        // Configures logging
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
        });

        // Configures Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Library Booking API",
                Version = "v1"
            });
            // Gets the XML file path
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); 
            c.IncludeXmlComments(xmlPath);  // Includes XML comments for Swagger
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();  // Enables the developer exception page in development mode
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Booking API v1");
                c.RoutePrefix = string.Empty;  // Sets Swagger UI as the default page
            });
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");  // Uses the exception handler in non-development mode
            app.UseHsts();  // Uses HTTP Strict Transport Security (HSTS)
        }

        app.UseHttpsRedirection();  // Redirects HTTP requests to HTTPS
        app.UseStaticFiles();  // Serves static files

        app.UseRouting();  // Adds routing

        app.UseAuthorization();  // Adds authorization

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();  // Maps controller endpoints
        });

        // Middleware for logging HTTP requests
        app.Use(async (context, next) =>
        {
            logger.LogInformation("Handling request: {Path}", context.Request.Path);
            await next.Invoke();
            logger.LogInformation("Finished handling request.");
        });

        // Middleware for logging exceptions
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;
                logger.LogError(exception, "An error occurred while handling the request");
                await context.Response.WriteAsync("An error occurred");
            });
        });
    }
}

