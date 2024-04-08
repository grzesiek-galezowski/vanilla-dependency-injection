using FluentAssertions;
using Flurl.Http;
using TodoAppTests.TestDtos;

namespace TodoAppTests.AdapterTests.Endpoints.Automation;

public class RetrieveTodoItemResponse
{
  private readonly IFlurlResponse _response;

  public RetrieveTodoItemResponse(IFlurlResponse response)
  {
    _response = response;
  }

  public void ShouldBeSuccessful()
  {
    _response.StatusCode.Should().Be(200);
  }

  public async Task ShouldContain(TodoNoteTestDto returnedDto)
  {
    var todoNoteTestDto = await _response.GetJsonAsync<TodoNoteTestDto>();
    todoNoteTestDto.Should().Be(returnedDto);
  }
}