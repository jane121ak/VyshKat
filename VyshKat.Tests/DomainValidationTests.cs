using VyshKat.Domain.Things;
using VyshKat.Domain.Transports;

namespace VyshKat.Tests;

public sealed class DomainValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Transport_RejectsSimplicityOutsideOneToTen(int simplicity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Bike("B-001", "Велосипед", simplicity));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Transport_AcceptsSimplicityBoundaries(int simplicity)
    {
        var bike = new Bike("B-001", "Велосипед", simplicity);
        Assert.Equal(simplicity, bike.BeginnerSimplicity);
    }

    [Fact]
    public void Models_RejectBlankInventoryNumberAndName()
    {
        Assert.Throws<ArgumentException>(() => new Helmet("  ", "Шлем"));
        Assert.Throws<ArgumentException>(() => new Helmet("H-001", "  "));
        Assert.Throws<ArgumentException>(() => new Bike(" ", "Велосипед", 5));
        Assert.Throws<ArgumentException>(() => new Bike("B-001", " ", 5));
    }

    [Fact]
    public void PowerConsumers_RejectNegativeEnergy()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Scooter("S-001", "Самокат", 6, -1.0));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ElectricBike("EB-001", "Велосипед", 6, -0.1));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ChargingStation("CS-001", "Станция", -2.0));
    }

    [Fact]
    public void PowerConsumers_RejectNanAndInfiniteEnergy()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Scooter("S-001", "Самокат", 6, double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ElectricBike("EB-001", "Велосипед", 6, double.PositiveInfinity));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ChargingStation("CS-001", "Станция", double.NegativeInfinity));
    }

    [Fact]
    public void PowerConsumers_AllowZeroEnergy()
    {
        Assert.Equal(0, new Scooter("S-001", "Самокат", 6, 0).DailyEnergyConsumption);
        Assert.Equal(0, new ChargingStation("CS-001", "Станция", 0).DailyEnergyConsumption);
    }
}
