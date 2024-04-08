using ApplicationLogic.AddNewTodoNote;
using ApplicationLogic.Ports;

namespace ApplicationLogic;

public class ApplicationLogicRoot(ITodoNoteDao todoNoteDao)
{
  public ITodoCommandFactory TodoCommandFactory { get; } = new TodoCommandFactory(
    todoNoteDao,
    new CompoundConversion([
      new ReplacementConversion("truck", "duck"),
      new ReplacementConversion("dick", "thick"),
      new ReplacementConversion("freaking", "flarking")
    ]));
}
