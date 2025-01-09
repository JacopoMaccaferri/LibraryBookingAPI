public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();  // Builds and runs the host
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();  // Specifies the Startup class for configuring services and the app's request pipeline
            });
}

