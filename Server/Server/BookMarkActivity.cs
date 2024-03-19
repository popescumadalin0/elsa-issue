using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Activities.Flowchart.Attributes;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using Server.Models;

namespace Server;

[Activity("Elsa", "Bookmark", "This is a bookmark activity")]
[FlowNode("Success", "Failed")]
public class BookMarkActivity : Activity<object>
{
    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        context.CreateBookmark(new CreateBookmarkArgs
        {
            Payload = new WaitForBookmark
            {
                Identifier = $"{context.Id}-{context.WorkflowExecutionContext.Id}"
            },
            Callback = ResumeAsync

        });
    }

    private async ValueTask ResumeAsync(ActivityExecutionContext context)
    {
        var input = context.GetWorkflowInput<ControllerResponse>($"{context.Id}-{context.WorkflowExecutionContext.Id}");

        if (input.IsSuccess)
        {
            context.Set(Result, input);
            await context.CompleteActivityWithOutcomesAsync("Success");
        }
        else
        {
            context.Set(Result, input);
            await context.CompleteActivityWithOutcomesAsync("Failed");
        }
    }
}