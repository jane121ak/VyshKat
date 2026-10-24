using VyshKat.Domain.Interfaces;
using VyshKat.Domain;

namespace VyshKat.Domain.Things;

public sealed class ChargingStation : ThingBase, IPowerConsumer
{
    public double DailyEnergyConsumption { get; }

    public ChargingStation(string inventoryNumber, string name, double dailyEnergyConsumption)
        : base(inventoryNumber, name)
    {
        DailyEnergyConsumption = EnergyConsumptionValidator.Validate(dailyEnergyConsumption);
    }
}
