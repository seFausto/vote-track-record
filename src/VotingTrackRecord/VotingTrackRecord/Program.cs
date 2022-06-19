using VotingTrackRecordClasses;
using System.Configuration;
using VotingTrackRecord.Common.Settings;
using VoteTracker;
using Serilog;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using TwitterService;
using Repository;
using VotingTrackRecord.Middleware;

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

            var applicationSettings = builder.Configuration.GetSection("ApplicationSettings");
            builder.Services.Configure<ApplicationSettings>(applicationSettings);

            var propublicaSettings = builder.Configuration.GetSection("PropublicaSettings");
            builder.Services.Configure<PropublicaSettings>(propublicaSettings);

            var databaseSettings = builder.Configuration.GetSection("DatabaseSettings");
            builder.Services.Configure<DatabaseSettings>(databaseSettings);

            var twitterSettings = builder.Configuration.GetSection("TwitterSettings");
            builder.Services.Configure<TwitterSettings>(twitterSettings);

            builder.Services.AddSingleton<IWordListRepository, WordListRepository>();

            builder.Services.AddSingleton<IPropublicaRepository, PropublicaRepository>();
            builder.Services.AddSingleton<IPropublicaApiService, PropublicaApiService>();

            builder.Services.AddSingleton<IPropublicaBusiness, PropublicaBusiness>();
            builder.Services.AddSingleton<ITwitterBusiness, TwitterBusiness>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Http(builder.Configuration["ApplicationSettings:LoggingHttpEndpoint"].ToString(), 1000)
                .CreateLogger();
            
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
    }
}