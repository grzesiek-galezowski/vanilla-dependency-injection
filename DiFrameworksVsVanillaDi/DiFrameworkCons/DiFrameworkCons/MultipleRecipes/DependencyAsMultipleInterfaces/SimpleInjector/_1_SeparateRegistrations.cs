using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.SimpleInjector;

public static class _1_SeparateRegistrations
{
  /// <summary>
  /// With SimpleInjector, this is relatively easy - we can register
  /// the same type several times, each time as a different interface.
  /// SimpleInjector doesn't necessarily "lose" with Vanilla DI because
  /// registering as multiple interfaces is very straightforward. Even though,
  /// contrary to Autofac, SimpleInjector does not allow registering
  /// a class as all interfaces it implements. This means that
  /// Every time a new interface is extracted, the registrations
  /// need to be updated.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfaces()
  {
    //GIVEN
    using var container = new Container();

    container.RegisterSingleton<UserOfReaderAndWriter>();
    container.RegisterSingleton<IReadCache, Cache>();
    container.RegisterSingleton<IWriteCache, Cache>();

    container.Verify();

    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}