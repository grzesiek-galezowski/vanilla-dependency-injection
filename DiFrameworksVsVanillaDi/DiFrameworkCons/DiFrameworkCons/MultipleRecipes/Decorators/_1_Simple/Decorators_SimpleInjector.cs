using System.Linq.Expressions;
using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple;

public static class Decorators_SimpleInjector
{
  /// <summary>
  /// SimpleInjector supports special feature for decorators,
  /// but it doesn't allow passing literals. A fallback approach
  /// is to use conditional registrations.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsUsingAutofac()
  {
    using var container = new Container();
    container.Register<TracedAnswer>();
    container.Register<SynchronizedAnswer>(
      () => ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
        container,
        container.GetRequiredService<TracedAnswer>(), 1));
    container.RegisterConditional
      <IAnswer, Answer>(WhenRequestedBy<TracedAnswer>());
    container.RegisterConditional
      <IAnswer, TracedAnswer>(WhenRequestedBy<SynchronizedAnswer>());
    container.RegisterConditional<IAnswer>(
      new TransientRegistration(
        container,
        typeof(SynchronizedAnswer),
        container.GetInstance<SynchronizedAnswer>),
      context => !context.Handled);

    var answer = container.GetInstance<IAnswer>();

    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }

  private static Predicate<PredicateContext> WhenRequestedBy<T>()
  {
    return x => x.HasConsumer && x.Consumer.ImplementationType == typeof(T);
  }
}

// copied from SimpleInjector code
sealed class TransientRegistration : Registration
{
  public TransientRegistration(
    Container container,
    Type implementationType,
    Func<object>? creator = null)
    : base(Lifestyle.Transient, container, implementationType, creator)
  {
  }

  public override Expression BuildExpression() =>
    BuildTransientExpression();
}