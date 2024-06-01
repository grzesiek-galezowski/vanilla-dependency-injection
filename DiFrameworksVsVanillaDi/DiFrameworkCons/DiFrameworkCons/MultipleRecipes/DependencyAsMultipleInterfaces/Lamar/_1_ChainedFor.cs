using Lamar;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.Lamar;

public static class _1_ChainedFor
{
  /// <summary>
  /// With Lamar, this is also relatively easy - we can use the 
  /// .For<> multiple times.
  /// Lamar doesn't necessarily "lose" with Vanilla DI because registering as
  /// multiple interfaces is very straightforward. Even though,
  /// contrary to Autofac, Lamar does not allow registering
  /// a class as all interfaces it implements. This means that
  /// Every time a new interface is extracted, the registrations
  /// need to be updated.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfaces()
  {
    //BUG: check if convention api can be used to register as
    //BUG: multiple interfaces (e.g. querying only for one type)
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddSingleton<UserOfReaderAndWriter>();
      builder.Use<Cache>().Singleton()
        .For<IReadCache>()
        .For<IWriteCache>();
    });

    container.AssertConfigurationIsValid();

    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}