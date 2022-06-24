using Repository;
using Serilog;
using TwitterService;
using VoteTracker;
using VotingTrackRecord.Common.Settings;
using VotingTrackRecordClasses;

namespace VotingTrackRecord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            ConfigureSettings(builder);

            ConfigureServices(builder);

            ConfigureLogger(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IWordListRepository, WordListRepository>();

            builder.Services.AddScoped<IPropublicaRepository, PropublicaRepository>();
            builder.Services.AddScoped<IPropublicaApiService, PropublicaApiService>();

            builder.Services.AddScoped<IPropublicaBusiness, PropublicaBusiness>();
            builder.Services.AddScoped<ITwitterBusiness, TwitterBusiness>();
        }

        private static void ConfigureLogger(WebApplicationBuilder builder)
        {
            var httpEndpoint = builder.Configuration["ApplicationSettings:LoggingHttpEndpoint"].ToString();

            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .Enrich.FromLogContext()                            
                            .WriteTo.Http(httpEndpoint, queueLimitBytes: null)
                            .WriteTo.File("logs\\twitterbot.log", rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }

        private static void ConfigureSettings(WebApplicationBuilder builder)
        {
            var applicationSettings = builder.Configuration.GetSection("ApplicationSettings");
            builder.Services.Configure<ApplicationSettings>(applicationSettings);

            var propublicaSettings = builder.Configuration.GetSection("PropublicaSettings");
            builder.Services.Configure<PropublicaSettings>(propublicaSettings);

            var databaseSettings = builder.Configuration.GetSection("DatabaseSettings");
            builder.Services.Configure<DatabaseSettings>(databaseSettings);

            var twitterSettings = builder.Configuration.GetSection("TwitterSettings");
            builder.Services.Configure<TwitterSettings>(twitterSettings);
        }
    }
}