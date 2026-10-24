namespace VyshKat.Domain.Interfaces;

public interface ITransport : IInventoryItem
{
    int BeginnerSimplicity { get; }

    bool IsServiceable { get; }
}
