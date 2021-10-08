using Elsa.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElsaQuickstarts.Server.DashboardAndServer2.Bookmarks
{
    public class InputApprovalBookmarkProvider : BookmarkProvider<Bookmarks.InputApprovalBookmark, Activities.InputApprovalActivity>
    {
        private readonly ILogger<InputApprovalBookmarkProvider> logger;

        public InputApprovalBookmarkProvider(ILogger<InputApprovalBookmarkProvider> logger)
        {
            this.logger = logger;
        }

        public override async ValueTask<IEnumerable<BookmarkResult>> GetBookmarksAsync(BookmarkProviderContext<Activities.InputApprovalActivity> context, CancellationToken cancellationToken)
        {
            var requireInput = await RequireBookmark(context, cancellationToken);

            if (requireInput)
            {
                using var _a = logger.BeginScope("{CorrelationId}", context.ActivityExecutionContext.CorrelationId);

                Models.ApproveInput input = await GetInputAsync(context, cancellationToken);

                if (input != null)
                {
                    this.logger.LogInformation("Input needs approval: {@Input}", input);

                    return new[] {
                    this.Result(new Bookmarks.InputApprovalBookmark() {
                        Input = input
                    })};
                }
            }

            return Enumerable.Empty<BookmarkResult>();
        }

        private static async ValueTask<bool> RequireBookmark(BookmarkProviderContext<Activities.InputApprovalActivity> context, CancellationToken cancellationToken) =>
            context.Mode == BookmarkIndexingMode.WorkflowInstance
                ? context.Activity.GetPropertyValue(x => x.RequiresApproval)
                : await context.ReadActivityPropertyAsync(x => x.RequiresApproval, cancellationToken);

        private static async ValueTask<Models.ApproveInput> GetInputAsync(BookmarkProviderContext<Activities.InputApprovalActivity> context, CancellationToken cancellationToken) 
        {
            var model = new Models.ApproveInput();

            if (context.Mode == BookmarkIndexingMode.WorkflowInstance)
            {
                model.Json = context.Activity.GetPropertyValue(x => x.OriginalInput);
            }
            else 
            {
                model.Json = await context.ReadActivityPropertyAsync(x => x.OriginalInput, cancellationToken);
            }

            return model;
        }

    }
}
