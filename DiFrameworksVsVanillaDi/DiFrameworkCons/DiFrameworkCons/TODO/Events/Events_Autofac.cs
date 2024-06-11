namespace DiFrameworkCons.TODO.Events;

public static class Events_Autofac
{
  /// <summary>
  /// Autofac provides a special feature called "OnActivating" (as well as other lifetime callbacks)
  /// which allows doing something after an object is created while still getting the
  /// benefits of autowiring.
  ///
  /// We can use OnRelease callback to unplug an event if it's necessary
  /// </summary>
  [Test]
  public static void ShouldShowHandMadeHandlingOfEvents()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<MyObserver>().SingleInstance();
    builder.RegisterType<MyDependency>().InstancePerDependency()
      .OnActivated(args =>
        args.Instance.SomeKindOfEvent += args.Context.Resolve<MyObserver>().Notify);
    using var container = builder.Build();

    //WHEN
    var observer = container.Resolve<MyObserver>();
    var dependency1 = container.Resolve<MyDependency>();
    var dependency2 = container.Resolve<MyDependency>();
    var dependency3 = container.Resolve<MyDependency>();

    //THEN
    dependency1.DoSomething();
    observer.LastReceived.Should().Be(dependency1.InstanceId);

    dependency2.DoSomething();
    observer.LastReceived.Should().Be(dependency2.InstanceId);

    dependency3.DoSomething();
    observer.LastReceived.Should().Be(dependency3.InstanceId);
  }
}