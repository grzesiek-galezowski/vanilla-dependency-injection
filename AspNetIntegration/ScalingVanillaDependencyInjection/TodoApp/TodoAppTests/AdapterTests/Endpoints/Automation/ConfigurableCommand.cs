using ApplicationLogic.Ports;

namespace TodoAppTests.AdapterTests.Endpoints.Automation;

internal class ConfigurableCommand : ITodoAppCommand
{
    private readonly Func<CancellationToken, Task> _func;

    public ConfigurableCommand(Func<CancellationToken, Task> func)
    {
        _func = func;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        await _func.Invoke(cancellationToken);
    }
}