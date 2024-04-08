using Flurl.Http;
using Microsoft.Net.Http.Headers;
using TodoAppTests.TestDtos;

namespace TodoAppTests.AdapterTests.Endpoints.Automation;

public class AddTodoItemRequest
{
  private readonly IFlurlRequest _request;

  public AddTodoItemRequest(IFlurlRequest request)
  {
    _request = request
      .WithHeader(HeaderNames.Accept, "application/json");
  }

  public Task<IFlurlResponse> AttemptToExecuteWith(NewTodoNoteDefinitionTestDto dto)
  {
    return _request
      .AllowAnyHttpStatus()
      .PostJsonAsync(dto);
  }

  public AddTodoItemRequest WithoutAcceptHeader()
  {
    _request.Headers.Remove(HeaderNames.Accept);
    return this;
  }

  public AddTodoItemRequest WithoutWrongAcceptHeaderValue()
  {
    _request.Headers.AddOrReplace(HeaderNames.Accept, "figizmakiem");
    return this;
  }
}