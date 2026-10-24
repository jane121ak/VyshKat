namespace VyshKat.Domain;

internal static class EnergyConsumptionValidator
{
    public static double Validate(double dailyEnergyConsumption)
    {
        if (!double.IsFinite(dailyEnergyConsumption) || dailyEnergyConsumption < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailyEnergyConsumption), "Энергопотребление должно быть конечным и неотрицательным.");
        }

        return dailyEnergyConsumption;
    }
}
