using Apache.Ignite.Core;
using Apache.Ignite.Core.Client;

namespace Cache.Operations;

public interface IClient
{
    IIgniteClient Client();
}

public class IgniteConnection : IClient
{
    private string[] _endpoints;
    public IgniteConnection(params string[] endpoints)
    {
        _endpoints = endpoints;
    }

    public IIgniteClient Client()
    {
        var cofiguration = new IgniteClientConfiguration
        {
            Endpoints = _endpoints
        };

        return Ignition.StartClient(cofiguration);
    }

    
}