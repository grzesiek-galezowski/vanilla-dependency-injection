using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkPros;

/// <summary>
/// Some DI containers allow using modules. Modules are recipes for fragments of composition.
/// They can be used to:
/// - parametrize fragment of object composition recipe
/// - logically split the composition into several parts so that each part
///   could potentially be replaced
/// - If combined with visibility modifiers, can also serve as
///   quasi-architectural component, where the module consists of registrations
///   of mostly internal types and the registered public types serve as a boundary
/// </summary>
internal class _7_Modules
{
  public class UsingAutofac
  {
    /// <summary>
    /// Autofac has modules built-in.
    /// </summary>
    [Test]
    public void ShouldAllowComposingModulesUsingAutofac()
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
      Assert.AreEqual("Hello",
        ((ListOutput)container.Resolve<IApplicationLogicOutput>()).Content);
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

  public class UsingMsDi
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
    public void ShouldAllowComposingModulesUsingMsDi()
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
      Assert.AreEqual("Hello",
        ((ListOutput)container.GetRequiredService<IApplicationLogicOutput>()).Content);
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

  public class UsingVanillaDi
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
    public void ShouldAllowComposingModulesUsingVanillaDi()
    {
      //GIVEN
      var outputModule = new InMemoryOutputModule();
      var applicationLogicModule =
        new ApplicationLogicModule(outputModule.Output);

      //WHEN
      applicationLogicModule.ApplicationLogic.PerformAction();

      //THEN
      Assert.AreEqual("Hello",
        ((ListOutput)outputModule.Output).Content);
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
}

file class ListOutput : IApplicationLogicOutput
{
  public string Content { get; private set; } = string.Empty;

  public void Write(string text)
  {
    Content += text;
  }
}

file class ConsoleOutput : IApplicationLogicOutput
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

file class MyApplicationLogic : IApplicationLogic
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
