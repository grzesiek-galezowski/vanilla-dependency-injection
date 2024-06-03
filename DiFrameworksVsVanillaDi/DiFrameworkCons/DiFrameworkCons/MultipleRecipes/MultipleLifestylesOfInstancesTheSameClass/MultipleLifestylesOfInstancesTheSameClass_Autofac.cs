namespace DiFrameworkCons.MultipleRecipes.MultipleLifestylesOfInstancesTheSameClass;

public static class MultipleLifestylesOfInstancesTheSameClass_Autofac
{
  [Test]
  public static void ShouldComposeWithTwoLifestylesOfThrottledOutbox()
  {
    //GIVEN
    var builder = new ContainerBuilder();

    builder.RegisterType<ThrottledOutbox>()
      .Keyed<ThrottledOutbox>("shared")
      .SingleInstance();
    builder.RegisterType<ThrottledOutbox>().InstancePerLifetimeScope();
    builder.RegisterType<OnDemandProcess>().WithParameter(
      (info, _) => info.Position == 0,
      (_, context) => context.ResolveKeyed<ThrottledOutbox>("shared"));
    builder.RegisterType<ScheduledProcess>()
      .WithParameter(
        (info, _) => info.Position == 0,
        (_, context) => context.ResolveKeyed<ThrottledOutbox>("shared"));
    builder.RegisterType<EmergencyProcess>().InstancePerLifetimeScope();

    using var container = builder.Build();

    //WHEN
    var p1 = container.Resolve<OnDemandProcess>();
    var p2 = container.Resolve<ScheduledProcess>();
    var p3 = container.Resolve<EmergencyProcess>();

    //THEN
    p1.ThrottledOutbox.Should().BeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p1.ThrottledOutbox);
  }
}