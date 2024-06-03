using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.MultipleLifestylesOfInstancesTheSameClass;

internal class MultipleLifestylesOfInstancesTheSameClass_SimpleInjector
{
  /// <summary>
  /// SimpleInjector has issues with two conditional registrations
  /// of the same resolved type, even when they are of different lifestyles.
  /// Hence, the CreateRegistration with lambda is used to work around it.
  ///
  /// This could be worked around, e.g. by making two distinct interfaces
  /// both implemented by ThrottledOutbox, which I think is what the
  /// author of the container would recommend.
  /// </summary>
  [Test]
  public void ShouldComposeWithTwoLifestylesOfThrottledOutbox()
  {
    using var container = new Container();

    container.RegisterConditional<ThrottledOutbox>(
        Lifestyle.Transient.CreateRegistration(
          () => ActivatorUtilities.CreateInstance<ThrottledOutbox>(
            container), container),
        context => context.Consumer.ImplementationType == typeof(EmergencyProcess));
    container.RegisterConditional<ThrottledOutbox, ThrottledOutbox>(
      Lifestyle.Singleton,
      context => !context.Handled);
    container.Register<OnDemandProcess>();
    container.Register<ScheduledProcess>();
    container.Register<EmergencyProcess>();

    //WHEN
    var p1 = container.GetRequiredService<OnDemandProcess>();
    var p2 = container.GetRequiredService<ScheduledProcess>();
    var p3 = container.GetRequiredService<EmergencyProcess>();

    //THEN
    p1.ThrottledOutbox.Should().BeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p1.ThrottledOutbox);
  }
}