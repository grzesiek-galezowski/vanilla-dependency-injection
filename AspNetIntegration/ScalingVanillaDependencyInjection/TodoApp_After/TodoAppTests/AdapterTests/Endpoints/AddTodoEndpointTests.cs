using TodoAppTests.AdapterTests.Endpoints.Automation;
using TodoAppTests.TestDtos;

namespace TodoAppTests.AdapterTests.Endpoints;

public class AddTodoEndpointTests
{
  [Test]
  public async Task ShouldRespondWithSuccessWhenCommandReportsASuccessToAddingATodoItem()
  {
    //GIVEN
    await using var driver = new EndpointsAdapterDriver();

    //WHEN
    var addTodoItemResponse = await driver.AttemptToAddATodoItem(
      Any.Instance<NewTodoNoteDefinitionTestDto>());

    //THEN
    addTodoItemResponse.ShouldBeSuccessful();
    await addTodoItemResponse.ShouldContainFilledMetadata();
  }

  [Test]
  public async Task ShouldRespondWithBadRequestWhenSendingAddTodoRequestWithoutTheAcceptHeader()
  {
    //GIVEN
    await using var driver = new EndpointsAdapterDriver();

    //WHEN
    var addTodoItemResponse = await driver.AttemptToAddATodoItem(
      Any.Instance<NewTodoNoteDefinitionTestDto>(), r => r.WithoutAcceptHeader());

    //THEN
    addTodoItemResponse.ShouldBeBadRequest();
  }

  [Test]
  public async Task ShouldRespondWithBadRequestWhenSendingAddTodoRequestWithWrongAcceptHeader()
  {
    //GIVEN
    await using var driver = new EndpointsAdapterDriver();

    //WHEN
    var addTodoItemResponse = await driver.AttemptToAddATodoItem(
      Any.Instance<NewTodoNoteDefinitionTestDto>(), r => r.WithoutWrongAcceptHeaderValue());

    //THEN
    addTodoItemResponse.ShouldBeBadRequest();
  }
}