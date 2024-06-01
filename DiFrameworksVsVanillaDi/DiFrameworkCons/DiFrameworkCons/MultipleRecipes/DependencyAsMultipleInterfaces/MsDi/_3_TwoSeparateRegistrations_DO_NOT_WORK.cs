namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.MsDi;

public static class _3_TwoSeparateRegistrations_DO_NOT_WORK
{
  /// <summary>
  /// This is a test adapted from
  /// https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
  /// only to show that in MsDi, registering the same implementation twice
  /// as singleton, each time as a different interface is not equal to
  /// resolving the same instance from each registration
  /// </summary>
  [Test]
  public static void WhenRegisteredAsSeparateSingleton_InstancesAreNotTheSame()
  {
    //GIVEN
    var builder = new ServiceCollection();

    builder.AddSingleton<IReadCache, Cache>();
    builder.AddSingleton<IWriteCache, Cache>();
    builder.AddSingleton<UserOfReaderAndWriter>();

    //WHEN
    using var container = builder.BuildServiceProvider();
    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().NotBeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().NotBe(cacheUser.ReadCache.Number);
  }
}