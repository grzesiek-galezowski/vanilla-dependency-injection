using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.MultipleLifestylesOfInstancesTheSameClass;

//todo add descriptions
internal class MultipleLifestylesOfInstancesTheSameClass_PureDiLibrary
{
  [Test]
  public void ShouldComposeWithTwoLifestylesOfThrottledOutbox()
  {
    //GIVEN
    var composition = new Composition10();

    //WHEN
    var p1 = composition.OnDemandProcess;
    var p2 = composition.ScheduledProcess;
    var p3 = composition.EmergencyProcess;

    //THEN
    p1.ThrottledOutbox.Should().BeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p1.ThrottledOutbox);
  }
}

partial class Composition10
{
  public void Setup()
  {
    DI.Setup(nameof(Composition10))
      .Bind<ThrottledOutbox>("emergencyOutbox").As(Lifetime.Transient).To<ThrottledOutbox>()
      .Bind<ThrottledOutbox>().As(Lifetime.Singleton).To<ThrottledOutbox>()
      .RootBind<OnDemandProcess>(nameof(OnDemandProcess)).As(Lifetime.Transient).To<OnDemandProcess>()
      .RootBind<ScheduledProcess>(nameof(ScheduledProcess)).As(Lifetime.Transient).To<ScheduledProcess>()
      .RootBind<EmergencyProcess>(nameof(EmergencyProcess))
      .As(Lifetime.Transient)
      .To(context =>
      {
        context.Inject<ThrottledOutbox>("emergencyOutbox", out var value);
        return new EmergencyProcess(value);
      });
  }
}

