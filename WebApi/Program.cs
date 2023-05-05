using Entities;
using FireBaseAuthenticator.Extensions;
using Infastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Quartz;
using UseCase.Jobs;
using UseCase.Service;
using UseCase.Service.Tabs;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        builder.Services.AddControllers();

        builder.Services.AddTransient<IStartupHelper, StartupHelper>();
        builder.Services.AddSingleton<IBrowserManagerService, BrowserManagerService>();
        builder.Services.AddSingleton<GlobalLockResourceService>();

        builder.Services.AddTransient<IDeviceInfoChart, DeviceInfoChart>();
        builder.Services.AddTransient<IPostLogs, PostLogs>();
        builder.Services.AddTransient<ISettingService, SettingService>();
        builder.Services.AddTransient<IKijijiPostingService, KijijiPostingService>();
        builder.Services.AddTransient<ISettingRepository, SettingRepository>();
        builder.Services.AddTransient<ISigninTabService, SigninTabService>();
        builder.Services.AddTransient<IReadAdTabService, ReadAdTabService>();
        builder.Services.AddTransient<IDeleteTabService, DeleteTabService>();
        builder.Services.AddTransient<IPostTabService, PostTabCategoriesService>();
        builder.Services.AddTransient<IRequestRepository, RequestRepository>();
        builder.Services.AddTransient<IPostRepository, PostRepository>();
        builder.Services.AddTransient<IStepLogRepository, StepLogRepository>();
        builder.Services.AddTransient<ISettingRepository, SettingRepository>();
        builder.Services.AddTransient<IDeviceRegistrationRepository, DeviceRegistrationRepository>();
        builder.Services.AddTransient<IJobManagerService, JobManagerService>();

        builder.Services.AddTransient<KijijiActionHelper>();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseDefaultThreadPool(3);
        });

        builder.Services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });


        builder.Services
            .AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Services.AddLogging(opt =>
        {
            opt.AddSimpleConsole(c =>
            {
                c.TimestampFormat = "[HH:mm:ss] ";
                c.SingleLine = false;
            });
        });
        builder.Services.AddFireBaseRegistrationDependencies();

        var app = builder.Build();
        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        Console.WriteLine("Wait 60 seconds so browser started");
        Thread.Sleep(60000);
        
        using (var scope = app.Services.CreateScope())
        {
           await scope.ServiceProvider.GetRequiredService<IStartupHelper>().Initialize();
        }
        await app.RunAsync();
    }
}