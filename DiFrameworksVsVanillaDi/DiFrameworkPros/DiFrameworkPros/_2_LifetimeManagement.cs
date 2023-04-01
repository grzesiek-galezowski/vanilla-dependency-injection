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
public class LifetimeManagement
{
  [Test]
  public void ShouldDisposeOfCreatedDependenciesUsingAutofac()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<Lol>();
    using (var container = containerBuilder.Build())
    {
      var lol1 = container.Resolve<Lol>();
      var lol2 = container.Resolve<Lol>();
      Console.WriteLine("opening scope");
      using (var nested = container.BeginLifetimeScope("nested"))
      {
        var lol3 = nested.Resolve<Lol>();
        var lol4 = nested.Resolve<Lol>();
        Console.WriteLine("closing scope");
      } // lol3.Dispose(), lol3.Dispose()
      Console.WriteLine("closed scope");
      var lol5 = container.Resolve<Lol>();
    } // lol1.Dispose(), lol2.Dispose(), lol3.Dispose()
  }

  [Test]
  public void ShouldDisposeOfCreatedDependenciesUsingMsDi()
  {
    var containerBuilder = new ServiceCollection();
    containerBuilder.AddTransient<Lol>();
    using (var container = containerBuilder.BuildServiceProvider())
    {
      var lol1 = container.GetRequiredService<Lol>();
      var lol2 = container.GetRequiredService<Lol>();
      Console.WriteLine("opening scope");
      using (var nested = container.CreateScope())
      {
        var lol3 = nested.ServiceProvider.GetRequiredService<Lol>();
        var lol4 = nested.ServiceProvider.GetRequiredService<Lol>();
        Console.WriteLine("closing scope");
      } // lol3.Dispose(), lol3.Dispose()
      Console.WriteLine("closed scope");
      var lol5 = container.GetRequiredService<Lol>();
    } // lol1.Dispose(), lol2.Dispose(), lol3.Dispose()
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
      var lol1 = new Lol();
      var lol2 = new Lol();
      Console.WriteLine("opening scope");
      {
        var lol3 = new Lol();
        var lol4 = new Lol();
        Console.WriteLine("closing scope");
        lol3.Dispose();
        lol4.Dispose();
      }
      var lol5 = new Lol();

      lol1.Dispose();
      lol2.Dispose();
      lol5.Dispose();
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
      var lol1 = new Lol();
      var lol2 = new Lol();
      disposables.Add(lol1);
      disposables.Add(lol2);

      Console.WriteLine("opening scope");
      {
        using var disposables2 = new Disposables();
        var lol3 = new Lol();
        var lol4 = new Lol();
        disposables2.Add(lol3);
        disposables2.Add(lol4);

        Console.WriteLine("closing scope");
      }
      var lol5 = new Lol();
      disposables.Add(lol5);
    } 
  }

}

file class Lol : IDisposable
{
  private static int _counter = 0;
  private readonly int _currentId;

  public Lol()
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