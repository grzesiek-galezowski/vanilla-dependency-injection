using ApplicationLogic.AddNewTodoNote;
using ApplicationLogic.Ports;
using NSubstitute;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Invokable;
using TddXt.AnyRoot.Strings;

namespace ApplicationLogicTests.AddNewTodoNote;

public class NoteDefinitionByDtoTests
{
  [Test]
  public async Task ShouldSaveNoteWithContentCorrectedByTheWordConversions()
  {
    //GIVEN
    var content1 = Any.String();
    var content2 = Any.String();
    var content3 = Any.String();
    var content4 = Any.String();
    var dto = Any.Instance<NewTodoNoteDefinitionDto>() with
    {
      Content = content1
    };
    var wordConversion1 = Substitute.For<IWordConversion>();
    var wordConversion2 = Substitute.For<IWordConversion>();
    var wordConversion3 = Substitute.For<IWordConversion>();
    var definition = new NoteDefinitionByDto(dto,
    [
      wordConversion1,
      wordConversion2,
      wordConversion3,
    ]);
    var dao = Substitute.For<ITodoNoteDao>();
    var steps = Any.Instance<IAfterTodoNotePersistenceSteps>();
    var cancellationToken = Any.CancellationToken();

    wordConversion1.Apply(content1).Returns(content2);
    wordConversion2.Apply(content2).Returns(content3);
    wordConversion3.Apply(content3).Returns(content4);

    definition.Correct();

    //WHEN
    await definition.PersistIn(dao, steps, cancellationToken);

    //THEN
    await dao.Received(1)
      .Save(
        dto with
        {
          Content = content4
        }, cancellationToken);
  }

  [Test]
  public async Task ShouldExecuteNextStepsWithTheResultOfNotePersistence()
  {
    //GIVEN
    var dto = Any.Instance<NewTodoNoteDefinitionDto>();
    var definition = new NoteDefinitionByDto(dto, Any.List<IWordConversion>());
    var dao = Substitute.For<ITodoNoteDao>();
    var steps = Substitute.For<IAfterTodoNotePersistenceSteps>();
    var cancellationToken = Any.CancellationToken();
    var metadata = Any.Instance<TodoNoteMetadataDto>();

    dao.Save(dto, cancellationToken).Returns(metadata);

    //WHEN
    await definition.PersistIn(dao, steps, cancellationToken);

    //THEN
    await steps.Received(1).ExecuteFor(metadata, cancellationToken);
  }
}