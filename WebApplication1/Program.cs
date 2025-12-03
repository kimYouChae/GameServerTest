using WebApplication1.Middleware;
using WebApplication1.Repository;
using ZLogger;

//

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

// Add services to the container.
// DI 구조 사용 부분 
// <인터페이스, 구현 객체>
builder.Services.AddTransient<IGameDB, GameDB>();
builder.Services.AddSingleton<IMemoryDB, MemoryDB>();

builder.Services.AddControllers();

SettingLogger();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
// app.UseMiddleware<CheckUserAuthAndLoadUserData>();

app.UseAuthorization();

app.MapControllers();

app.Run();

void SettingLogger()
{
    ILoggingBuilder logging = builder.Logging;
    _ = logging.ClearProviders();

    string? fileDir = configuration["logdir"];
    if (fileDir == null)
    {
        throw new Exception("logdir is not set int appsettings.json");
    }

    bool exits = Directory.Exists(fileDir);
    if (!exits)
    {
        _ = Directory.CreateDirectory(fileDir);
    }

    _ = logging.AddZLoggerRollingFile(
        options =>
        {
            options.UseJsonFormatter();
            options.FilePathSelector = (timestamp, sequenceNumber) => $"{fileDir}{timestamp.ToLocalTime():yyyy-MM-dd}_{sequenceNumber:000}.log";
            options.RollingInterval = ZLogger.Providers.RollingInterval.Day;
            options.RollingSizeKB = 1024;
        });

    _ = logging.AddZLoggerConsole(options =>
    {
        options.UseJsonFormatter();
    });
}