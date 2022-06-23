using VotingTrackRecordClasses;
using VotingTrackRecord.Common.Settings;
using VoteTracker;
using Serilog;
using TwitterService;
using Repository;

namespace VotingTrackRecord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            SetSettings(builder);

            builder.Services.AddSingleton<IWordListRepository, WordListRepository>();

            builder.Services.AddSingleton<IPropublicaRepository, PropublicaRepository>();
            builder.Services.AddSingleton<IPropublicaApiService, PropublicaApiService>();

            builder.Services.AddSingleton<IPropublicaBusiness, PropublicaBusiness>();
            builder.Services.AddSingleton<ITwitterBusiness, TwitterBusiness>();

            CreateLogger(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseMiddleware<ApiKeyMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void CreateLogger(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.Http(builder.Configuration["ApplicationSettings:LoggingHttpEndpoint"].ToString(),
                                queueLimitBytes: null)
                            .CreateLogger();
        }

        private static void SetSettings(WebApplicationBuilder builder)
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