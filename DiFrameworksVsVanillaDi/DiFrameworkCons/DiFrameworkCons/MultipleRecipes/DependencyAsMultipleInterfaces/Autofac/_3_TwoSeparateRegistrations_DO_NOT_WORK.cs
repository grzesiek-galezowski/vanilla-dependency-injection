namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces.Autofac;

public static class _3_TwoSeparateRegistrations_DO_NOT_WORK
{
  /// <summary>
  /// This is a test adapted from
  /// https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
  /// only to show that in Autofac, registering the same implementation twice
  /// as singleton, each time as a different interface is not equal to
  /// registering one singleton as multiple interfaces
  /// </summary>
  [Test]
  public static void WhenRegisteredAsSeparateSingleton_InstancesAreNotTheSame()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<UserOfReaderAndWriter>().SingleInstance();
    containerBuilder.RegisterType<Cache>()
      .SingleInstance().As<IReadCache>();
    containerBuilder.RegisterType<Cache>()
      .SingleInstance().As<IWriteCache>();

    using var container = containerBuilder.Build();
    //WHEN
    var cacheUser = container.Resolve<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().NotBeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().NotBe(cacheUser.ReadCache.Number);
  }
}