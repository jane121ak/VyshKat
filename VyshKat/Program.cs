using System.Globalization;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Things;
using VyshKat.Domain.Transports;
using VyshKat.Services;

Console.OutputEncoding = Encoding.UTF8;

var services = new ServiceCollection();
services.AddSingleton<IServiceCenter, ServiceCenter>();
services.AddSingleton<FleetManager>();

using var serviceProvider = services.BuildServiceProvider();
var fleetManager = serviceProvider.GetRequiredService<FleetManager>();

Console.WriteLine("=== ВЫШКАТ — УЧЁТ ПАРКА ===");

RunInteractive(fleetManager);

static void RunInteractive(FleetManager fleetManager)
{
    while (true)
    {
        Console.WriteLine("\n1 — Принять транспорт");
        Console.WriteLine("2 — Добавить вещь на баланс");
        Console.WriteLine("3 — Показать отчёты");
        Console.WriteLine("0 — Выйти");
        Console.Write("Ваш выбор: ");

        var choice = Console.ReadLine()?.Trim();
        if (choice is null or "0")
        {
            return;
        }

        try
        {
            switch (choice)
            {
                case "1":
                    PrintAcceptance(fleetManager, ReadTransport());
                    break;
                case "2":
                    var thing = ReadThing();
                    fleetManager.AddThing(thing);
                    Console.WriteLine($"Добавлено: {thing.Name} ({thing.InventoryNumber}).");
                    break;
                case "3":
                    PrintReports(fleetManager);
                    break;
                default:
                    Console.WriteLine("Неизвестный пункт меню.");
                    break;
            }
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"Операция не выполнена: {exception.Message}");
        }
    }
}

static ITransport ReadTransport()
{
    Console.WriteLine("Вид транспорта: 1 — самокат, 2 — велосипед, 3 — электровелосипед");
    var kind = ReadField("Номер вида: ");
    if (kind is not ("1" or "2" or "3"))
    {
        throw new ArgumentException("Неизвестный вид транспорта.");
    }

    var number = ReadField("Инвентарный номер: ");
    var name = ReadField("Наименование: ");
    var simplicity = ReadInt("Простота управления (1–10): ");
    var energy = kind == "2" ? 0 : ReadDouble("Расход энергии (кВт·ч/сутки): ");
    var isServiceable = ReadBoolean("Технически исправен? (да/нет): ");

    return kind switch
    {
        "1" => new Scooter(number, name, simplicity, energy, isServiceable),
        "2" => new Bike(number, name, simplicity, isServiceable),
        _ => new ElectricBike(number, name, simplicity, energy, isServiceable)
    };
}

static IThing ReadThing()
{
    Console.WriteLine("Вид вещи: 1 — шлем, 2 — зарядная станция");
    var kind = ReadField("Номер вида: ");
    if (kind is not ("1" or "2"))
    {
        throw new ArgumentException("Неизвестный вид вещи.");
    }

    var number = ReadField("Инвентарный номер: ");
    var name = ReadField("Наименование: ");
    return kind == "1" ? new Helmet(number, name)
                       : new ChargingStation(number, name, ReadDouble("Расход энергии (кВт·ч/сутки): "));
}

static string ReadField(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine()?.Trim() ?? throw new InvalidOperationException("Ввод завершён.");
}

static int ReadInt(string prompt)
{
    var value = ReadField(prompt);
    return int.TryParse(value, out var result)
        ? result
        : throw new ArgumentException("Нужно ввести целое число.");
}

static double ReadDouble(string prompt)
{
    var value = ReadField(prompt).Replace(',', '.');
    if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
    {
        return result;
    }

    throw new ArgumentException("Нужно ввести число (например, 1,2 или 1.2).");
}

static bool ReadBoolean(string prompt)
{
    return ReadField(prompt).ToLowerInvariant() switch
    {
        "да" or "д" or "yes" or "y" => true,
        "нет" or "н" or "no" or "n" => false,
        _ => throw new ArgumentException("Введите «да» или «нет».")
    };
}

static void PrintAcceptance(FleetManager fleetManager, ITransport transport)
{
    var accepted = fleetManager.TryAddTransport(transport);
    Console.WriteLine($"{transport.Name} ({transport.InventoryNumber}): " +
        (accepted ? "техосмотр пройден, принят в парк" : "техосмотр не пройден, отклонён"));
}

static void PrintReports(FleetManager fleetManager)
{
    Console.WriteLine("\n--- Отчёты ---");
    Console.WriteLine($"Транспортных средств: {fleetManager.GetTransportCount()}");
    Console.WriteLine($"Вещей: {fleetManager.GetThingCount()}");
    Console.WriteLine($"Суточное энергопотребление: {fleetManager.GetTotalDailyEnergyConsumption():0.###} кВт·ч");

    Console.WriteLine("\nТранспорт для новичков (простота ≥ 6):");
    var beginners = fleetManager.GetBeginnerFriendlyTransports();
    if (beginners.Count == 0)
    {
        Console.WriteLine("Нет подходящего транспорта.");
    }
    foreach (var transport in beginners)
    {
        Console.WriteLine($"- {transport.Name} (№ {transport.InventoryNumber}, простота {transport.BeginnerSimplicity})");
    }

    Console.WriteLine("\nВсё на балансе:");
    var items = fleetManager.GetAllItems();
    if (items.Count == 0)
    {
        Console.WriteLine("Парк пуст.");
    }
    foreach (var item in items)
    {
        var kind = item is ITransport ? "Транспорт" : "Вещь";
        Console.WriteLine($"- {kind}: {item.Name} (№ {item.InventoryNumber})");
    }
}
