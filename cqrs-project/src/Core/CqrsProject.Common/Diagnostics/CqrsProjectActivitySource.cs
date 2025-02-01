using System.Diagnostics;

namespace CqrsProject.Common.Diagnostics;

public class CqrsProjectActivitySource
{
    public readonly ActivitySource ActivitySourceDefault;
    public CqrsProjectActivitySource(string serviceNameDefault)
    {
        ActivitySourceDefault = new ActivitySource(serviceNameDefault);
    }
}
