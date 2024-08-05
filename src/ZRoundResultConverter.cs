using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using RaceResultConverter.Zon;

namespace RaceResultConverter;

public class ZRoundResultConverter : IResultConverter<ZRoundResult, ZonResult>
{
    private readonly RcfFileParser _rcfFileParser;
    private readonly ZRoundToZonResultConverter _resultConverter;

    public ZRoundResultConverter(Func<ZRoundResult, ZonResult> convert)
    {
        _rcfFileParser = new RcfFileParser();
        _resultConverter = new ZRoundToZonResultConverter(convert);
    }

    public ZonResult ConvertToTarget(string rcfFilePath)
    {
        var rcfFile = _rcfFileParser.ParseRaceResult(rcfFilePath);
        var jsonFilePath = Path.Combine(Path.GetDirectoryName(rcfFilePath), $"{rcfFile.Run}.json");

        var zonResult =  _resultConverter.ConvertToTarget(jsonFilePath, rcfFile);

        if (zonResult == null)
            throw new JsonSerializationException();

        return zonResult;
    }
}

public class ZRoundToZonResultConverter
{
    private readonly Func<ZRoundResult, ZonResult> _convert;

    public ZRoundToZonResultConverter(Func<ZRoundResult, ZonResult> convert)
    {
        _convert = convert;
    }

    public ZonResult ConvertToTarget(string jsonFilePath, RcfFile rcfFile)
    {
        var zRoundResult = GetZRoundResult(jsonFilePath);

        var zonResult = _convert(zRoundResult);

        zonResult.RaceDataProp.RaceTime = rcfFile.Duration / 60;
        zonResult.RaceDataProp.Name = rcfFile.Description;

        if (string.IsNullOrEmpty(zonResult.RaceDataProp.RaceTitle))
            zonResult.RaceDataProp.RaceTitle = string.IsNullOrEmpty(rcfFile.Description) ? "Default Race" : rcfFile.Description;

        if (string.IsNullOrEmpty(zonResult.RaceDataProp.PrintTitle))
            zonResult.RaceDataProp.PrintTitle = string.IsNullOrEmpty(rcfFile.Description) ? "Default Race" : rcfFile.Description;

        zonResult.RaceDataProp.Name = zonResult.RaceDataProp.RaceTitle;

        zonResult.Laps = rcfFile.Cars.Select(car => new Zon.Laps { Name = car.Name, Key = car.SenorNumber })
            .ToArray();

        var laps = rcfFile.Cars.Select(car => new CarLaps(car, zRoundResult.Classification.First(classification => classification.RacerId == car.Id))).ToList();

        zonResult.EntryLaps = laps.ToDictionary(
            lap => lap.Car.SenorNumber,
            lap => lap.Classification.Laps.Select(l => new Lap
            {
                Ave = lap.Classification.Laps.First() != l,
                Chk = true,
                LapIndex = lap.Classification.Laps.IndexOf(l) + 1,
                Hit = l.Position,
                Time = float.TryParse(l.CurrentLapTime, out var time) ? time : float.NaN,
                Totaltime = ResultTotalMilliseconds(l)
            }).ToArray());

        int ResultTotalMilliseconds(Laps laps1)
        {
            var laps1TotalTime = (laps1.TotalTime.Contains(':') ? laps1.TotalTime : ($"0:{laps1.TotalTime}"));
            return TimeSpan.TryParse($"0:0:{laps1TotalTime}", new CultureInfo("en-US"), out var total)
                ? (int)total.TotalMilliseconds / 10 : 0;
        }

        return zonResult;
    }

    public virtual ZRoundResult GetZRoundResult(string jsonFilePath)
    {
        using var reader = new StreamReader(jsonFilePath);
        var readToEnd = reader.ReadToEnd();

        var result = JsonConvert.DeserializeObject<ZRoundResult>(readToEnd);
        return result ?? throw new JsonSerializationException(readToEnd);
    }


    private class CarLaps
    {
        public CarLaps(Car car, Classification classification)
        {
            Car = car;
            Classification = classification;
        }

        public Car Car { get; set; }
        public Classification Classification { get; set; }
    }
}