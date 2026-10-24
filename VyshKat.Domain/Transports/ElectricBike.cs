using VyshKat.Domain.Interfaces;
using VyshKat.Domain;

namespace VyshKat.Domain.Transports;

public sealed class ElectricBike : TransportBase, IPowerConsumer
{
    public double DailyEnergyConsumption { get; }

    public ElectricBike(
        string inventoryNumber,
        string name,
        int beginnerSimplicity,
        double dailyEnergyConsumption,
        bool isServiceable = true)
        : base(inventoryNumber, name, beginnerSimplicity, isServiceable)
    {
        DailyEnergyConsumption = EnergyConsumptionValidator.Validate(dailyEnergyConsumption);
    }
}
