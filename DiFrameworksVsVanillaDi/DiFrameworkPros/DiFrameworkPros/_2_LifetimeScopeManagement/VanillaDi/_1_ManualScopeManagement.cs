namespace DiFrameworkPros._2_LifetimeScopeManagement.VanillaDi;

public static class _1_ManualScopeManagement
{
  /// <summary>
  /// In case of manual DI, dependencies have to be disposed by hand.
  /// Also, exception thrown from a Dispose() prevents further Dispose() calls.
  /// Fortunately I don't typically encounter many disposable objects
  /// that need to go beyond single method scope (e.g. that cannot be handled with
  /// 'using()' or 'using var', however...
  /// </summary>
  [Test]
  public static void ShouldDisposeOfCreatedDependenciesUsingVanillaDependencyInjection()
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
}