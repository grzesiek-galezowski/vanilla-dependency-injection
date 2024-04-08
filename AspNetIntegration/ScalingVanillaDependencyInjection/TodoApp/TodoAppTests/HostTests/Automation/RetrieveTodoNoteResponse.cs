using FluentAssertions;
using TodoAppTests.TestDtos;

namespace TodoAppTests.HostTests.Automation;

public class RetrieveTodoNoteResponse
{
  private readonly TodoNoteTestDto _dto;

  public RetrieveTodoNoteResponse(TodoNoteTestDto dto)
  {
    _dto = dto;
  }

  public void ShouldContainNoteCreatedFrom(NewTodoNoteDefinitionTestDto definitionDto, Guid id)
  {
    _dto.Title.Should().Be(definitionDto.Title);
    _dto.Content.Should().Be(definitionDto.Content);
    _dto.Id.Should().Be(id);
  }
}