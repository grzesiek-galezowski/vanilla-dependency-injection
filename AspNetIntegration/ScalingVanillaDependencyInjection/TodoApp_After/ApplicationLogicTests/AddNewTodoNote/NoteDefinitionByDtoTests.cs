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
    var initialContent = Any.String();
    var contentAfterConversion = Any.String();
    var dto = Any.Instance<NewTodoNoteDefinitionDto>() with
    {
      Content = initialContent
    };
    var wordConversion = Substitute.For<IWordConversion>();
    var definition = new NoteDefinitionByDto(dto, wordConversion);
    var dao = Substitute.For<ITodoNoteDao>();
    var steps = Any.Instance<IAfterTodoNotePersistenceSteps>();
    var cancellationToken = Any.CancellationToken();

    wordConversion.Apply(initialContent).Returns(contentAfterConversion);

    definition.Correct();

    //WHEN
    await definition.PersistIn(dao, steps, cancellationToken);

    //THEN
    await dao.Received(1)
      .Save(
        dto with
        {
          Content = contentAfterConversion
        }, cancellationToken);
  }

  [Test]
  public async Task ShouldExecuteNextStepsWithTheResultOfNotePersistence()
  {
    //GIVEN
    var dto = Any.Instance<NewTodoNoteDefinitionDto>();
    IEnumerable<IWordConversion> conversions = Any.List<IWordConversion>();
    var definition = new NoteDefinitionByDto(dto, new CompoundConversion(conversions));
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