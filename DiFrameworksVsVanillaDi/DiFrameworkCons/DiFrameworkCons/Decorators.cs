using FluentAssertions;

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
  /// from any other composition - just create objects and pass one object
  /// as an argument of another's constructor.
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingVanillaDi()
  {
    var answer = new SynchronizedAnswer(new TracedAnswer(new Answer()), 1);

    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    answer.X.Should().Be(1);
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
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
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
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }

  /// <summary>
  /// MsDi doesn't support decorators (we can always fall back to lambda registration of course).
  ///
  /// Two ways around these limitations are: 1) MediatR, 2) Scrutor (shown below)
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingMsDiWithScrutor()
  {
    var builder = new ServiceCollection();

    builder.AddTransient<IAnswer, Answer>();

    //Decorators are applied in the order they are registered
    builder.Decorate<IAnswer, TracedAnswer>();
    //With MsDi, there's currently no way around falling back lambda
    //(see https://github.com/dotnet/extensions/issues/2937)
    builder.Decorate<IAnswer>((a, ctx) => ActivatorUtilities.CreateInstance<SynchronizedAnswer>(ctx, a, 1));

    using var container = builder.BuildServiceProvider();
    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }

  public interface IAnswer
  {
    IAnswer NestedAnswer { get; }
  }

  public record TracedAnswer(IAnswer NestedAnswer) : IAnswer;
  public record SynchronizedAnswer(IAnswer NestedAnswer, int X) : IAnswer;
  public record Answer : IAnswer
  {
    public IAnswer NestedAnswer => null!;
  }
}