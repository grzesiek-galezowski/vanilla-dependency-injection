using System.Threading.Tasks;

namespace DiFrameworkRefactoring.Controllers
{
  public interface IWeatherCommand
  {
    Task Execute();
  }
}