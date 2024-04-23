namespace DiFrameworkPros._7_Modules;

internal class ListOutput : IApplicationLogicOutput
{
  public string Content { get; private set; } = string.Empty;

  public void Write(string text)
  {
    Content += text;
  }
}

internal class ConsoleOutput : IApplicationLogicOutput
{
  public void Write(string text)
  {
    Console.WriteLine(text);
  }
}

public interface IApplicationLogicOutput
{
  void Write(string text);
}

public interface IApplicationLogic
{
  void PerformAction();
}

internal class MyApplicationLogic : IApplicationLogic
{
  private readonly IApplicationLogicOutput _output;

  public MyApplicationLogic(IApplicationLogicOutput output)
  {
    _output = output;
  }

  public void PerformAction()
  {
    _output.Write("Hello");
  }
}