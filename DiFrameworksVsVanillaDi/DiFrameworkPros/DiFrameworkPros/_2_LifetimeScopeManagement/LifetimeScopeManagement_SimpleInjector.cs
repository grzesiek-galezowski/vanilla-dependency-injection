using DiFrameworkPros.HelperCode;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DiFrameworkPros._2_LifetimeScopeManagement;

public static class LifetimeScopeManagement_SimpleInjector
{
  [Test]
  public static void ShouldDisposeOfCreatedDependenciesUsingLamar()
  {
    var log = new Log();
    using (var container = new Container())
    {
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterInstance(log);
      // SimpleInjector does not seem to support auto disposing of transient objects
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
        } // 1.Dispose()
        log.ClosedScope();

        container.GetRequiredService<DisposableDependency3>(); //2
        log.ClosingScope();
      } // 2.Dispose(), 0.Dispose()
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
        "_____CREATED______2",
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