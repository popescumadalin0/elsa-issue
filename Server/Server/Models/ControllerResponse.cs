namespace Server.Models;

public class ControllerResponse
{
    public string WorkflowIdentifier { get; set; }

    public bool IsSuccess { get; set; }

    public string ErrorMessage { get; set; }

    public string Stuff { get; set; }
}