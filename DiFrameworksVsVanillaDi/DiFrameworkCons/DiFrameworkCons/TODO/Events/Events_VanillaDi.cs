namespace DiFrameworkCons.TODO.Events;

public class Events_VanillaDi
{
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
    observer.LastReceived.Should().Be(dependency1.InstanceId);

    dependency2.DoSomething();
    observer.LastReceived.Should().Be(dependency2.InstanceId);

    dependency3.DoSomething();
    observer.LastReceived.Should().Be(dependency3.InstanceId);
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
    observer.LastReceived.Should().Be(dependency1.InstanceId);

    dependency2.DoSomething();
    observer.LastReceived.Should().Be(dependency2.InstanceId);

    dependency3.DoSomething();
    observer.LastReceived.Should().Be(dependency3.InstanceId);
  }
}

