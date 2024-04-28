namespace DiFrameworkCons.MultipleLifestylesOfInstancesTheSameClass;

public static class MultipleLifestylesOfInstancesTheSameClass_MsDi
{
  [Test]
  public static void ShouldComposeWithTwoLifestylesOfThrottledOutbox_UsingMsDi()
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
}