using FluentAssertions;
using TodoAppTests.TestDtos;

namespace TodoAppTests.HostTests.Automation;

public class RetrieveTodoNoteResponse(TodoNoteTestDto dto)
{
  public void ShouldContainNoteCreatedFrom(NewTodoNoteDefinitionTestDto definitionDto, Guid id)
  {
    dto.Title.Should().Be(definitionDto.Title);
    dto.Content.Should().Be(definitionDto.Content);
    dto.Id.Should().Be(id);
  }
}