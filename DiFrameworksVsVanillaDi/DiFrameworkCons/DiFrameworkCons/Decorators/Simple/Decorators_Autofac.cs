namespace DiFrameworkCons.Decorators.Simple;

public static class Decorators_Autofac
{
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
  public static void ShouldAssembleDecoratorsUsingAutofac()
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