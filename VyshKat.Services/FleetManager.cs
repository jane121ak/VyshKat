using VyshKat.Domain.Interfaces;

namespace VyshKat.Services;

public sealed class FleetManager
{
    private readonly List<IInventoryItem> _items = new();
    private readonly IServiceCenter _serviceCenter;

    public FleetManager(IServiceCenter serviceCenter)
    {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    public bool TryAddTransport(ITransport transport)
    {
        ArgumentNullException.ThrowIfNull(transport);
        EnsureUniqueInventoryNumber(transport.InventoryNumber);

        if (!_serviceCenter.Inspect(transport))
        {
            return false;
        }

        _items.Add(transport);
        return true;
    }

    public void AddThing(IThing thing)
    {
        ArgumentNullException.ThrowIfNull(thing);
        EnsureUniqueInventoryNumber(thing.InventoryNumber);
        _items.Add(thing);
    }

    public int GetTransportCount() => _items.OfType<ITransport>().Count();

    public int GetThingCount() => _items.OfType<IThing>().Count();

    public double GetTotalDailyEnergyConsumption() =>
        _items.OfType<IPowerConsumer>().Sum(item => item.DailyEnergyConsumption);

    public IReadOnlyList<ITransport> GetBeginnerFriendlyTransports() =>
        _items.OfType<ITransport>()
            .Where(transport => transport.BeginnerSimplicity >= 6)
            .ToArray();

    // Возвращаем доступный только для чтения снимок, чтобы нельзя было обойти техосмотр.
    public IReadOnlyList<IInventoryItem> GetAllItems() => _items.ToArray();

    private void EnsureUniqueInventoryNumber(string inventoryNumber)
    {
        if (_items.Any(item => string.Equals(
                item.InventoryNumber,
                inventoryNumber,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Объект с инвентарным номером '{inventoryNumber}' уже состоит на балансе.");
        }
    }
}
