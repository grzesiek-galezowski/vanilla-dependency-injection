namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.Autofac;

public static class _2_RegisterDecoratorWithLambdas
{
  /// <summary>
  /// This version pragmatically falls back to manual composition
  /// for the troublesome dependency
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsUsingAutofacWithABitOfManualComposition()
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
}