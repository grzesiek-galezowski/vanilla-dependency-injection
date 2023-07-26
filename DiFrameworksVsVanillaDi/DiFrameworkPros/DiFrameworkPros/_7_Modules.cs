using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkPros;

//bug incomplete
//Autofac - use modules
//MsDi - use extension methods
//Vanilla - use objects
internal class _7_Modules
{
  public class UsingAutofac
  {
    [Test]
    public void ShouldAllowComposingModulesUsingAutofac()
    {
      //GIVEN
      var builder = new ContainerBuilder();

      builder.RegisterModule<ApplicationLogicModule>();
      builder.RegisterModule<InMemoryOutputModule>();

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
    [Test]
    public void ShouldAllowComposingModulesUsingMsDi()
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
