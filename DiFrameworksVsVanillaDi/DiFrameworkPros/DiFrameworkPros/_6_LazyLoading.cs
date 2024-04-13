using System;
using System.Diagnostics;
using System.Threading;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkPros;

public class _6_LazyLoading
{
  /// <summary>
  /// Containers typically use "lazy creation by default" approach,
  /// which means that a dependency, even one with "singleton" lifestyle
  /// is not resolved unless somebody requests it.
  ///
  /// The good part is that your higher-level tests might run faster, because
  /// the part of the graph that the test doesn't need is never instantiated.
  /// In the example below, we have a dependency that takes 10 seconds to create
  /// (we can imagine it makes some kind of connection), but it doesn't influence
  /// the test because the test doesn't execute any path that needs this dependency.
  ///
  /// The bad part is that you don't know if your "singleton" objects will be created successfully
  /// until they are requested, which may even be 2 months after the app is deployed (ok, typically
  /// it's when the automated tests run, but not always). Also, the lazy evaluation might impact
  /// the performance of the first request that needs these singleton dependencies. People
  /// sometimes ask for a way to disable/work around this feature
  /// (e.g. https://stackoverflow.com/questions/39005861/asp-net-core-initialize-singleton-after-configuring-di)
  /// </summary>
  [Test]
  public void ShouldLazyLoadUsingAutofac()
  {
    //GIVEN
    var builder = new ContainerBuilder();

    builder.RegisterType<VerySlowDependency>().As<IVerySlowDependency>().InstancePerDependency();
    builder.RegisterType<ControllerINeed>().As<IControllerINeed>().InstancePerDependency();
    builder.RegisterType<ControllerIDoNotNeedButItNeedsSlowDependency>().As<IControllerIDoNotNeed>()
      .SingleInstance();

    //WHEN
    using var container = builder.Build();
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var controllerINeed = container.Resolve<IControllerINeed>();
    stopWatch.Stop();

    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means control never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }

  [Test]
  public void ShouldLazyLoadUsingMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();

    builder.AddSingleton<IVerySlowDependency, VerySlowDependency>();
    builder.AddTransient<IControllerINeed, ControllerINeed>();
    builder.AddTransient<IControllerIDoNotNeed, ControllerIDoNotNeedButItNeedsSlowDependency>();

    //WHEN
    using var container = builder.BuildServiceProvider();
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var controllerINeed = container.GetRequiredService<IControllerINeed>();
    stopWatch.Stop();

    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means runtime execution never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }

  /// <summary>
  /// With Vanilla DI, you have to be much more deliberate and explicit if
  /// you really want to have lazy creation (see Lazy usage in <see cref="VanillaCompositionRoot"/>).
  /// If you rely on laziness a lot, that can be a deal-breaker.
  ///
  /// I don't like the idea of "lazy by default". I consider it a value added
  /// that in case of Vanilla DI, the "singleton" dependencies are created up-front, because
  /// it gives me more confidence that when a service is started, the long-running objects
  /// are initialized correctly, without having to wait for a piece of code to "wake them up".
  /// As for the tests, I typically rely more on lower levels of testing pyramid
  /// and on test parallelization.
  /// </summary>
  [Test]
  public void ShouldLazyLoadUsingVanillaDi()
  {
    //GIVEN
    var compositionRoot = new VanillaCompositionRoot();

    //WHEN
    var stopWatch = new Stopwatch();
    var controllerINeed = compositionRoot.CreateControllerINeed();
    stopWatch.Stop();

    //THEN
    //VerySlowDependency takes 10 seconds to create.
    //Passing this assertion means control never reached this class
    stopWatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
  }

  public interface IVerySlowDependency;

  private class VerySlowDependency : IVerySlowDependency
  {
    public VerySlowDependency()
    {
      Thread.Sleep(TimeSpan.FromSeconds(10));
    }
  }

  public interface IControllerINeed;

  public class ControllerINeed : IControllerINeed;

  public interface IControllerIDoNotNeed;

  private class ControllerIDoNotNeedButItNeedsSlowDependency : IControllerIDoNotNeed
  {
    public ControllerIDoNotNeedButItNeedsSlowDependency(IVerySlowDependency dependency)
    {
    }
  }

  public class VanillaCompositionRoot
  {
    private readonly Lazy<IVerySlowDependency> _slowDependency;

    public VanillaCompositionRoot()
    {
      _slowDependency = new Lazy<IVerySlowDependency>(() => new VerySlowDependency());
    }

    public IControllerINeed CreateControllerINeed() => new ControllerINeed();
    public IControllerIDoNotNeed CreateControllerIDoNotNeed()
      => new ControllerIDoNotNeedButItNeedsSlowDependency(_slowDependency.Value);

  }
}
