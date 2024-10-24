﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using EpcDataApp.GrpcClient;
using ConsoleTables;
using Grpc.Core;

Console.Title = "EpcDataApp";

while (true)
{
    try
    {
        await StartProgram();
    }
    catch (RpcException ex)
    {
        Helpers.DisplayError(ex.Status.Detail);
    }
    catch (Exception ex)
    {
        Helpers.DisplayError(ex.Message);
    }
}


async Task StartProgram()
{
    using var chanell = GrpcChannel.ForAddress("https://localhost:7082");
    var client = new EpcData.EpcDataClient(chanell);

    while (true)
    {
        Helpers.PrintMenu();
        bool status = true;
        switch (Convert.ToInt64(Console.ReadLine()))
        {
            case 1:
                status = true;
                Console.WriteLine("Полуение данных о вагонах за период времени");
                while (status)
                {
                    Helpers.DisplayInvite("Введите начальную дату и время в формате(dd.mm.yyyy hh:mm:ss) ");
                    var inputDateTimeStart = Console.ReadLine();
                    if (!DateTime.TryParse(inputDateTimeStart, out DateTime InputDateTimeStart))
                    {
                        Helpers.DisplayError("Значение не валидно!");
                    }
                    else
                    {
                        while (status)
                        {
                            Helpers.DisplayInvite("Введите конечную дату в формате(dd.mm.yyyy hh:mm:ss) ");
                            var inputDateTimeEnd = Console.ReadLine();
                            if (!DateTime.TryParse(inputDateTimeEnd, out DateTime InputDateTimeEnd))
                            {
                                Helpers.DisplayError("Значение не валидно!");
                            }
                            else if (InputDateTimeStart > InputDateTimeEnd)
                            {
                                Helpers.DisplayError("Значение не валидно, конечная дата не может быть меньше начальной!");
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
                                    Helpers.DisplayError("За указанный период данные о вагонах отсутствуют!!!");
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
                    Helpers.DisplayInvite("Введите инвентарный номер вагона:");
                    var inputNumber = Console.ReadLine();
                    if (string.IsNullOrEmpty(inputNumber))
                    {
                        Helpers.DisplayError("Значение не валидно, номер не может быть пустым!");
                    }
                    else if (inputNumber.All(i => !char.IsDigit(i)))
                    {
                        Helpers.DisplayError("Значение не валидно, номер не может содержать буквы и пробелы!");
                    }
                    else
                    {
                        var result = await client.GetPathListCrossMoveEpcAsync(new PathRequest()
                        {
                            NumberEpc = inputNumber
                        });
                        if (result.Paths.Count > 0)
                        {
                            var table = new ConsoleTable(new string[] { "Id пути", "Номер пути во внешней системе", "Id парка", "Название парка", "Номер парка во внешней системе", "Тип парка", "Направление движения" });
                            foreach (var path in result.Paths)
                            {
                                table.AddRow(new object[] { path.IdPath, path.AsuNumberPath, path.IdPark, path.NamePark, path.AsuNumberPark, path.TypePark, path.DirectionPark });
                            }
                            Console.WriteLine(table.ToString());
                            status = false;
                        }
                        else
                        {
                            Helpers.DisplayError("Данных с запрашиваемыми параметрами нет!!!");
                            status = false;
                        }
                    }
                }
                break;
            case 3:
                Environment.Exit(0);
                break;
            default:
                Helpers.DisplayError("Выберите пункт из меню!!!");
                break;
        }
    }
}

