using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Models;

namespace Server;

[Route("api")]
[ApiController]
public class ApiController(
        IWorkflowInbox workflowInbox)
    : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    [HttpPost("ResumeBookmark")]
    public async Task ResumeBookmark(ControllerResponse response)
    {
        Console.WriteLine("ResumeBookmark");

        var bookmarkFilter = new WaitForBookmark()
        {
            Identifier = response.WorkflowIdentifier
        };
        var input = new Dictionary<string, object>() { { response.WorkflowIdentifier, response } };
        var workflowInboxMessage = NewWorkflowInboxMessage.For<BookMarkActivity>(bookmarkFilter, input: input);
        await workflowInbox.SubmitAsync(workflowInboxMessage);
    }
}