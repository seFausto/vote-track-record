using VotingTrackRecordClasses;
using System.Configuration;
using VotingTrackRecord.Common.Settings;
using DatabaseRepository;
using VoteTracker;
using Serilog;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;

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

            var propublicaSettings = builder.Configuration.GetSection("PropublicaSettings");
            builder.Services.Configure<PropublicaSettings>(propublicaSettings);

            var databaseSettings = builder.Configuration.GetSection("DatabaseSettings");
            builder.Services.Configure<DatabaseSettings>(databaseSettings);

            var twitterSettings = builder.Configuration.GetSection("TwitterSettings");
            builder.Services.Configure<TwitterSettings>(twitterSettings);

            builder.Services.AddScoped<IPropublicaService, PropublicaService>();
            builder.Services.AddScoped<IVotingTrackRecordRepository, VotingTrackRecordRepository>();
            builder.Services.AddScoped<IVoteTrackerBusiness, VoteTrackerBusiness>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Http(builder.Configuration["ApplicationSettings:LoggingHttpEndpoint"].ToString(), 1000)
                .CreateLogger();

            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(builder.Configuration["ConnectionStrings:HangfireConnection"],
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "Vote Tracker",
                Authorization = new[]
                {
                    new HangfireCustomBasicAuthenticationFilter{
                        User = builder.Configuration.GetSection("HangfireSettings:UserName").Value,
                        Pass = builder.Configuration.GetSection("HangfireSettings:Password").Value
                    }
                }
            });

            BackgroundJob.Schedule(() => Console.WriteLine("Hello world!"), TimeSpan.FromSeconds(5));

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapHangfireDashboard();

            

            app.Run();
        }
    }
}