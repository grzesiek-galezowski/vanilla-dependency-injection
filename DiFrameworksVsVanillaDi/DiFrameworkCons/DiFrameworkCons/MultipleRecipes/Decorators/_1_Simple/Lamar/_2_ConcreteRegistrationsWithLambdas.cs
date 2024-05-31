using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.Lamar;

public static class _2_ConcreteRegistrationsWithLambdas
{
  /// <summary>
  /// Alternatively, one can use registrations of concrete types
  /// together with lambda registrations.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsLambdaRegistrations()
  {
    using var container = new Container(builder =>
    {
      builder.AddTransient<Answer>();
      builder.AddTransient<TracedAnswer>(x =>
        ActivatorUtilities.CreateInstance<TracedAnswer>(
          x, x.GetRequiredService<Answer>()));
      builder.AddTransient<IAnswer, SynchronizedAnswer>(x =>
        ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
          x, x.GetRequiredService<TracedAnswer>(), 1));
    });

    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}