using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Activities
{
	[Trigger(Category = "Content", DisplayName = "Content Approval")]
	public class InputApprovalActivity : Activity
	{
		private readonly ILogger<InputApprovalActivity> logger;

		public InputApprovalActivity(ILogger<InputApprovalActivity> logger)
		{
			this.logger = logger;
		}

		[ActivityInput(Hint = "Require User Input", UIHint = ActivityInputUIHints.Checkbox)]
		public bool RequiresApproval { get; set; } = true;

		[ActivityOutput]
		public bool IsApproved { get; set;  }

		[ActivityOutput]
		public string OriginalInput { get; set; }

		[ActivityOutput]
		public string AcceptedInput { get; set; }


        private ValueTask<bool> Approved(ActivityExecutionContext context)
        {
			if (this.IsApproved) 
			{
				this.logger.LogInformation("Is already approved: {isApproved}", this.IsApproved);
				return new(true);
			}

            if (context.GetInput<Models.ApproveInput>() is not null)
            {
                var input = context.GetInput<Models.ApproveInput>();
				this.logger.LogInformation("check if input is approved: {isApproved}", input.IsApproved);
				if (input.IsApproved)
                {
                    this.IsApproved = true;
                    this.AcceptedInput = input.Json;

                    return new(true);
                }
                else 
                {
                    this.OriginalInput = input.Json;
                }
            }
			

            return new(false);
        }

		private async ValueTask<IActivityExecutionResult> Triggerd(ActivityExecutionContext context) 
		{
			var approved = await this.Approved(context);

			if (approved) 
			{
				return Done();
			}

			return Suspend();
		}

		protected override ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context) => Triggerd(context);

		protected override ValueTask<IActivityExecutionResult> OnResumeAsync(ActivityExecutionContext context) => Triggerd(context);
	}
}
