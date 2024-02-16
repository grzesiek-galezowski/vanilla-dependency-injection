namespace DiFrameworkCons;

/// <summary>
/// Events are not a very often used feature nowadays,
/// but it's an example of something we sometimes need to do in addition
/// to just creating an object using its constructor.
/// </summary>
public class EventsAndOnActivated
{
  /// <summary>
  /// Autofac provides a special feature called "OnActivating" (as well as other lifetime callbacks)
  /// which allows doing something after an object is created while still getting the
  /// benefits of autowiring.
  ///
  /// We can use OnRelease callback to unplug an event if it's necessary
  /// </summary>
  [Test]
  public void ShouldShowHandMadeHandlingOfEventsUsingAutofac()
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
    Assert.AreEqual(dependency1.InstanceId, observer.LastReceived);

    dependency2.DoSomething();
    Assert.AreEqual(dependency2.InstanceId, observer.LastReceived);

    dependency3.DoSomething();
    Assert.AreEqual(dependency3.InstanceId, observer.LastReceived);
  }

  /// <summary>
  /// In MsDi, one way we can use autowiring and still be able to register event handlers
  /// is to use the double registration workaround - register the dependency twice - one time
  /// as its concrete type and the other as its interface. Then we need to make sure all other
  /// classes only require the interface.
  /// </summary>
  [Test]
  public void ShouldShowHandMadeHandlingOfEventsUsingMicrosoftDependencyInjection()
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
    Assert.AreEqual(dependency1.InstanceId, observer.LastReceived);

    dependency2.DoSomething();
    Assert.AreEqual(dependency2.InstanceId, observer.LastReceived);

    dependency3.DoSomething();
    Assert.AreEqual(dependency3.InstanceId, observer.LastReceived);
  }

  /// <summary>
  /// In vanilla DI everything is simple and easy - wherever we create an object,
  /// we can perform additional operations on it.
  /// One difference is that here, we need to register the observer
  /// every time we create an object (we are not establishing a convention)
  /// But from my experience it's a good thing because I rarely want to perform
  /// the same registrations for every created object. If I do, I can create
  /// a factory method (see the next example)  
  /// </summary>
  [Test]
  public void ShouldShowHandMadeHandlingOfEventsWithVanillaDi()
  {
    //GIVEN
    var observer = new MyObserver();
    var dependency1 = new MyDependency();
    var dependency2 = new MyDependency();
    var dependency3 = new MyDependency();

    //WHEN
    dependency1.SomeKindOfEvent += observer.Notify;
    dependency2.SomeKindOfEvent += observer.Notify;
    dependency3.SomeKindOfEvent += observer.Notify;

    //THEN
    dependency1.DoSomething();
    Assert.AreEqual(dependency1.InstanceId, observer.LastReceived);

    dependency2.DoSomething();
    Assert.AreEqual(dependency2.InstanceId, observer.LastReceived);

    dependency3.DoSomething();
    Assert.AreEqual(dependency3.InstanceId, observer.LastReceived);
  }

  /// <summary>
  /// This example shows how to register an event every time an object is created.
  ///
  /// Very rarely useful IME.
  /// </summary>
  [Test]
  public void ShouldShowHandMadeHandlingOfEventsWithVanillaDiAndFactoryMethod()
  {
    //GIVEN
    var observer = new MyObserver();

    MyDependency GetMyDependency()
    {
      var dependency = new MyDependency();
      dependency.SomeKindOfEvent += observer.Notify;
      return dependency;
    }

    //WHEN
    var dependency1 = GetMyDependency();
    var dependency2 = GetMyDependency();
    var dependency3 = GetMyDependency();

    //THEN
    dependency1.DoSomething();
    Assert.AreEqual(dependency1.InstanceId, observer.LastReceived);

    dependency2.DoSomething();
    Assert.AreEqual(dependency2.InstanceId, observer.LastReceived);

    dependency3.DoSomething();
    Assert.AreEqual(dependency3.InstanceId, observer.LastReceived);
  }
}

public class MyObserver
{
  public void Notify(int value)
  {
    LastReceived = value;
  }

  public int LastReceived { get; set; }
}

public interface IMyDependency
{
  void DoSomething();
  int InstanceId { get; }
}

public class MyDependency : IMyDependency
{
  private static int _lastInstanceId = 0;
  public int InstanceId { get; }

  public MyDependency()
  {
    InstanceId = _lastInstanceId++;
  }

  public event Action<int> SomeKindOfEvent;

  public void DoSomething()
  {
    SomeKindOfEvent?.Invoke(InstanceId);
  }
}
