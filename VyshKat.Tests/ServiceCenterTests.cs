using VyshKat.Domain.Transports;
using VyshKat.Services;

namespace VyshKat.Tests;

public sealed class ServiceCenterTests
{
    [Fact]
    public void Inspect_ServiceableAndDifficultTransport_IsAccepted()
    {
        var serviceCenter = new ServiceCenter();
        var transport = new Scooter("S-001", "Сложный самокат", 1, 1.0, isServiceable: true);

        Assert.True(serviceCenter.Inspect(transport));
    }

    [Fact]
    public void Inspect_BrokenAndBeginnerFriendlyTransport_IsRejected()
    {
        var serviceCenter = new ServiceCenter();
        var transport = new Bike("B-001", "Неисправный велосипед", 10, isServiceable: false);

        Assert.False(serviceCenter.Inspect(transport));
    }

    [Fact]
    public void Inspect_NullTransport_Throws()
    {
        var serviceCenter = new ServiceCenter();
        Assert.Throws<ArgumentNullException>(() => serviceCenter.Inspect(null!));
    }
}
