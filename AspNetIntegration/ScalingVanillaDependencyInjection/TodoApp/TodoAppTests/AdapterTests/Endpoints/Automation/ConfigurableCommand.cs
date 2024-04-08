using ApplicationLogic.Ports;

namespace TodoAppTests.AdapterTests.Endpoints.Automation;

internal class ConfigurableCommand(Func<CancellationToken, Task> func) : ITodoAppCommand
{
  public async Task Execute(CancellationToken cancellationToken)
  {
    await func.Invoke(cancellationToken);
  }
}