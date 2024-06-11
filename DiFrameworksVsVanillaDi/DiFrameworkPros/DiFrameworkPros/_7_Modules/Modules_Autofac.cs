using Autofac;

namespace DiFrameworkPros._7_Modules;

internal class UsingAutofac
{
  /// <summary>
  /// Autofac has modules built-in.
  /// This example shows how one module (InMemoryOutputModule)
  /// delivers an implementation of an interface required by a class
  /// from another module (ApplicationLogicModule).
  /// </summary>
  [Test]
  public static void ShouldAllowComposingModules()
  {
    //GIVEN
    var builder = new ContainerBuilder();

    // modules can either be registered as types
    builder.RegisterModule<ApplicationLogicModule>();
    // or as objects - in which case additional arguments can easily be
    // passed to a constructor.
    builder.RegisterModule(new InMemoryOutputModule());

    //WHEN
    using var container = builder.Build();

    var logic = container.Resolve<IApplicationLogic>();

    logic.PerformAction();

    //THEN
    ((ListOutput)container.Resolve<IApplicationLogicOutput>()).Content
      .Should().Be("Hello");
  }

  private class ApplicationLogicModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<MyApplicationLogic>()
        .As<IApplicationLogic>()
        .SingleInstance();
    }
  }

  private class ConsoleOutputModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<ConsoleOutput>()
        .As<IApplicationLogicOutput>()
        .SingleInstance();
    }
  }

  private class InMemoryOutputModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<ListOutput>()
        .As<IApplicationLogicOutput>()
        .SingleInstance();
    }
  }
}