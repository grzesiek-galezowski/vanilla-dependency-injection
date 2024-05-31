using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.SimpleInjector;

public static class _2_ConcreteRegistrationsAndLambdas
{
  /// <summary>
  /// Alternatively, one can use registrations of concrete types
  /// together with lambda registrations.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsLambdaRegistrations()
  {
    var container = new Container();

    container.Register<Answer>();
    container.Register<TracedAnswer>(() =>
      ActivatorUtilities.CreateInstance<TracedAnswer>(
        container, container.GetRequiredService<Answer>()));
    container.Register<IAnswer>(() =>
      ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
        container, container.GetRequiredService<TracedAnswer>(), 1));

    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}