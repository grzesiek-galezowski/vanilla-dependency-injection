namespace DiFrameworkPros._7_Modules;

internal class Modules_VanillaDi
{
  /// <summary>
  /// Unless you need some advanced features of DI container
  /// modules (e.g. assembly scanning for modules), they can
  /// be easily replaced by classes that encapsulate creating
  /// fragments of object graph and expose public methods and
  /// properties to allow access only to the stuff that needs
  /// to be externally visible.
  ///
  /// This is merely old-fashioned OO.
  /// </summary>
  [Test]
  public void ShouldAllowComposingModules()
  {
    //GIVEN
    var outputModule = new InMemoryOutputModule();
    var applicationLogicModule =
      new ApplicationLogicModule(outputModule.Output);

    //WHEN
    applicationLogicModule.ApplicationLogic.PerformAction();

    //THEN
    ((ListOutput)outputModule.Output).Content.Should().Be("Hello");
  }

  private class ApplicationLogicModule
  {
    public IApplicationLogic ApplicationLogic { get; }

    public ApplicationLogicModule(IApplicationLogicOutput output)
    {
      ApplicationLogic = new MyApplicationLogic(output);
    }
  }

  private class InMemoryOutputModule
  {
    public InMemoryOutputModule()
    {
      Output = new ListOutput();
    }

    public IApplicationLogicOutput Output { get; }
  }

  private class ConsoleOutputModule
  {
    public ConsoleOutputModule()
    {
      OutputModule = new ConsoleOutput();
    }

    public IApplicationLogicOutput OutputModule { get; }
  }
}