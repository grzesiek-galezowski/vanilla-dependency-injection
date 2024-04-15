using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkPros;

/// <summary>
/// This example shows automatic lifetime management.
/// DI containers automatically dispose of their created objects
/// when the scope where they were created is over.
/// </summary>
public class LifetimeScopeManagement
{
  [Test]
  public void ShouldDisposeOfCreatedDependenciesUsingAutofac()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<DisposableDependency>();
    using (var container = containerBuilder.Build())
    {
      container.Resolve<DisposableDependency>(); //0
      container.Resolve<DisposableDependency>(); //1

      Console.WriteLine("opening scope");
      using (var nested = container.BeginLifetimeScope())
      {
        nested.Resolve<DisposableDependency>();  //2
        nested.Resolve<DisposableDependency>();  //3
        Console.WriteLine("closing scope");
      } // 3.Dispose(), 2.Dispose()
      Console.WriteLine("closed scope");

      container.Resolve<DisposableDependency>(); //4

    } // 4.Dispose(), 1.Dispose(), 0.Dispose()
  }

  [Test]
  public void ShouldDisposeOfCreatedDependenciesUsingMsDi()
  {
    var containerBuilder = new ServiceCollection();
    containerBuilder.AddTransient<DisposableDependency>();
    using (var container = containerBuilder.BuildServiceProvider())
    {
      container.GetRequiredService<DisposableDependency>(); //0
      container.GetRequiredService<DisposableDependency>(); //1

      Console.WriteLine("opening scope");
      using (var nested = container.CreateScope())
      {
        nested.ServiceProvider.GetRequiredService<DisposableDependency>(); //2
        nested.ServiceProvider.GetRequiredService<DisposableDependency>(); //3
        Console.WriteLine("closing scope");
      } // 3.Dispose(), 2.Dispose()
      Console.WriteLine("closed scope");
      container.GetRequiredService<DisposableDependency>(); //4
    } // 4.Dispose(), 1.Dispose(), 0.Dispose()
  }

  /// <summary>
  /// In case of manual DI, dependencies have to be disposed by hand.
  /// Also, exception thrown from a Dispose() prevents further Dispose() calls.
  /// Fortunately I don't typically encounter many disposable objects
  /// that need to go beyond single method scope (e.g. that cannot be handled with
  /// 'using()' or 'using var', however...
  /// </summary>
  [Test]
  public void ShouldDisposeOfCreatedDependenciesUsingVanillaDependencyInjection()
  {
    {
      var dependency1 = new DisposableDependency();
      var dependency2 = new DisposableDependency();
      Console.WriteLine("opening scope");
      {
        var dependency3 = new DisposableDependency();
        var dependency4 = new DisposableDependency();
        Console.WriteLine("closing scope");
        dependency3.Dispose();
        dependency4.Dispose();
      }
      var dependency5 = new DisposableDependency();

      dependency1.Dispose();
      dependency2.Dispose();
      dependency5.Dispose();
    } 
  }

  /// <summary>
  /// ... if it really becomes a problem, I can use a simple disposal subsystem.
  /// BTW if a framework like ASP.Net Core starts a scope, it may make sense to plug into its
  /// scope by registering lambda from manual composition root as created within a container scope.
  /// This might be considering "cheating" because it uses the container, but it's only used
  /// for scope control which belongs to the framework anyway.
  /// </summary>
  [Test]
  public void ShouldDisposeOfCreatedDependenciesUsingVanillaDependencyInjection2()
  {
    {
      using var disposables = new Disposables();
      var dependency1 = new DisposableDependency();
      var dependency2 = new DisposableDependency();
      disposables.Add(dependency1);
      disposables.Add(dependency2);

      Console.WriteLine("opening scope");
      {
        using var disposables2 = new Disposables();
        var dependency3 = new DisposableDependency();
        var dependency4 = new DisposableDependency();
        disposables2.Add(dependency3);
        disposables2.Add(dependency4);

        Console.WriteLine("closing scope");
      }
      var dependency5 = new DisposableDependency();
      disposables.Add(dependency5);
    } 
  }
}

file class DisposableDependency : IDisposable
{
  private static int _counter = 0;
  private readonly int _currentId;

  public DisposableDependency()
  {
    _currentId = _counter++;
    Console.WriteLine("_____CREATED______" + _currentId);
  }

  public void Dispose()
  {
    Console.WriteLine("_____DISPOSED______" + _currentId);
  }
}

file class Disposables : IDisposable
{
  private readonly List<IDisposable> _disposables = new();

  public void Add(IDisposable disposable) => _disposables.Add(disposable);

  public void Dispose()
  {
    foreach (var disposable in _disposables)
    {
      try
      {
        disposable.Dispose();
      }
      catch (Exception e)
      {
        Console.WriteLine(e); //might be logging or sth. else
      }
    }
  }
}