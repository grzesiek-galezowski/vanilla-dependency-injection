using TddXt.AnyRoot;
using TodoAppTests.AdapterTests.Endpoints.Automation;
using TodoAppTests.TestDtos;

namespace TodoAppTests.AdapterTests.Endpoints;

public class RetrieveTodoEndpointTests
{
  [Test]
  public async Task ShouldRespondWithSuccessWhenCommandReportsASuccessToRetrievingATodoItem()
  {
    //GIVEN
    await using var driver = new EndpointsAdapterDriver();
    var id = Any.Guid();
    var returnedDto = Any.Instance<TodoNoteTestDto>();

    driver.RetrievingNoteFromAppLogicReturns(returnedDto);

    //WHEN
    var retrieveTodoItemResponse = await driver.AttemptToRetrieveATodoItem(id);

    //THEN
    retrieveTodoItemResponse.ShouldBeSuccessful();
    await retrieveTodoItemResponse.ShouldContain(returnedDto);
  }
}