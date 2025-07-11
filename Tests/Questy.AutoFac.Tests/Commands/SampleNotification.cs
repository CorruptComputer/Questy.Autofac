namespace Questy.AutoFac.Tests.Commands;

public class SampleNotification : INotification
{
    public string Message { get; }

    public SampleNotification(string message)
    {
        this.Message = message;
    }
}