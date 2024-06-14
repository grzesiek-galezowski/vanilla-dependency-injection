using System.Collections.Generic;

namespace DiFrameworkPros._2_LifetimeScopeManagement.VanillaDi;

public class _2_ManualScopeManagementWithHelperObject
{
  /// <summary>
  /// ... if it really becomes a problem, I can use a simple disposal subsystem.
  /// BTW if a framework like ASP.Net Core starts a scope, it may make sense to plug into its
  /// scope by registering lambda from manual composition root as created within a container scope.
  /// This might be considering "cheating" because it uses the container, but it's only used
  /// for scope control which belongs to the framework anyway.
  /// </summary>
  [Test]
  public static void ShouldDisposeOfCreatedDependenciesUsingVanillaDependencyInjection2()
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