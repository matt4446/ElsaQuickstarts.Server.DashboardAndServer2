using Elsa.Services;
using ElsaQuickstarts.Server.DashboardAndServer2.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.StartupTasks
{
	public class StartThePretendPeople : IStartupTask
	{
		private readonly PersonA personA;
		private readonly PersonB personB;

		public StartThePretendPeople(
			Services.PersonA personA, 
			Services.PersonB personB)
		{
			this.personA = personA;
			this.personB = personB;
		}

		public int Order => 100000;

		public Task ExecuteAsync(CancellationToken cancellationToken = default)
		{
			Actors.Start(personA, personB, cancellationToken);

			return Task.CompletedTask;
		}
	}
}
