using AirHockey.Analytics;
using AirHockey.Facades;
using AirHockey.Services;
using AirHockey.Strategies;

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
            builder.Services.AddSingleton<Facade>();
            builder.Services.AddSingleton<GameService>();
            builder.Services.AddSingleton<ICollision, BaseCollision>();
            builder.Services.AddSingleton<ICollision, WallCollision>();
            builder.Services.AddSingleton<ICollision, QuickCollision>();
            builder.Services.AddSingleton<ICollision, TeleportCollision>();
            builder.Services.AddSingleton<ICollision, BouncyCollision>();
            builder.Services.AddSingleton<ICollision, ScrolingCollision>();
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
