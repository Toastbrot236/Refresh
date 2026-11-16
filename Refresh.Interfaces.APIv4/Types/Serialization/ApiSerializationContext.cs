using Refresh.Core.Types.Data;
using Refresh.Interfaces.APIv4.Types.Api;

namespace Refresh.Interfaces.APIv4.Types.Serialization;

public class ApiSerializationContext
{
    public ApiResponseDependencies Dependencies { get; init; } = new();
    public DataContext DataContext { get; init; }

    public ApiSerializationContext(DataContext dataContext)
    {
        this.DataContext = dataContext;
    }
}