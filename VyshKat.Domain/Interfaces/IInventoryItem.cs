namespace VyshKat.Domain.Interfaces;

// Общие сведения обо всём, что ставится на баланс.
public interface IInventoryItem
{
    string InventoryNumber { get; }
    string Name { get; }
}
