using Lamar;
using Scrutor;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.Lamar;

public static class DependencyAsMultipleInterfaces_Lamar
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

  /// <summary>
  /// Alternatively, convention-based API can be used to register
  /// just a single type.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesWithScrutor()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddSingleton<UserOfReaderAndWriter>();
      builder.Scan(scan => scan
        .FromAssemblyOf<Cache>()
        .AddClasses(classes => classes.Where(c => c == typeof(Cache)))
        .UsingRegistrationStrategy(RegistrationStrategy.Throw)
        .AsSelfWithInterfaces()
        .WithSingletonLifetime());
    });

    container.AssertConfigurationIsValid();

    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }

  /// <summary>
  /// This is a test adapted from
  /// https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
  /// only to show that in Lamar, registering the same implementation twice
  /// as singleton, each time as a different interface is not equal to
  /// registering one singleton as multiple interfaces
  /// </summary>
  [Test]
  public static void WhenRegisteredAsSeparateSingleton_InstancesAreNotTheSame()
  {
    //GIVEN
    var container = new Container(builder =>
    {
      builder.For<IReadCache>().Use<Cache>().Singleton();
      builder.For<IWriteCache>().Use<Cache>().Singleton();
      builder.AddSingleton<UserOfReaderAndWriter>();
    });

    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().NotBeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().NotBe(cacheUser.ReadCache.Number);
  }
}