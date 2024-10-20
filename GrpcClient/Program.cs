using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using EpcDataApp.GrpcClient;
using ConsoleTables;
using Grpc.Core;

Console.Title = "EpcDataApp";

try
{
    await StartProgram();
}
catch (RpcException ex)
{
    Helpers.DisplayError(ex.Status.Detail);
    await StartProgram();
}
catch (Exception ex)
{
    Helpers.DisplayError(ex.Message);
    await StartProgram();
}
finally
{
    Environment.Exit(0);
}

async Task StartProgram()
{
    using var chanell = GrpcChannel.ForAddress("https://localhost:7082");
    var client = new EpcData.EpcDataClient(chanell);
    while (true)
    {
        Helpers.PrintMenu();
        bool status = true;
        switch (Convert.ToInt16(Console.ReadLine()))
        {
            case 1:
                status = true;
                Console.WriteLine("Полуение данных о вагонах за период времени");
                while (status)
                {
                    Console.WriteLine("Введите начальную дату и время в формате(dd.mm.yyyy hh:mm:ss) ");
                    var inputDateTimeStart = Console.ReadLine();
                    if (!DateTime.TryParse(inputDateTimeStart, out DateTime InputDateTimeStart))
                    {
                        Console.WriteLine("значение не валидно");
                    }
                    else
                    {
                        while (status)
                        {
                            Console.WriteLine("введите конечную дату в формате(dd.mm.yyyy hh:mm:ss) ");
                            var inputDateTimeEnd = Console.ReadLine();
                            if (!DateTime.TryParse(inputDateTimeEnd, out DateTime InputDateTimeEnd))
                            {
                                Console.WriteLine("значение не валидно");
                            }
                            else if (InputDateTimeStart > InputDateTimeEnd)
                            {
                                Console.WriteLine("Значение не валидно, конечная дата не может быть меньше начальной");
                            }
                            else
                            {
                                var result = await client.GetWagonsAsync(new WagonRequest()
                                {
                                    TimeStart = Timestamp.FromDateTime(InputDateTimeStart.ToUniversalTime()),
                                    TimeEnd = Timestamp.FromDateTime(InputDateTimeEnd.ToUniversalTime())
                                });

                                if (result.Wagons.Count == 0)
                                {
                                    Console.WriteLine("За указанный период данные о вагонах отсутствуют!!!");
                                }
                                else
                                {
                                    var table = new ConsoleTable(new string[] { "№", "Инвентарный номер вагона", "Время прибытия", "Время отправления" });
                                    foreach (var wagon in result.Wagons)
                                    {
                                        table.AddRow(result.Wagons.IndexOf(wagon) + 1, wagon.Number, wagon.TimeArrival, wagon.TimeDeparture);
                                    }
                                    Console.WriteLine(table.ToString());
                                }
                                status = false;
                            }
                        }
                    }
                }
                break;
            case 2:
                status = true;
                while (status)
                {
                    Console.WriteLine("Введите инвентарный номер вагона:");
                    var inputNumber = Console.ReadLine();
                    if (string.IsNullOrEmpty(inputNumber))
                    {
                        Console.WriteLine("Значение не валидно, номер не может быть пустым");
                    }
                    else if (inputNumber.All(i => char.IsLetter(i) || char.IsWhiteSpace(i)))
                    {
                        Console.WriteLine("Значение не валидно, номер не может содержать буквы и пробелы");
                    }
                    else
                    {
                        var result = await client.GetPathListCrossMoveEpcAsync(new PathRequest()
                        {
                            NumberEpc = inputNumber,
                            TypeEpc = 1
                        });
                        if (result.Paths.Count > 0)
                        {
                            var table = new ConsoleTable(new string[] { "IdPath", "AsuNumberPath", "IdPark", "NamePark", "AsuNumberPark", "TypePark", "DirectionPark" });
                            foreach (var path in result.Paths)
                            {
                                table.AddRow(new object[] { path.IdPath, path.AsuNumberPath, path.IdPark, path.NamePark, path.AsuNumberPark, path.TypePark, path.DirectionPark });
                            }
                            Console.WriteLine(table.ToString());
                            status = false;
                        }
                        else
                        {
                            Console.WriteLine("Данных с запрашиваемыми параметрами нет!!!");
                            status = false;
                        }
                    }
                }
                break;
            case 3:
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Выберите пункт из меню!!!");
                break;
        }
    }
}

