namespace VyshKat.Domain.Transports;

public sealed class Bike : TransportBase
{
    public Bike(string inventoryNumber, string name, int beginnerSimplicity, bool isServiceable = true)
        : base(inventoryNumber, name, beginnerSimplicity, isServiceable)
    {
    }
}
