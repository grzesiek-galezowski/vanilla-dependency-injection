using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._7_Modules;

internal class Modules_MsDi
{
  // MS DI doesn't support modules, but the usual way
  // to work around it is to use static methods.
  // In this example, the static methods are called on
  // their respective classes (so that each method can be called Add())
  // but a more idiomatic way is to use extension methods, e.g.
  // container.AddApplicationLogic();
  // container.AddHttpClient();
  // etc.
  [Test]
  public void ShouldAllowComposingModules()
  {
    //GIVEN
    var builder = new ServiceCollection();

    MsDiApplicationLogicModule.AddTo(builder);
    MsDiInMemoryOutputModule.AddTo(builder);

    //WHEN
    using var container = builder.BuildServiceProvider();

    var logic = container.GetRequiredService<IApplicationLogic>();

    logic.PerformAction();

    //THEN
    ((ListOutput)container.GetRequiredService<IApplicationLogicOutput>()).Content.Should().Be("Hello");
  }

  private static class MsDiApplicationLogicModule
  {
    public static void AddTo(IServiceCollection container)
    {
      container.AddSingleton<IApplicationLogic, MyApplicationLogic>();
    }
  }

  private static class MsDiInMemoryOutputModule
  {
    public static void AddTo(IServiceCollection container)
    {
      container.AddSingleton<IApplicationLogicOutput, ListOutput>();
    }
  }

  private static class MsDiConsoleOutputModule
  {
    public static void Add(IServiceCollection container)
    {
      container.AddSingleton<IApplicationLogicOutput, ConsoleOutput>();
    }
  }
}