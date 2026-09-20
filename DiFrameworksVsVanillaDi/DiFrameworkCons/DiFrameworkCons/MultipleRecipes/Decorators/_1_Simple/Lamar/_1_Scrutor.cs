using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.Lamar;

public static class _1_Scrutor
{
  /// <summary>
  /// Lamar supports decorators and interceptors
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsAndInterceptors()
  {
    using var container = new Container(builder =>
    {
      builder.AddTransient<IAnswer, Answer>();

      //Decorators and interceptors are applied in the order they are registered
      builder.For<IAnswer>().DecorateAllWith<TracedAnswer>();
      builder.For<IAnswer>().InterceptAll((context, a) =>
        ActivatorUtilities.CreateInstance<SynchronizedAnswer>(context, a, 1));
    });

    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}