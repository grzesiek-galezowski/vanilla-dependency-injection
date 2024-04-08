using System.Text.Json;
using Core.NullableReferenceTypesExtensions;

namespace TodoApp.Database;

public class FileStorage(string filePath)
{
  public async Task Save<T>(T value, CancellationToken cancellationToken)
  {
    var serializedDto = JsonSerializer.Serialize(value);
    await File.WriteAllTextAsync(filePath, serializedDto, cancellationToken);
  }

  public async Task<T> GetValue<T>(T defaultValue, CancellationToken cancellationToken)
  {
    var fileText = await File.ReadAllTextAsync(filePath, cancellationToken);
    if (fileText.Length == 0)
    {
      return defaultValue;
    }
    var value = JsonSerializer.Deserialize<T>(fileText).OrThrow();
    return value;
  }
}