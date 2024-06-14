using Autofac;

namespace DiFrameworkPros._2_LifetimeScopeManagement.Autofac;

public static class _1_BuiltinManagement
{
  [Test]
  public static void ShouldDisposeOfCreatedDependencies()
  {
    var log = new Log();
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterInstance(log);
    containerBuilder.RegisterType<DisposableDependency>();
    using (var container = containerBuilder.Build())
    {
      container.Resolve<DisposableDependency>(); //0
      container.Resolve<DisposableDependency>(); //1

      log.OpeningScope();
      using (var nested = container.BeginLifetimeScope())
      {
        nested.Resolve<DisposableDependency>();  //2
        nested.Resolve<DisposableDependency>();  //3
        log.ClosingScope();
      } // 3.Dispose(), 2.Dispose()
      log.ClosedScope();

      container.Resolve<DisposableDependency>(); //4
    } // 4.Dispose(), 1.Dispose(), 0.Dispose()

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