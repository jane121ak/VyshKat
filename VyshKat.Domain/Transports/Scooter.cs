using VyshKat.Domain.Interfaces;
using VyshKat.Domain;

namespace VyshKat.Domain.Transports;

public sealed class Scooter : TransportBase, IPowerConsumer
{
    public double DailyEnergyConsumption { get; }

    public Scooter(
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
