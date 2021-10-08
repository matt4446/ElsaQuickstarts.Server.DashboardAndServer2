using Elsa.Services;
using Elsa.Services.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Bookmarks
{
	public interface ICodeActivityInputRecievedInvoker
    {
        Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(Models.ApproveInput input, string correlationId, string workflowInstanceId, CancellationToken cancellationToken = default);
        Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(Models.ApproveInput input, string correlationId, string workflowInstanceId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BookmarkFinderResult>> FindRequiredInputs(string correlationId);
    }
}
