using Elsa.Services;
using Elsa.Services.Models;
using ElsaQuickstarts.Server.DashboardAndServer2.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Services
{
	public interface IApproveInputDispatcher
	{
		Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(ApproveInput input, string correlationId = null, string workflowInstanceId = null, CancellationToken cancellationToken = default);
		Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(ApproveInput input, string correlationId = null, string workflowInstanceId = null, CancellationToken cancellationToken = default);
		Task<IEnumerable<BookmarkFinderResult>> FindRequiredInputs(string correlationId =  null);
	}
}