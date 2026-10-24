using VyshKat.Domain.Interfaces;

namespace VyshKat.Services;

// Упрощённый техосмотр: допуск зависит от исправности, а не от удобства новичка.
public sealed class ServiceCenter : IServiceCenter
{
    public bool Inspect(ITransport transport)
    {
        ArgumentNullException.ThrowIfNull(transport);
        return transport.IsServiceable;
    }
}
