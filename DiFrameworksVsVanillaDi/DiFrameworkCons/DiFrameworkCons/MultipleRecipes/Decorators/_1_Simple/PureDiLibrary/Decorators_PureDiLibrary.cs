using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.PureDiLibrary;

public class Decorators_PureDiLibrary
{
  [Test]
  public static void ShouldAssembleDecorators()
  {
    //GIVEN
    var composition = new Composition7();

    var answer = composition.Answer;
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}

partial class Composition7
{
  public void Setup()
  {
    DI.Setup(nameof(Composition7))
      .Bind<IAnswer>("answer").To<Answer>()
      .Bind<IAnswer>("tracedAnswer").To(
        context =>
        {
          context.Inject<IAnswer>("answer", out var answer);
          return new TracedAnswer(answer);
        })
      .Bind<IAnswer>("synchronizedAnswer").To(
        context =>
        {
          context.Inject<IAnswer>("tracedAnswer", out var answer);
          return new SynchronizedAnswer(answer, 1);
        })
      .Root<IAnswer>("Answer", "synchronizedAnswer");
  }
}