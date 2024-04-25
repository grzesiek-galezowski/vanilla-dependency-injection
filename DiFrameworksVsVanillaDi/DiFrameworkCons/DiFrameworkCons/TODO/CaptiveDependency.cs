namespace DiFrameworkCons.TODO;

/// <summary>
/// TODO revise this. Seemann & Van Deursen say captive dependencies
/// are possible with Pure DI, just much harder to pull off
/// </summary>
public class CaptiveDependency
{
  /// <summary>
  /// Captive dependency is described as a dependency from a wider scope
  /// obtaining an object from registration made with narrower scope in mind.
  /// One example is when a singleton dependency obtains a reference to scoped dependency.
  /// This can be problematic because the scoped dependency might be e.g. non-thread-safe
  /// but accessed from multiple threads that use the singleton (e.g. from HTTP request
  /// handlers). Also, as illustrated further, when a singleton is resolved
  /// within a request scope and it captivates a scoped instance, the scoped instance
  /// is not disposed of until the singleton dies.
  ///
  /// ---This problem is exclusive to DI Containers. Some of them have special runtime checks
  /// to detect this situation. In Vanilla DI it's possible to pass unsafe dependency,
  /// but that can only happen as a conscious decision, not a configuration mistake.---
  ///
  /// Autofac doesn't have strict validity checks built-in.
  /// See https://autofac.readthedocs.io/en/stable/lifetime/captive-dependencies.html
  /// See https://autofac.readthedocs.io/en/latest/faq/container-analysis.html
  /// </summary>
  [Test]
  public void ShouldShowCaptiveDependencyIssueUsingAutofac()
  {
    var i = 0;
    //GIVEN
    var builder = new ContainerBuilder();
    builder.Register(ctr => "Lol" + i++).InstancePerLifetimeScope();
    builder.RegisterType<Captor>().SingleInstance();
    builder.RegisterType<Transitive>().InstancePerDependency();
    using var container = builder.Build();

    //WHEN
    using (var scope = container.BeginLifetimeScope())
    {
      var text1 = scope.Resolve<string>();
      var text2 = scope.Resolve<string>();
      //let's say that the singleton is first resolved during the first request
      var captor1 = scope.Resolve<Captor>();
      var captor2 = scope.Resolve<Captor>();
      //THEN
      text1.Should().Be("Lol0");
      text2.Should().Be("Lol0");

      //captor still holds "Lol1"
      captor1.T.Str.Should().Be("Lol1");
      captor2.T.Str.Should().Be("Lol1");
    }

    using (var scope = container.BeginLifetimeScope())
    {
      var text1 = scope.Resolve<string>();
      var text2 = scope.Resolve<string>();
      var captor1 = scope.Resolve<Captor>();
      var captor2 = scope.Resolve<Captor>();
      //THEN
      text1.Should().Be("Lol2");
      text2.Should().Be("Lol2");

      //captor still holds "Lol1"
      captor1.T.Str.Should().Be("Lol1");
      captor2.T.Str.Should().Be("Lol1");
    }

    using (var scope = container.BeginLifetimeScope())
    {
      var text1 = scope.Resolve<string>();
      var text2 = scope.Resolve<string>();
      var captor1 = scope.Resolve<Captor>();
      var captor2 = scope.Resolve<Captor>();
      //THEN
      text1.Should().Be("Lol3");
      text2.Should().Be("Lol3");

      //captor still holds "Lol1"
      captor1.T.Str.Should().Be("Lol1");
      captor2.T.Str.Should().Be("Lol1");
    }
  }

  /// <summary>
  /// When applied standalone, the Microsoft.DependencyInjection container
  /// doesn't by default detect captive dependencies.
  /// </summary>
  [Test]
  public void ShouldShowCaptiveDependencyIssueUsingMsDi__WithoutScopeValidation()
  {
    var i = 0;
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddScoped(ctr => "Lol" + i++);
    builder.AddSingleton<Captor>();
    builder.AddTransient<Transitive>();
    using var container = builder.BuildServiceProvider();

    //WHEN
    using (var scope = container.CreateScope())
    {
      var text1 = scope.ServiceProvider.GetRequiredService<string>();
      var text2 = scope.ServiceProvider.GetRequiredService<string>();
      //let's say that the singleton is first resolved during the first request
      var captor1 = scope.ServiceProvider.GetRequiredService<Captor>();
      var captor2 = scope.ServiceProvider.GetRequiredService<Captor>();
      //THEN
      text1.Should().Be("Lol0");
      text2.Should().Be("Lol0");

      //captor still holds "Lol1"
      captor1.T.Str.Should().Be("Lol1");
      captor2.T.Str.Should().Be("Lol1");
    }

    using (var scope = container.CreateScope())
    {
      var text1 = scope.ServiceProvider.GetRequiredService<string>();
      var text2 = scope.ServiceProvider.GetRequiredService<string>();
      var captor1 = scope.ServiceProvider.GetRequiredService<Captor>();
      var captor2 = scope.ServiceProvider.GetRequiredService<Captor>();
      //THEN
      text1.Should().Be("Lol2");
      text2.Should().Be("Lol2");
      //captor still holds "Lol1"
      captor1.T.Str.Should().Be("Lol1");
      captor2.T.Str.Should().Be("Lol1");
    }

    using (var scope = container.CreateScope())
    {
      var text1 = scope.ServiceProvider.GetRequiredService<string>();
      var text2 = scope.ServiceProvider.GetRequiredService<string>();
      var captor1 = scope.ServiceProvider.GetRequiredService<Captor>();
      var captor2 = scope.ServiceProvider.GetRequiredService<Captor>();
      //THEN
      text1.Should().Be("Lol3");
      text2.Should().Be("Lol3");

      //captor still holds "Lol1"
      captor1.T.Str.Should().Be("Lol1");
      captor2.T.Str.Should().Be("Lol1");
    }
  }

  /// <summary>
  /// However, the way it's configured in ASP.Net Core, it has the scope validation
  /// enabled. Thus, it will detect captive dependencies and throw an exception at runtime.
  ///
  /// This still isn't ideal, because:
  /// 1) The error is still in runtime
  /// 2) The error message (at least in .net 7.0) skips all transients standing between
  ///    the captor and captive, making it harder to debug and fix.
  /// </summary>
  [Test]
  public void ShouldShowCaptiveDependencyDetectionUsingMsDi__WithScopeValidation()
  {
    var i = 0;
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddScoped(ctr => "Lol" + i++);
    builder.AddSingleton<Captor>();
    builder.AddTransient<Transitive>();
    using var container = builder.BuildServiceProvider(true); //scope validation enabled

    //WHEN
    Invoking(container.GetRequiredService<Captor>)
      .Should().Throw<InvalidOperationException>()
      //There is no information about the transitive transient dependency:
      .WithMessage(
        "Cannot consume scoped service 'System.String' " +
        "from singleton 'DiFrameworkCons.TODO.CaptiveDependency+Captor'.");
  }

  /// <summary>
  /// The diagnostics become even worse when we use lambda registrations.
  /// </summary>
  [Test]
  public void ShouldShowCaptiveDependencyDetectionUsingMsDi__WithScopeValidationAndLambdas()
  {
    var i = 0;
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddScoped(ctr => "Lol" + i++);
    builder.AddSingleton<Captor>(ctx => new Captor(ctx.GetRequiredService<Transitive>()));
    builder.AddTransient<Transitive>(ctx => new Transitive(ctx.GetRequiredService<string>()));
    using var container = builder.BuildServiceProvider(true); //scope validation enabled

    //WHEN
    Invoking(container.GetRequiredService<Captor>)
      .Should().Throw<InvalidOperationException>()
      .WithMessage("Cannot resolve scoped service 'System.String' from root provider.");
  }

  [Test]
  public void ShouldDemonstrateNotDisposingScopedDependenciesHeldBySingletonUsingAutofac()
  {
    var serviceCollection = new ContainerBuilder();
    serviceCollection.RegisterType<Scoped>().InstancePerLifetimeScope();
    serviceCollection.RegisterType<Singleton>().SingleInstance();

    var provider = serviceCollection.Build();
    using (var scope = provider.BeginLifetimeScope())
    {
      var requiredService = scope.Resolve<Singleton>();
    }

    Invoking(provider.Dispose).Should().Throw<Exception>();
  }

  [Test]
  public void ShouldDemonstrateNotDisposingScopedDependenciesHeldBySingletonUsingMsDi()
  {
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddScoped<Scoped>();
    serviceCollection.AddSingleton<Singleton>();

    var provider = serviceCollection.BuildServiceProvider();
    using (var scope = provider.CreateScope())
    {
      var requiredService = scope.ServiceProvider.GetRequiredService<Singleton>();
    }

    Invoking(provider.Dispose).Should().Throw<Exception>();
  }

  public class Scoped : IDisposable
  {
    public void Dispose()
    {
      throw new Exception();
    }
  }

  public class Singleton
  {
    public Singleton(Scoped scoped)
    {
    }
  }

  [Test]
  public void ShouldShowCaptiveDependencyDetectionUsingVanillaDependencyInjection()
  {
    //// In Vanilla DI, you cannot pass a shorter-lived variable into a longer-lived one.
    ////GIVEN
    //var captor = new Captor(new Transitive(strInScope1));
    //{
    //  var strInScope1 = "Lol2";
    //}

    // You can still "capture" a dependency, but it's your explicit decision,
    // not a configuration mistake. You have to create the "captured" dependency
    // by yourself in the wider scope.
    // In other words, captive dependency is like:
    // "I know this should always be scoped, but I misconfigured the container"
    // With vanilla DI, we can only have:
    // "Oh, I didn't know this should always be scoped"
    var captor = new Captor(new Transitive("Lol1"));
    {
      var strInScope1 = "Lol2";
    }
    //you could argue that the container (with scope validation enabled)
    //at least protects from doing this at runtime (while vanilla DI just allows it),
    //but that's not true. With DI container, you can also make a conscious decision
    //to pass a special instance to a singleton:
    var i = 0;
    var builder = new ServiceCollection();
    builder.AddScoped(ctr => "Lol" + i++);
    builder.AddSingleton<Captor>(ctx => new Captor(new Transitive("Lol1")));
    builder.AddTransient<Transitive>();
    using var container = builder.BuildServiceProvider(true); //scope validation enabled

    //WHEN
    Invoking(() =>
    {
        //this doesn't throw exception because we handwired a special instance of normally
        //scoped dependency to the singleton Captor.
        var captor1 = container.GetRequiredService<Captor>();
    }).Should().NotThrow();

  }

  public record Captor(Transitive T);
  public record Transitive(string Str);


}
