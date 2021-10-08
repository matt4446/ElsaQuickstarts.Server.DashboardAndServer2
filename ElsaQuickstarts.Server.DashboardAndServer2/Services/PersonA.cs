using Elsa.Services;
using ElsaQuickstarts.Server.DashboardAndServer2.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Services
{
	public static class Actors 
	{
		private static PersonA PersonA { get; set; }
		private static PersonB PersonB { get; set; }

		public static void Start(PersonA a, PersonB b, System.Threading.CancellationToken cancellationToken) 
		{
			PersonA = a;
			PersonB = b;
			a.Start(cancellationToken);
			b.Start(cancellationToken);
		}
	}

	public class PersonA: IDisposable
	{
		private readonly IApproveInputDispatcher approveInputDispatcher;
		private readonly ILogger<PersonA> logger;
		private readonly IServiceScope scope;
		private IDisposable worker;
		

		public PersonA(ILogger<PersonA> logger, IServiceScopeFactory scopeFactory)
		{
			this.logger = logger;
			this.scope = scopeFactory.CreateScope();
			this.approveInputDispatcher = scope.ServiceProvider.GetRequiredService<IApproveInputDispatcher>();
		}

		public void Start(System.Threading.CancellationToken cancellationToken) 
		{
			var seconds = 30; 
			this.worker?.Dispose();
			this.worker = Observable
				.Interval(TimeSpan.FromSeconds(seconds))
				.SubscribeOn(NewThreadScheduler.Default)
				.Select(_ =>
					Observable
						.FromAsync(() => CreatesWork())
						.Do(_ => this.logger.LogInformation("Finished creating work for {seconds} secnods", seconds))
						.Catch(Observable.Empty<bool>())
				)
				.Merge()
				.Subscribe();
		}

		private async Task<bool> CreatesWork() 
		{
			using var _ = logger.BeginScope("{Actor} creates work}", nameof(PersonA));

			var input = new ApproveInput() 
			{
				Json = $"Current Datetime: {DateTime.UtcNow:u}",
				User = "Person A"
			};
			this.logger.LogInformation("Person A created some work. Approve: {input}", input);

			var workflowRunner = this.scope.ServiceProvider.GetRequiredService<IBuildsAndStartsWorkflow>();
			var result = await workflowRunner.BuildAndStartWorkflowAsync<Workflows.TestInputApprovalWorkflow>(input: new Elsa.Models.WorkflowInput(input), correlationId: $"{DateTime.UtcNow:u}");

			this.logger.LogInformation("Created some test work. Items {@workflowResult}", result);

			return true;
		}

		public void Dispose()
		{
			this.logger.LogInformation("Person A shutting down");
			this.worker?.Dispose();
			this.scope?.Dispose();
		}
	}
}
