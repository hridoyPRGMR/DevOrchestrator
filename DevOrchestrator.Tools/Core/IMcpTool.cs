using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevOrchestrator.Tools.Core;

public interface IMcpTool
{
    string Name { get; }
    Task<object> ExecuteAsync(Dictionary<string, object> args);
}
