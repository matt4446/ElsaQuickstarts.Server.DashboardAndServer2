using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Services
{

	public class PersonB
	{
		private readonly ILogger<PersonB> logger;
		private readonly IServiceScope scope;
		private readonly IApproveInputDispatcher approveInputDispatcher;
		private IDisposable worker;

		public PersonB(ILogger<PersonB> logger, IServiceScopeFactory scopeFactory)
		{
			this.logger = logger;
			this.scope = scopeFactory.CreateScope();
			this.approveInputDispatcher = scope.ServiceProvider.GetRequiredService<IApproveInputDispatcher>();
		}

		public void Start(System.Threading.CancellationToken cancellationToken) 
		{
			var seconds = 10;
			this.worker?.Dispose();
			this.worker = Observable
				.Interval(TimeSpan.FromSeconds(seconds))
				.SubscribeOn(NewThreadScheduler.Default)
				.Select(_ => Observable
					.FromAsync(() => ApprovesWork())
					.Do(x => logger.LogInformation("Finished approving any work for a bit. Back in {seconds} seconds", seconds))
					.Catch(Observable.Empty<bool>())
				)
				.Merge()
				.Subscribe();
		}

		private async Task<bool> ApprovesWork() 
		{
			using var _ = logger.BeginScope("{Actor} consuming work", nameof(PersonB));

			this.logger.LogInformation("Checking for some work");


			var currentBookmarks = (await approveInputDispatcher.FindRequiredInputs()).ToList();

			this.logger.LogInformation("Current bookmarks for Person B: {count}", currentBookmarks.Count);

			// kinda pretend the person is approving a particular one (skipping a visit to suspended workflow instances)
			var first = currentBookmarks.FirstOrDefault();

			if (first is not null) 
			{
				Bookmarks.InputApprovalBookmark bookmark = first.Bookmark as Bookmarks.InputApprovalBookmark;
				
				
				bookmark.Input.User = "PersonB";


				var result = await approveInputDispatcher.ExecuteWorkflowsAsync(bookmark.Input, first.CorrelationId, first.WorkflowInstanceId);
			}

			return true;
		}

		public void Dispose()
		{
			this.logger.LogInformation("PersonB shutting down");
			this.worker?.Dispose();
			this.scope?.Dispose();
		}
	}
}
