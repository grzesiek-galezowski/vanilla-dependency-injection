namespace DiFrameworkCons.Events;

public static class Events_MsDi
{
  /// <summary>
  /// In MsDi, one way we can use autowiring and still be able to register event handlers
  /// is to use the double registration workaround - register the dependency twice - one time
  /// as its concrete type and the other as its interface. Then we need to make sure all other
  /// classes only require the interface.
  /// </summary>
  [Test]
  public static void ShouldShowHandMadeHandlingOfEventsUsingMsDi()
  {
    //GIVEN
    var services = new ServiceCollection();
    services.AddSingleton<MyObserver>();
    services.AddTransient<MyDependency>();
    services.AddTransient<IMyDependency>(sp =>
    {
      var observer = sp.GetRequiredService<MyObserver>();
      var dependency = sp.GetRequiredService<MyDependency>();
      dependency.SomeKindOfEvent += observer.Notify;
      return dependency;
    });

    var serviceProvider = services.BuildServiceProvider();

    //WHEN
    var observer = serviceProvider.GetRequiredService<MyObserver>();
    var dependency1 = serviceProvider.GetRequiredService<IMyDependency>();
    var dependency2 = serviceProvider.GetRequiredService<IMyDependency>();
    var dependency3 = serviceProvider.GetRequiredService<IMyDependency>();

    //THEN
    dependency1.DoSomething();
    observer.LastReceived.Should().Be(dependency1.InstanceId);

    dependency2.DoSomething();
    observer.LastReceived.Should().Be(dependency2.InstanceId);

    dependency3.DoSomething();
    observer.LastReceived.Should().Be(dependency3.InstanceId);
  }
}