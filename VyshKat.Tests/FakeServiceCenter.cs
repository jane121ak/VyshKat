using VyshKat.Domain.Interfaces;

namespace VyshKat.Tests;

internal sealed class FakeServiceCenter : IServiceCenter
{
    private readonly bool _accept;

    public int InspectionCount { get; private set; }

    public FakeServiceCenter(bool accept)
    {
        _accept = accept;
    }

    public bool Inspect(ITransport transport)
    {
        InspectionCount++;
        return _accept;
    }
}
