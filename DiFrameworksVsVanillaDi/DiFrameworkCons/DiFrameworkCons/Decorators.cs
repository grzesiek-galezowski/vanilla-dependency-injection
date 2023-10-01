using System.Linq;
using System.Security.Cryptography.X509Certificates;

//CONS:
//1. containers take time to learn (every special case -> new feature)
//2. containers hide the dependency graph
//3. containers do not yield themselves to refactoring
//4. containers give away some of the compile time checks

namespace DiFrameworkCons;

/// <summary>
/// Assembling decorators is a challenge in DI containers
/// because typically a decorator depends on the same interface it implements.
/// So if we want to avoid manual composition in this case,
/// we need to have several types registered for the same interface and then instruct
/// each registration which other implementation of the same interface should be its dependency
///
/// In the example below, answer is decorated by a traced answer which is decorated by synchronized answer.
/// </summary>
public class Decorators
{
  /// <summary>
  /// In Vanilla DI, this is pretty straightforward. It's really no different
  /// than any other composition - just create objects and pass one object
  /// as an argument of another's constructor.
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingVanillaDi()
  {
    var answer = new SynchronizedAnswer(new TracedAnswer(new Answer()), 1);

    Assert.IsInstanceOf<SynchronizedAnswer>(answer);
    Assert.IsInstanceOf<TracedAnswer>(answer.NestedAnswer);
    Assert.IsInstanceOf<Answer>(answer.NestedAnswer.NestedAnswer);
    Assert.AreEqual(1, answer.X);
  }


  /// <summary>
  /// Autofac supports special feature for decorators. On the surface
  /// it seems easy to apply (and can even set one decorator chain for multiple
  /// implementations for IAnswer if necessary), but quickly gets complicated
  /// when we want to combine it with named parameters etc.
  ///
  /// Btw good luck setting two slightly different chains of decorators
  /// without falling back to manual composition.
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingAutofac()
  {
    var builder = new ContainerBuilder();

    builder.RegisterType<Answer>().As<IAnswer>();
    //To pass parameters to decorators without invoking constructor,
    //do this:
    builder.RegisterType<SynchronizedAnswer>()
        .InstancePerDependency()
        .WithParameter(new NamedParameter("X", 1));

    //Decorators are applied in the order they are registered
    builder.RegisterDecorator<TracedAnswer, IAnswer>();
    builder.RegisterDecorator<IAnswer>((context, parameters, inner) =>
        context.Resolve<SynchronizedAnswer>(
            new NamedParameter("NestedAnswer", inner)));

    using var container = builder.Build();
    var answer = container.Resolve<IAnswer>();
    Assert.IsInstanceOf<SynchronizedAnswer>(answer);
    Assert.IsInstanceOf<TracedAnswer>(answer.NestedAnswer);
    Assert.IsInstanceOf<Answer>(answer.NestedAnswer.NestedAnswer);
    Assert.AreEqual(1, ((SynchronizedAnswer)answer).X);
  }

  /// <summary>
  /// This version pragmatically falls back to manual composition
  /// for the troublesome dependency
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingAutofacWithABitOfManualComposition()
  {
    var builder = new ContainerBuilder();

    builder.RegisterType<Answer>().As<IAnswer>();
    //Decorators are applied in the order they are registered
    builder.RegisterDecorator<TracedAnswer, IAnswer>();
    builder.RegisterDecorator<IAnswer>(
      (_, _, inner) => new SynchronizedAnswer(inner, 1));

    using var container = builder.Build();
    var answer = container.Resolve<IAnswer>();
    Assert.IsInstanceOf<SynchronizedAnswer>(answer);
    Assert.IsInstanceOf<TracedAnswer>(answer.NestedAnswer);
    Assert.IsInstanceOf<Answer>(answer.NestedAnswer.NestedAnswer);
    Assert.AreEqual(1, ((SynchronizedAnswer)answer).X);
  }

  /// <summary>
  /// MsDi doesn't support decorators (we can always fall back to manual composition of course).
  ///
  /// Two ways around this limitations are: 1) MediatR, 2) Scrutor (shown below)
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingMsDi()
  {
    var builder = new ServiceCollection();

    builder.AddTransient<IAnswer, Answer>();

    //Decorators are applied in the order they are registered
    builder.Decorate<IAnswer, TracedAnswer>();
    //With MsDi, there's currently no way around falling back to manual composition
    //(see https://github.com/dotnet/extensions/issues/2937)
    builder.Decorate<IAnswer>(a => new SynchronizedAnswer(a, 1));

    using var container = builder.BuildServiceProvider();
    var answer = container.GetRequiredService<IAnswer>();
    Assert.IsInstanceOf<SynchronizedAnswer>(answer);
    Assert.IsInstanceOf<TracedAnswer>(answer.NestedAnswer);
    Assert.IsInstanceOf<Answer>(answer.NestedAnswer.NestedAnswer);
    Assert.AreEqual(1, ((SynchronizedAnswer)answer).X);
  }

  public interface IAnswer
  {
    IAnswer NestedAnswer { get; }
  }

  public record TracedAnswer(IAnswer NestedAnswer) : IAnswer;
  public record SynchronizedAnswer(IAnswer NestedAnswer, int X) : IAnswer;
  public record Answer : IAnswer
  {
    public IAnswer NestedAnswer => null;
  }
  //TODO passing part of the chain to one object and full chain to another
  //TODO example with extracting library - easier to do with manual DI
}