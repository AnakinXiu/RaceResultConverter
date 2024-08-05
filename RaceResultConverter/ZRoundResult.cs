using Newtonsoft.Json;

namespace RaceResultConverter;

[JsonObject(MemberSerialization.OptIn)]
public class ZRoundResult : IRaceResult
{
    public ZRoundResult()
    { }

    public ZRoundResult(RcfFile rcfFile)
    {
        Description = rcfFile.Description;
        Duration = rcfFile.Duration;
        MinLapTime = rcfFile.MinLapTime;
        TimeToFinish = rcfFile.TimeToFinish;
    }

    public string Description { get; set; }

    public int Duration { get; set; }

    public float MinLapTime { get; set; }

    public float  TimeToFinish { get; set; }

    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;

    [JsonProperty("lapcount")]
    public int LapCount { get; set; }

    [JsonProperty("bestlap")]
    public string Bestlap { get; set; }

    [JsonProperty("start")]
    public string Start { get; set; }

    [JsonProperty("finish")]
    public string Finish { get; set; }

    [JsonProperty("grid")]
    public int[] Grid { get; set; }

    [JsonProperty("classification")]
    public Classification[] Classification { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
public sealed class Classification
{
    [JsonProperty("pos")]
    public int Pos { get; set; }

    [JsonProperty("points")] 
    public int Points { get; set; }

    [JsonProperty("racerid")] 
    public int RacerId { get; set; }

    [JsonProperty("lapcount")] 
    public int LapCount { get; set; }

    [JsonProperty("time")] 
    public string Time { get; set; }

    [JsonProperty("gap")] 
    public string Gap { get; set; }

    [JsonProperty("diff")] 
    public string Diff { get; set; }

    [JsonProperty("best")] 
    public string Best { get; set; }

    [JsonProperty("mean")] 
    public string Mean { get; set; }

    [JsonProperty("laps")] public IList<Laps> Laps { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
public sealed class Laps
{
    [JsonProperty("v")] 
    public string CurrentLapTime { get; set; } = string.Empty;

    [JsonProperty("p")]
    public int Position { get; set; }

    [JsonProperty("t")] 
    public string TotalTime { get; set; } = string.Empty;

    [JsonProperty("b")] 
    public int B { get; set; }
}