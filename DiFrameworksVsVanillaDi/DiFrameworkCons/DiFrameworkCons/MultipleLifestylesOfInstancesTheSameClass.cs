namespace DiFrameworkCons;

//todo add descriptions
internal class MultipleLifestylesOfInstancesTheSameClass
{
  [Test]
  public void ShouldComposeWithTwoLifestylesOfThrottledOutbox()
  {
    //GIVEN
    var o = new ThrottledOutbox();
    var p1 = new OnDemandProcess(o);
    var p2 = new ScheduledProcess(o);
    var p3 = new EmergencyProcess(
      new ThrottledOutbox()
    );

    //THEN
    p1.ThrottledOutbox.Should().BeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p1.ThrottledOutbox);
  }

  [Test]
  public void ShouldComposeWithTwoLifestylesOfThrottledOutbox_UsingMsDi()
  {
    //GIVEN
    var services = new ServiceCollection();

    services.AddKeyedSingleton<ThrottledOutbox>("shared");
    services.AddTransient<ThrottledOutbox>();
    services.AddTransient(ctx =>
      ActivatorUtilities.CreateInstance<OnDemandProcess>(
        ctx,
        ctx.GetRequiredKeyedService<ThrottledOutbox>("shared")));
    services.AddTransient(ctx =>
      ActivatorUtilities.CreateInstance<ScheduledProcess>(
        ctx,
        ctx.GetRequiredKeyedService<ThrottledOutbox>("shared")));
    services.AddTransient<EmergencyProcess>();

    using var container = services.BuildServiceProvider(new ServiceProviderOptions
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });

    //WHEN
    var p1 = container.GetRequiredService<OnDemandProcess>();
    var p2 = container.GetRequiredService<ScheduledProcess>();
    var p3 = container.GetRequiredService<EmergencyProcess>();


    //THEN
    p1.ThrottledOutbox.Should().BeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p1.ThrottledOutbox);
  }

  [Test]
  public void ShouldComposeWithTwoLifestylesOfThrottledOutbox_UsingAutofac()
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

internal record EmergencyProcess(ThrottledOutbox ThrottledOutbox);
internal record ScheduledProcess(ThrottledOutbox ThrottledOutbox);
internal record OnDemandProcess(ThrottledOutbox ThrottledOutbox);

internal class ThrottledOutbox;