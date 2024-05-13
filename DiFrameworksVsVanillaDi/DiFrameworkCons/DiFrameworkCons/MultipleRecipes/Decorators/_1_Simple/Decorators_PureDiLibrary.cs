using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple;

public class Decorators_PureDiLibrary
{
  /// <summary>
  /// Pure.DI allows compositions by tagging parameters
  /// in constructors. This requires coupling classes
  /// to the composition mechanism and the generator
  /// can bind only one type to tag.
  ///
  /// In this example, I used subclasses and tagged them
  /// instead of the "production" classes. Normally
  /// that's not what I believe I should be doing,
  /// but here I wanted to keep the shared classes
  /// independent of Pure.DI.
  ///
  /// Still, it works for simple cases.
  /// </summary>
  [Test]
  public static void ContainerContainsSomeDeadCodeWithMsDi()
  {
    //GIVEN
    var composition = new Composition7(X: 1);

    var answer = composition.Answer;
    answer.Should().BeOfType<TaggedSynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TaggedTracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}

partial class Composition7
{
  public void Setup()
  {
    DI.Setup(nameof(Composition7))
      .Bind("tracedNested").To<Answer>()
      .Bind("synchronizedNested").To<TaggedTracedAnswer>()
      .RootBind<IAnswer>("Answer").To<TaggedSynchronizedAnswer>().Arg<int>("X");
  }
}

public record TaggedTracedAnswer(
  [Tag("tracedNested")] IAnswer NestedAnswer)
  : TracedAnswer(NestedAnswer);

public record TaggedSynchronizedAnswer(
  [Tag("synchronizedNested")] IAnswer NestedAnswer, int X)
  : SynchronizedAnswer(NestedAnswer, X);