using FluentAssertions;
using Flurl.Http;
using TodoAppTests.TestDtos;

namespace TodoAppTests.AdapterTests.Endpoints.Automation;

public class AddTodoItemResponse(IFlurlResponse response)
{
  public void ShouldBeSuccessful()
  {
    response.StatusCode.Should().Be(200);
  }

  public async Task ShouldContainFilledMetadata()
  {
    var todoNoteMetadataTestDto = await response.GetJsonAsync<TodoNoteMetadataTestDto>();
    todoNoteMetadataTestDto.Should().NotBeNull();
    todoNoteMetadataTestDto.Id.Should().NotBeEmpty();
  }

  public void ShouldBeBadRequest()
  {
    response.StatusCode.Should().Be(400);
  }
}