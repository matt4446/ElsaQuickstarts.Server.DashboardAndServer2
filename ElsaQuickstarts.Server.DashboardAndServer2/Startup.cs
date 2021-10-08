using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Elsa;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Microsoft.Extensions.Configuration;
using Elsa.Persistence.YesSql;
using ElsaQuickstarts.Server.DashboardAndServer2.Services;
using YesSql.Provider.Sqlite;
using Elsa.Runtime;

namespace ElsaQuickstarts.Server.DashboardAndServer2
{
	public class Startup
	{
		public IWebHostEnvironment Environment { get; private set; }
		public IConfiguration Configuration { get; private set; }

		public Startup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			var elsaSection = Configuration.GetSection("Elsa");

			// Elsa services.
			services
				.AddElsa(elsa => 
					elsa
					.UseYesSqlPersistence(config =>
					{
						config.UseSqLite($"Data Source=elsayessql.db;Cache=Shared");
						//config
						//.UseSqlite(@"Data Source=yessql.db;Cache=Shared")
						//.UseSqlServer(connectionString)
						//.SetTablePrefix("Elsa2_");
					})
					//.UseEntityFrameworkPersistence(ef => ef.UseSqlite())
					.AddConsoleActivities()
					.AddHttpActivities(elsaSection.GetSection("Server").Bind)
					.AddQuartzTemporalActivities()
					.AddWorkflowsFrom<Startup>()
					.AddActivitiesFrom<Startup>()
					
				);

			// Elsa API endpoints.
			services.AddElsaApiEndpoints();
			services.AddBookmarkProvider<Bookmarks.InputApprovalBookmarkProvider>();

			// For Dashboard.
			services.AddRazorPages();

			services.AddSingleton<Services.PersonA>();
			services.AddSingleton<Services.PersonB>();
			services.AddScoped<IApproveInputDispatcher, ApproveInputDispatcher>();
			services.AddStartupTask<StartupTasks.StartThePretendPeople>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app
				.UseStaticFiles() // For Dashboard.
				.UseHttpActivities()
				.UseRouting()
				.UseEndpoints(endpoints =>
				{
					// Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
					endpoints.MapControllers();

					// For Dashboard.
					endpoints.MapFallbackToPage("/_Host");
				});
		}
	}
}
