using FluentAssertions;
using Flurl.Http;
using TodoAppTests.TestDtos;

namespace TodoAppTests.HostTests.Automation;

public class AttemptToCreateTodoNoteResponse(IFlurlResponse flurlResponse)
{
  public void ShouldBeSuccessful()
  {
    flurlResponse.StatusCode.Should().Be(200);
  }

  public async Task ShouldContainValidId()
  {
    var metadata = await DeserializeDto();
    metadata.Id.Should().NotBeEmpty();
  }

  private Task<TodoNoteMetadataTestDto> DeserializeDto()
  {
    return flurlResponse.GetJsonAsync<TodoNoteMetadataTestDto>();
  }
}