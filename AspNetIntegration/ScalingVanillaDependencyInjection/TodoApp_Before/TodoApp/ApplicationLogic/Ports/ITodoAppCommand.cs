namespace TodoApp.ApplicationLogic.Ports;

public interface ITodoAppCommand
{
  Task Execute(CancellationToken cancellationToken);
}