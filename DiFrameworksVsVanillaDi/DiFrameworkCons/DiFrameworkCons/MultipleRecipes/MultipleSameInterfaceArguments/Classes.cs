using Autofac.Features.AttributeFilters;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments;

public interface IDataStorage;

public record LocalDataStorage : IDataStorage;

public record ArchiveService(
  IDataStorage LocalStorage,
  IDataStorage RemoteStorage);


public record RemoteDataStorage : IDataStorage;

public enum Storages
{
  Local, Remote
}