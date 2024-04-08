using ApplicationLogic.Ports;

namespace ApplicationLogic;

public class ApplicationLogicRoot(ITodoNoteDao todoNoteDao)
{
  public ITodoCommandFactory TodoCommandFactory { get; } = new TodoCommandFactory(todoNoteDao);
}