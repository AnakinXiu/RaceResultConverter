using System.Globalization;
using Newtonsoft.Json;
using RaceResultConverter.DTO;
using RaceResultConverter.DTO.Zon;
using RaceResultConverter.DTO.ZRound;
using Laps = RaceResultConverter.DTO.ZRound.Laps;

namespace RaceResultConverter.Conversion;

public class ZRoundResultConverter : IResultConverter<ZRoundResult, ZonResult>
{
    public ZonResult ConvertToTarget(ZRoundResult rcfFilePath)
    {
        var zonResult = InnerConvert(rcfFilePath);

        if (zonResult == null)
            throw new JsonSerializationException();

        return zonResult;
    }

    private static ZonResult InnerConvert(ZRoundResult zRoundResult)
    {
        var zonResult = new ZonResult
        {
            RaceDataProp = new RaceDataProp
            {
                RaceTitle = zRoundResult.Name,
                PrintTitle = zRoundResult.Name,
                Time = zRoundResult.Start,
                RaceRoundid = zRoundResult.Start,
                RaceMode = 1,
                RaceTime = zRoundResult.Duration / 60,
                Name = zRoundResult.Description
            }
        };

        if (string.IsNullOrEmpty(zonResult.RaceDataProp.RaceTitle))
            zonResult.RaceDataProp.RaceTitle = string.IsNullOrEmpty(zRoundResult.Description)
                ? "Default Race"
                : zRoundResult.Description;

        if (string.IsNullOrEmpty(zonResult.RaceDataProp.PrintTitle))
            zonResult.RaceDataProp.PrintTitle = string.IsNullOrEmpty(zRoundResult.Description)
                ? "Default Race"
                : zRoundResult.Description;

        zonResult.RaceDataProp.Name = zonResult.RaceDataProp.RaceTitle;

        zonResult.Laps = zRoundResult.Cars
                                     .Select(car => new DTO.Zon.Laps { Name = car.Name, Key = car.SenorNumber })
                                     .ToArray();

        if (!MergeRacerId(zRoundResult.Grid, zRoundResult.Cars))
            return zonResult;

        var laps = zRoundResult.Cars
                               .Select(car => new CarLaps(car, 
                                   zRoundResult.Classification.First(classification => classification.RacerId == car.Id)))
                               .ToList();

        zonResult.EntryLaps = laps.ToDictionary(
            lap => lap.Car.SenorNumber,
            lap => lap.Classification.Laps.Select(l => new Lap
            {
                Ave = lap.Classification.Laps.First() != l,
                Chk = true,
                LapIndex = lap.Classification.Laps.IndexOf(l) + 1,
                Hit = l.Position,
                Time = float.TryParse((string?)l.CurrentLapTime, out var time) ? time : float.NaN,
                Totaltime = ResultTotalMilliseconds(l)
            }).ToArray());

        return zonResult;

        int ResultTotalMilliseconds(Laps laps1)
        {
            var laps1TotalTime = (laps1.TotalTime.Contains(':') ? laps1.TotalTime : ($"0:{laps1.TotalTime}"));
            return TimeSpan.TryParse($"0:0:{laps1TotalTime}", new CultureInfo("en-US"), out var total)
                ? (int)total.TotalMilliseconds / 10
                : 0;
        }
    }

    private static bool MergeRacerId(int[] carGrid, ICollection<Car> cars)
    {
        // This is a workaround for the RacerID issue, currently I'm not sure the relationship between
        // the classification in Json file and the car sector in rcf file.

        if (carGrid.Length != cars.Count)
            return false;

        var i = 0;
        foreach (var car in cars)
        {
            car.Id = carGrid[i];
            i++;
        }

        return true;
    }

    private struct CarLaps
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