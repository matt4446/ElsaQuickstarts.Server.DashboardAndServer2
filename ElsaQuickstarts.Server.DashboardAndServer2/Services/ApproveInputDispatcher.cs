using Elsa;
using Elsa.Services;
using Elsa.Services.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Services
{
	public class ApproveInputDispatcher : IApproveInputDispatcher
	{
		private readonly IWorkflowLaunchpad workflowLaunchpad;
		private readonly ITriggerFinder triggerFinder;
		private readonly IBookmarkFinder bookmarkFinder;
		//private readonly ITriggerStore triggerStore; 
		public ApproveInputDispatcher(IWorkflowLaunchpad workflowLaunchpad, ITriggerFinder triggerFinder, IBookmarkFinder bookmarkFinder)
		{
			this.workflowLaunchpad = workflowLaunchpad;
			this.triggerFinder = triggerFinder;
			this.bookmarkFinder = bookmarkFinder;
		}

		public async Task<IEnumerable<BookmarkFinderResult>> FindRequiredInputs(string correlationId = null)
		{
			var all = await bookmarkFinder.FindBookmarksAsync<Activities.InputApprovalActivity>(correlationId: correlationId);

			return all;
		}

		public async Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(Models.ApproveInput input, string correlationId = null, string workflowInstanceId = null, CancellationToken cancellationToken = default)
		{
			var context = new WorkflowsQuery(nameof(Activities.InputApprovalActivity), new Bookmarks.InputApprovalBookmark(), CorrelationId: correlationId, WorkflowInstanceId: workflowInstanceId);
			return await workflowLaunchpad.CollectAndDispatchWorkflowsAsync(context, new Elsa.Models.WorkflowInput(input), cancellationToken);
		}

		public async Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(Models.ApproveInput input, string correlationId = null, string workflowInstanceId = null, CancellationToken cancellationToken = default)
		{
			var context = new WorkflowsQuery(nameof(Activities.InputApprovalActivity), new Bookmarks.InputApprovalBookmark(), CorrelationId: correlationId, WorkflowInstanceId: workflowInstanceId);
			return await workflowLaunchpad.CollectAndExecuteWorkflowsAsync(context, new Elsa.Models.WorkflowInput(input), cancellationToken);
		}

	
	}
}
