using TodoAppTests.TestDtos;

namespace TodoAppTests.HostTests.Automation;

public class CreateTodoNoteResponse
{
  private readonly TodoNoteMetadataTestDto _todoNoteMetadataDto;

  public CreateTodoNoteResponse(TodoNoteMetadataTestDto todoNoteMetadataDto)
  {
    _todoNoteMetadataDto = todoNoteMetadataDto;
  }

  public Guid Id => _todoNoteMetadataDto.Id;
}