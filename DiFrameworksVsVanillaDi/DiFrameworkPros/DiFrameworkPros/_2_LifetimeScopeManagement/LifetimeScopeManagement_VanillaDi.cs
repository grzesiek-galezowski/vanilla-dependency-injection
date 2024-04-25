using System.Collections.Generic;

namespace DiFrameworkPros._2_LifetimeScopeManagement;

public class LifetimeScopeManagement_VanillaDi
{
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
    var log = new Log();
    {
      var dependency1 = new DisposableDependency(log);
      var dependency2 = new DisposableDependency(log);
      log.OpeningScope();
      {
        var dependency3 = new DisposableDependency(log);
        var dependency4 = new DisposableDependency(log);
        log.ClosingScope();
        dependency4.Dispose();
        dependency3.Dispose();
      }
      log.ClosedScope();
      var dependency5 = new DisposableDependency(log);

      dependency5.Dispose();
      dependency2.Dispose();
      dependency1.Dispose();
    }

    log.Entries.Should()
      .Equal([
        "_____CREATED______0",
        "_____CREATED______1",
        "opening scope",
        "_____CREATED______2",
        "_____CREATED______3",
        "closing scope",
        "_____DISPOSED______3",
        "_____DISPOSED______2",
        "closed scope",
        "_____CREATED______4",
        "_____DISPOSED______4",
        "_____DISPOSED______1",
        "_____DISPOSED______0"
      ]);
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
    var log = new Log();
    {
      using var disposables = new Disposables();
      var dependency1 = new DisposableDependency(log);
      var dependency2 = new DisposableDependency(log);
      disposables.Add(dependency1);
      disposables.Add(dependency2);

      log.OpeningScope();
      {
        using var disposables2 = new Disposables();
        var dependency3 = new DisposableDependency(log);
        var dependency4 = new DisposableDependency(log);
        disposables2.Add(dependency3);
        disposables2.Add(dependency4);
        log.ClosingScope();
      }
      log.ClosedScope();
      var dependency5 = new DisposableDependency(log);
      disposables.Add(dependency5);
    }

    log.Entries.Should()
      .Equal([
        "_____CREATED______0",
        "_____CREATED______1",
        "opening scope",
        "_____CREATED______2",
        "_____CREATED______3",
        "closing scope",
        "_____DISPOSED______3",
        "_____DISPOSED______2",
        "closed scope",
        "_____CREATED______4",
        "_____DISPOSED______4",
        "_____DISPOSED______1",
        "_____DISPOSED______0"
      ]);
  }
}

file class Disposables : IDisposable
{
  private readonly List<IDisposable> _disposables = [];

  public void Add(IDisposable disposable)
    => _disposables.Insert(0, disposable);

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