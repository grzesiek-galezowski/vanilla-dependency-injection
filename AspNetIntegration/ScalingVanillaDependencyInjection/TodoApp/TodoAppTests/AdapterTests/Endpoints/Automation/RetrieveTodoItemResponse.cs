using FluentAssertions;
using Flurl.Http;
using TodoAppTests.TestDtos;

namespace TodoAppTests.AdapterTests.Endpoints.Automation;

public class RetrieveTodoItemResponse(IFlurlResponse response)
{
  public void ShouldBeSuccessful()
  {
    response.StatusCode.Should().Be(200);
  }

  public async Task ShouldContain(TodoNoteTestDto returnedDto)
  {
    var todoNoteTestDto = await response.GetJsonAsync<TodoNoteTestDto>();
    todoNoteTestDto.Should().Be(returnedDto);
  }
}