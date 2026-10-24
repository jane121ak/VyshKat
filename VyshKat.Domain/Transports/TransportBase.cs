using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Transports;

public abstract class TransportBase : ITransport
{
    public string InventoryNumber { get; }
    public string Name { get; }
    public int BeginnerSimplicity { get; }
    public bool IsServiceable { get; }

    protected TransportBase(
        string inventoryNumber,
        string name,
        int beginnerSimplicity,
        bool isServiceable = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inventoryNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (beginnerSimplicity is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(
                nameof(beginnerSimplicity), "Простота управления должна быть от 1 до 10.");
        }

        InventoryNumber = inventoryNumber.Trim();
        Name = name.Trim();
        BeginnerSimplicity = beginnerSimplicity;
        IsServiceable = isServiceable;
    }
}
