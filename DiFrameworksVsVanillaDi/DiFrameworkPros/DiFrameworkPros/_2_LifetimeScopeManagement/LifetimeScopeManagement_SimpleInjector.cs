using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DiFrameworkPros._2_LifetimeScopeManagement;

public static class LifetimeScopeManagement_SimpleInjector
{
  /// <summary>
  /// SimpleInjector does not seem to support auto disposing of transient objects.
  /// (actually, it throws when a transient implements IDisposable, but
  /// after disabling that check the transients aren't auto-disposed anyway)
  /// 
  /// That's why this example uses scoped lifetimes.
  ///
  /// Also, the end log is different than I expected.
  /// Either there is a bug in this code, or SimpleInjector
  /// works differently than e.g. Autofac or MsDi.
  /// </summary>
  [Test]
  public static void ShouldDisposeOfCreatedDependenciesUsingSimpleInjector()
  {
    var log = new Log();
    using (var container = new Container())
    {
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterInstance(log);
      container.Register<DisposableDependency>(Lifestyle.Scoped);
      container.Register<DisposableDependency2>(Lifestyle.Scoped);
      container.Register<DisposableDependency3>(Lifestyle.Scoped);

      log.OpeningScope();
      using (var mainScope = AsyncScopedLifestyle.BeginScope(container))
      {
        mainScope.GetInstance<DisposableDependency>(); //0
        mainScope.GetInstance<DisposableDependency2>(); //1
        log.OpeningScope();
        using (var nested = AsyncScopedLifestyle.BeginScope(container))
        {
          nested.GetInstance<DisposableDependency>(); //2
          nested.GetInstance<DisposableDependency2>(); //3

          log.ClosingScope();
        }
        log.ClosedScope();

        container.GetRequiredService<DisposableDependency3>(); //2
        log.ClosingScope();
      }
      log.ClosedScope();

    }

    // This sequence seems slightly strange, as if
    // all the scoped instances were resolved together but only in the
    // topmost scope...
    log.Entries.Should()
      .Equal([
        "opening scope",
        "_____CREATED______0",
        "_____CREATED______1",
        "_____CREATED______2", //... what..?
        "opening scope",
        "_____CREATED______3",
        "_____CREATED______4",
        "closing scope",
        "_____DISPOSED______4",
        "_____DISPOSED______3",
        "closed scope",
        "closing scope",
        "_____DISPOSED______2",
        "_____DISPOSED______1",
        "_____DISPOSED______0",
        "closed scope"
      ]);
  }
}