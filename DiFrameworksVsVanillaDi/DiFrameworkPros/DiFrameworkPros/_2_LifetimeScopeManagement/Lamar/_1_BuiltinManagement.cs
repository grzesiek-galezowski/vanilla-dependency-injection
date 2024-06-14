using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._2_LifetimeScopeManagement.Lamar;

public static class _1_BuiltinManagement
{
  [Test]
  public static void ShouldDisposeOfCreatedDependenciesUsingLamar()
  {
    var log = new Log();
    using (var container = new Container(builder =>
           {
             builder.AddSingleton(log);
             builder.AddTransient<DisposableDependency>();
           }))
    {
      container.GetRequiredService<DisposableDependency>(); //0
      container.GetRequiredService<DisposableDependency>(); //1

      log.OpeningScope();
      using (var nested = container.CreateScope())
      {
        nested.ServiceProvider.GetRequiredService<DisposableDependency>(); //2
        nested.ServiceProvider.GetRequiredService<DisposableDependency>(); //3
        log.ClosingScope();
      } // 3.Dispose(), 2.Dispose()
      log.ClosedScope(); ;
      container.GetRequiredService<DisposableDependency>(); //4
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