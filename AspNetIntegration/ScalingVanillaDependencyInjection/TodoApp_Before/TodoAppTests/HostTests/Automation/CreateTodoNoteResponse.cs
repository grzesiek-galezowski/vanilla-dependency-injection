using TodoAppTests.TestDtos;

namespace TodoAppTests.HostTests.Automation;

public class CreateTodoNoteResponse(TodoNoteMetadataTestDto todoNoteMetadataDto)
{
  public Guid Id => todoNoteMetadataDto.Id;
}