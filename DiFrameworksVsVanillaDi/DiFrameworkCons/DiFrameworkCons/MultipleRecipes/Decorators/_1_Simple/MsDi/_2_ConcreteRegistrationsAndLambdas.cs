namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.MsDi;

public static class _2_ConcreteRegistrationsAndLambdas
{
  /// <summary>
  /// Alternatively, one can use registrations of concrete types
  /// together with lambda registrations.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsLambdaRegistrations()
  {
    var builder = new ServiceCollection();

    builder.AddTransient<Answer>();
    builder.AddTransient<TracedAnswer>(x =>
      ActivatorUtilities.CreateInstance<TracedAnswer>(
        x, x.GetRequiredService<Answer>()));
    builder.AddTransient<IAnswer, SynchronizedAnswer>(x =>
      ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
        x, x.GetRequiredService<TracedAnswer>(), 1));

    using var container = builder.BuildServiceProvider();
    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}