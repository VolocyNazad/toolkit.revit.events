namespace Revit.Events.Services;

[Flags]
public enum ExternalEventOptions
{
    None = 0,
    AllowDirectInvocation = 1 << 0
}
