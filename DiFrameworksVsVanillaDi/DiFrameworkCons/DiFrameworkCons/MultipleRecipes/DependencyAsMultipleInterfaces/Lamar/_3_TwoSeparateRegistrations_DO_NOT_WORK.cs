using Lamar;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.Lamar;

public static class _3_TwoSeparateRegistrations_DO_NOT_WORK
{
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