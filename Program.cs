using MCPSharp;
using MCPv1.Models;
using MCPv1.Models.Entities;
using MCPv1.Models.repositories;
using MCPv1.Models.Services;
using MCPv1.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System;
using System.IO;
using System.Threading;

namespace MCPv1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Contains("--mcp-server"))
            {
                await RunMcpServerAsync();
                return;
            }
            await RunWebAppAsync(args);
        }

        private static async Task RunMcpServerAsync()
        {
            var originalOut = Console.Out;
            Console.SetOut(System.IO.TextWriter.Null);

            try
            {
                string connectionString;

                try
                {
                   
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true)
                        .Build();

                    connectionString = configuration.GetConnectionString("DefaultConnection");
                }
                catch
                {
                    connectionString = null;
                }
         
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = "Server=.;Database=MCPV1;Trusted_Connection=True;TrustServerCertificate=True";
                }

                var services = new ServiceCollection();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString)
                    .EnableSensitiveDataLogging(false)
                    .LogTo(_ => { }, Microsoft.Extensions.Logging.LogLevel.None));

                services.AddScoped<IStudentRepository, StudentRepository>();
                services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                services.AddScoped<IStudentService, StudentService>();
                services.AddScoped<IDepartmentService, DepartmentService>();

                var serviceProvider = services.BuildServiceProvider();
                StudentTools.ServiceProvider = serviceProvider;

                Console.SetOut(originalOut);
                MCPServer.Register<StudentTools>();

                var serverTask = MCPServer.StartAsync("saadanmcp.server", "1.0.0");
                var cts = new CancellationTokenSource();

                await Task.WhenAll(
                    serverTask,
                    Task.Delay(Timeout.Infinite, cts.Token)
                );
            }
            catch (Exception ex)
            {
                Console.SetOut(originalOut);
                Console.Error.WriteLine($"MCP Server Error: {ex.Message}");
                Console.Error.WriteLine($"Stack: {ex.StackTrace}");
                await Task.Delay(Timeout.Infinite);
            }
        }
        

        private static async Task RunWebAppAsync(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews()
    .AddOData(opt => opt
        .AddRouteComponents("odata", GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddHttpClient<OllamaService>();
            builder.Services.AddSingleton<MCPClient>(new MCPClient(
    "saadanyclient",
    "v1.0.0",
    @"MCPv1.exe",
    "--mcp-server"
));
            builder.Services.AddHttpClient();

            var app = builder.Build();

            StudentTools.ServiceProvider = app.Services;

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<StudentDTO>("Students");
            builder.EntitySet<DepartmentDTO>("Departments");
            return builder.GetEdmModel();
        }
    }
}