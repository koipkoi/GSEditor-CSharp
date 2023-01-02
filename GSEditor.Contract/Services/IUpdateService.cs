using GSEditor.Models;
using System.Threading.Tasks;

namespace GSEditor.Contract.Services;

public interface IUpdateService
{
  public Task<AppUpdate> GetAppUpdate();
}
