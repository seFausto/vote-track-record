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

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            SetSettings(builder);

            builder.Services.AddScoped<IWordListRepository, WordListRepository>();

            builder.Services.AddScoped<IPropublicaRepository, PropublicaRepository>();
            builder.Services.AddScoped<IPropublicaApiService, PropublicaApiService>();

            builder.Services.AddScoped<IPropublicaBusiness, PropublicaBusiness>();
            builder.Services.AddScoped<ITwitterBusiness, TwitterBusiness>();

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
                                queueLimitBytes: 1)
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