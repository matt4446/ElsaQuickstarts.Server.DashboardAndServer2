using Elsa.Builders;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Workflows
{
	public class TestInputApprovalWorkflow : IWorkflow
	{
		public void Build(IWorkflowBuilder builder)
		{
			builder
				.Add<Activities.InputApprovalActivity>(setup: setup =>
				{
					setup.Set(x => x.RequiresApproval, true);
				});
				
		}
	}

}
