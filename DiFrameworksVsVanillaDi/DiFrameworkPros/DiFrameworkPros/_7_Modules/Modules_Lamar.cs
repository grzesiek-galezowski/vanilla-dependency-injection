using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._7_Modules;

internal class Modules_Lamar
{
  /// <summary>
  /// Lamar supports modules, called registries.
  /// it has several ways of instantiating and passing
  /// the registries to the container.
  ///
  /// It doesn't including support multiple instances of the same
  /// registry type though.
  /// </summary>
  [Test]
  public void ShouldAllowComposingModules()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.IncludeRegistry<LamarApplicationLogicRegistry>();
      builder.IncludeRegistry<LamarInMemoryOutputRegistry>();
    });

    //WHEN
    var logic = container.GetRequiredService<IApplicationLogic>();

    logic.PerformAction();

    //THEN
    ((ListOutput)container.GetRequiredService<IApplicationLogicOutput>()).Content.Should().Be("Hello");
  }

  private class LamarApplicationLogicRegistry : ServiceRegistry
  {
    public LamarApplicationLogicRegistry()
    {
      this.AddSingleton<IApplicationLogic, MyApplicationLogic>();
    }
  }

  private class LamarInMemoryOutputRegistry : ServiceRegistry
  {
    public LamarInMemoryOutputRegistry()
    {
      this.AddSingleton<IApplicationLogicOutput, ListOutput>();
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