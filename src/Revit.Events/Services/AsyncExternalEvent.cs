using Autodesk.Revit.UI;
using Revit.Events.Abstractions.Services;
using Revit.Events.Infrastructure;

namespace Revit.Events.Services;

internal sealed class AsyncExternalEvent : IAsyncExternalEvent
{
    public Task Raise(Action<UIApplication> action, ExternalEventOptions options = ExternalEventOptions.None)
    {
        if ((options & ExternalEventOptions.AllowDirectInvocation) != 0 && RevitContextManager.IsRevitInApiMode)
        {
            try
            {
                action(RevitContextManager.UiApplication);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        TaskCompletionSource<object?> taskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

        void AsyncAction(UIApplication uiApplication)
        {
            try
            {
                action.Invoke(uiApplication);
                taskCompletionSource.SetResult(null);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        }

        IExternalEventHandler handler = new LambdaExternalEventHandler(AsyncAction);

        Autodesk.Revit.UI.ExternalEvent externalEvent;
        using (RevitContextManager.BeginApiContextScope())
        {
            externalEvent = Autodesk.Revit.UI.ExternalEvent.Create(handler);
            externalEvent.Raise();
        }
        
        // Добавляем продолжение для отслеживания ошибок
        return taskCompletionSource.Task;
    }
}
