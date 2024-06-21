using Pure.DI;

namespace DiFrameworkPros._7_Modules;

internal class Modules_PureDiLibrary
{
  [Test]
  public void ShouldAllowComposingModules()
  {
    //GIVEN
    var composition = new FinalComposition();
    var logic = composition.AppLogic;

    ////WHEN
    logic.PerformAction();
    
    //THEN
    ((ListOutput)composition.Output).Content.Should().Be("Hello");
  }
}

public partial class FinalComposition
{
  public void Setup()
  {
    DI.Setup(nameof(FinalComposition))
      .DependsOn(
        nameof(ApplicationLogicCompositionPart),
        nameof(InMemoryOutputCompositionPart))
      .Root<IApplicationLogic>("AppLogic");
  }
}


public partial class ApplicationLogicCompositionPart
{
  public void Setup()
  {
    DI.Setup(nameof(ApplicationLogicCompositionPart))
      .Bind<IApplicationLogic>().To<MyApplicationLogic>();
  }
}

public partial class InMemoryOutputCompositionPart
{
  public void Setup()
  {
    DI.Setup(nameof(InMemoryOutputCompositionPart))
      .RootBind<IApplicationLogicOutput>("Output").As(Lifetime.Singleton).To<ListOutput>();
  }
}

public partial class ConsoleOutputCompositionPart
{
  public void Setup()
  {
    DI.Setup(nameof(ConsoleOutputCompositionPart))
      .RootBind<IApplicationLogicOutput>("Output").As(Lifetime.Singleton).To<ConsoleOutput>();
  }
}