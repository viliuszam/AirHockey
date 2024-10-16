using AirHockey.Analytics;
using AirHockey.Services;

namespace AirHockey
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
            //services.AddSingleton<IGameAnalytics, ConsoleLoggerAdapter>();
            builder.Services.AddSingleton<IGameAnalytics>(sp => new FileLoggerAdapter("./"));
            builder.Services.AddSingleton<GameService>();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<GameHub>("/gameHub");

            app.Run();
        }
    }
}
