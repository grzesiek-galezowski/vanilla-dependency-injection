using Pure.DI;

namespace DiFrameworkPros._5_DependencyReplacement;

public class DependencyReplacement_PureDiLibrary
{
  [Test]
  public void ShouldBeAbleToOverrideArbitraryDependency()
  {
    var composition = new TestComposition20();
    composition.SomeLogic.Execute();

    composition.TroublesomeDependencyMock.Received(1).DoSomething();
  }
}

public partial class Composition20
{
  public void Setup()
  {
    DI.Setup(nameof(Composition20))
      .RootBind<ISomeLogic>("SomeLogic").As(Lifetime.Singleton).To<SomeLogic>()
      .Bind<TroublesomeDependency>().As(Lifetime.Singleton).To<TroublesomeDependency>()
      .Bind<ITroublesomeDependency>().As(Lifetime.Singleton).To(context =>
      {
        //Need to bind to lambda. Binding straight to type didn't work.
        context.Inject<TroublesomeDependency>(out var troublesomeDependency);
        return troublesomeDependency;
      });
  }
}

public partial class TestComposition20
{
  public void Setup()
  {
    DI.Setup(nameof(TestComposition20))
      .DependsOn(nameof(Composition20)) //need to do this to be able to override
      .RootBind<ITroublesomeDependency>("TroublesomeDependencyMock")
      .As(Lifetime.Singleton)
      .To(context => Substitute.For<ITroublesomeDependency>());
  }
}
