using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Things;
using VyshKat.Domain.Transports;
using VyshKat.Services;

namespace VyshKat.Tests;

public sealed class FleetManagerTests
{
    [Fact]
    public void TryAddTransport_WhenInspectionAccepts_AddsTransport()
    {
        var serviceCenter = new FakeServiceCenter(accept: true);
        var manager = new FleetManager(serviceCenter);

        var accepted = manager.TryAddTransport(new Scooter("S-001", "Самокат", 5, 1.0));

        Assert.True(accepted);
        Assert.Equal(1, manager.GetTransportCount());
        Assert.Equal(1, serviceCenter.InspectionCount);
    }

    [Fact]
    public void TryAddTransport_WhenInspectionRejects_DoesNotChangeReports()
    {
        var serviceCenter = new FakeServiceCenter(accept: false);
        var manager = new FleetManager(serviceCenter);

        var accepted = manager.TryAddTransport(new Scooter("S-001", "Самокат", 10, 1.5));

        Assert.False(accepted);
        Assert.Equal(1, serviceCenter.InspectionCount);
        Assert.Equal(0, manager.GetTransportCount());
        Assert.Equal(0, manager.GetTotalDailyEnergyConsumption());
        Assert.Empty(manager.GetBeginnerFriendlyTransports());
        Assert.Empty(manager.GetAllItems());
    }

    [Fact]
    public void TryAddTransport_UsesInjectedServiceCenterInsteadOfRealOne()
    {
        // Даже заведомо неисправный транспорт принимается, если тестовый техосмотр разрешает.
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        var broken = new Bike("B-001", "Велосипед", 2, isServiceable: false);

        Assert.True(manager.TryAddTransport(broken));
        Assert.Single(manager.GetAllItems());
    }

    [Fact]
    public void EnergyReport_SumsAcceptedElectricTransportAndChargingStations()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        manager.TryAddTransport(new Scooter("S-001", "Самокат", 5, 1.2));
        manager.TryAddTransport(new ElectricBike("EB-001", "Электровелосипед", 5, 0.8));
        manager.TryAddTransport(new Bike("B-001", "Велосипед", 8));
        manager.AddThing(new ChargingStation("CS-001", "Зарядная станция", 5.0));
        manager.AddThing(new Helmet("H-001", "Шлем"));

        Assert.Equal(7.0, manager.GetTotalDailyEnergyConsumption(), precision: 3);
        Assert.Equal(3, manager.GetTransportCount());
        Assert.Equal(2, manager.GetThingCount());
    }

    [Fact]
    public void Reports_WhenFleetEmpty_ReturnZerosAndEmptyCollections()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));

        Assert.Equal(0, manager.GetTransportCount());
        Assert.Equal(0, manager.GetThingCount());
        Assert.Equal(0, manager.GetTotalDailyEnergyConsumption());
        Assert.Empty(manager.GetBeginnerFriendlyTransports());
        Assert.Empty(manager.GetAllItems());
    }

    [Fact]
    public void BeginnerReport_IncludesBoundarySixButExcludesFiveAndRejectedTransport()
    {
        var serviceCenter = new FakeServiceCenter(accept: true);
        var manager = new FleetManager(serviceCenter);
        manager.TryAddTransport(new Scooter("S-005", "Пять", 5, 1.0));
        manager.TryAddTransport(new Scooter("S-006", "Шесть", 6, 1.0));
        manager.TryAddTransport(new Bike("B-010", "Десять", 10));

        var beginners = manager.GetBeginnerFriendlyTransports();

        Assert.Equal(2, beginners.Count);
        Assert.Contains(beginners, transport => transport.InventoryNumber == "S-006");
        Assert.Contains(beginners, transport => transport.InventoryNumber == "B-010");
        Assert.DoesNotContain(beginners, transport => transport.InventoryNumber == "S-005");
    }

    [Fact]
    public void AddThing_AppearsInInventoryReportWithNameAndNumber()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        manager.AddThing(new Helmet(" H-001 ", " Шлем "));

        var item = Assert.Single(manager.GetAllItems());
        Assert.Equal("H-001", item.InventoryNumber);
        Assert.Equal("Шлем", item.Name);
    }

    [Fact]
    public void AddThing_WhenInventoryNumberAlreadyBelongsToTransport_Throws()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        manager.TryAddTransport(new Scooter("S-001", "Самокат", 5, 1.0));

        Assert.Throws<InvalidOperationException>(() =>
            manager.AddThing(new Helmet("s-001", "Дублирующий шлем")));
        Assert.Single(manager.GetAllItems());
    }

    [Fact]
    public void TryAddTransport_WhenInventoryNumberAlreadyBelongsToThing_ThrowsBeforeInspection()
    {
        var serviceCenter = new FakeServiceCenter(accept: true);
        var manager = new FleetManager(serviceCenter);
        manager.AddThing(new Helmet("H-001", "Шлем"));

        Assert.Throws<InvalidOperationException>(() =>
            manager.TryAddTransport(new Bike("h-001", "Велосипед", 5)));
        Assert.Equal(0, serviceCenter.InspectionCount);
        Assert.Single(manager.GetAllItems());
    }

    [Fact]
    public void TryAddTransport_WhenNumberWasRejected_CanBeAcceptedLater()
    {
        var serviceCenter = new FakeServiceCenter(accept: false);
        var manager = new FleetManager(serviceCenter);
        Assert.False(manager.TryAddTransport(new Bike("B-001", "Первый", 2)));

        // Отклонённое устройство не числится на балансе и не занимает номер.
        Assert.False(manager.TryAddTransport(new Bike("B-001", "Второй", 2)));
        Assert.Equal(2, serviceCenter.InspectionCount);
        Assert.Empty(manager.GetAllItems());
    }

    [Fact]
    public void GetAllItems_ReturnsSnapshotThatCannotModifyFleet()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        manager.AddThing(new Helmet("H-001", "Шлем"));

        var snapshot = (IInventoryItem[])manager.GetAllItems();
        snapshot[0] = new Helmet("H-999", "Другой шлем");

        Assert.Equal("H-001", Assert.Single(manager.GetAllItems()).InventoryNumber);
    }

    [Fact]
    public void InventoryNumbers_MustBeUniqueWithinSameCategory()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        manager.AddThing(new Helmet("H-001", "Первый шлем"));
        Assert.Throws<InvalidOperationException>(() =>
            manager.AddThing(new Helmet("h-001", "Второй шлем")));

        manager.TryAddTransport(new Bike("B-001", "Первый велосипед", 5));
        Assert.Throws<InvalidOperationException>(() =>
            manager.TryAddTransport(new Bike("b-001", "Второй велосипед", 5)));
        Assert.Equal(2, manager.GetAllItems().Count);
    }

    [Fact]
    public void NewTransportType_UsesExistingFleetReportsWithoutChanges()
    {
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        manager.TryAddTransport(new ExperimentalTransport("E-001", "Новый тип", 6, 2.5));

        Assert.Equal(1, manager.GetTransportCount());
        Assert.Equal(2.5, manager.GetTotalDailyEnergyConsumption());
        Assert.Single(manager.GetBeginnerFriendlyTransports());
    }

    [Fact]
    public void NullArguments_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new FleetManager(null!));
        var manager = new FleetManager(new FakeServiceCenter(accept: true));
        Assert.Throws<ArgumentNullException>(() => manager.TryAddTransport(null!));
        Assert.Throws<ArgumentNullException>(() => manager.AddThing(null!));
    }

    private sealed class ExperimentalTransport : TransportBase, IPowerConsumer
    {
        public double DailyEnergyConsumption { get; }

        public ExperimentalTransport(string inventoryNumber, string name, int simplicity, double energy)
            : base(inventoryNumber, name, simplicity)
        {
            DailyEnergyConsumption = energy;
        }
    }
}
