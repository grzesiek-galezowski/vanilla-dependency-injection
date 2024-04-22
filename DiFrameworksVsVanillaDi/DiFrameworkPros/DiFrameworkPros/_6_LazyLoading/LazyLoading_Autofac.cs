using System.Diagnostics;
using Autofac;

namespace DiFrameworkPros._6_LazyLoading;

public static class LazyLoading_Autofac
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
  public static void ShouldLazyLoadUsingAutofac()
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
}