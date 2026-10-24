using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Things;

public abstract class ThingBase : IThing
{
    public string InventoryNumber { get; }
    public string Name { get; }

    protected ThingBase(string inventoryNumber, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inventoryNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        InventoryNumber = inventoryNumber.Trim();
        Name = name.Trim();
    }
}
