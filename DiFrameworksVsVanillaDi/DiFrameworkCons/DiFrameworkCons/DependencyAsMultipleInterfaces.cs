namespace DiFrameworkCons;

/// <summary>
/// This example shows using the same object as multiple interfaces.
/// </summary>
public class DependencyAsMultipleInterfaces
{
  /// <summary>
  /// With Vanilla DI, this is business as usual - we can create an object,
  /// assign it to a variable and pass everywhere where an object of compatible
  /// type is expected.
  /// </summary>
  [Test]
  public void ShouldUseOneInstanceForDifferentInterfacesUsingVanillaDi()
  {  
    //GIVEN
    var cache = new Cache();

    //WHEN
    var cacheUser = new UserOfReaderAndWriter(cache, cache);

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }

  /// <summary>
  /// With Autofac, this is also relatively easy - we can use either the .As()
  /// multiple times or just use the .AsImplementedInterfaces().
  /// Autofac doesn't "lose" with Vanilla DI because registering as
  /// multiple interfaces is very straightforward.
  /// </summary>
  [Test]
  public void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesUsingAutofac()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<UserOfReaderAndWriter>().SingleInstance();
    containerBuilder.RegisterType<Cache>()
      .SingleInstance()
      .AsImplementedInterfaces();

    using var container = containerBuilder.Build();
    //WHEN
    var cacheUser = container.Resolve<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }

  /// <summary>
  /// In MsDi, it's worse as we need to explicitly register each interface using lambdas,
  /// which are not subject to container validation. Hence, this approach is slightly more
  /// error-prone than Vanilla DI.
  /// </summary>
  [Test]
  public void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesUsingMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<UserOfReaderAndWriter>();
    builder.AddSingleton<Cache>();
    builder.AddSingleton<IReadCache>(c => c.GetRequiredService<Cache>());
    builder.AddSingleton<IWriteCache>(c => c.GetRequiredService<Cache>());

    using var container = builder.BuildServiceProvider(new ServiceProviderOptions()
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });
    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }

  /// <summary>
  /// This is a test adapted from
  /// https://andrewlock.net/how-to-register-a-service-with-multiple-interfaces-for-in-asp-net-core-di/
  /// only to show that in MsDi, registering the same implementation twice
  /// as singleton, each time as a different interface is not equal to
  /// resolving the same instance from each registration
  /// </summary>
  [Test]
  public void WhenRegisteredAsSeparateSingleton_InstancesAreNotTheSame()
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

public record UserOfReaderAndWriter(IReadCache ReadCache, IWriteCache WriteCache);

public interface IWriteCache
{
  public int Number { get; }
}

public interface IReadCache
{
  public int Number { get; }
}

public record Cache : IWriteCache, IReadCache
{
  public Cache()
  {
    Number = _num++;
  }

  private static int _num = 1;
  public int Number { get; } = _num;
};