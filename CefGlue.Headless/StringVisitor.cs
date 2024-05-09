using Xilium.CefGlue;

namespace CefGlue.Headless;

public class StringVisitor : CefStringVisitor
{
    public TaskCompletionSource<string> valueTask = new();

    public Task<string> GetValueAsync()
    {
        return valueTask.Task;
    }

    public bool IsValueAvailable => valueTask.Task.IsCompleted;
    public string Value => valueTask.Task.Result;
    protected override void Visit(string value)
    {
        valueTask.TrySetResult(value);
    }
}