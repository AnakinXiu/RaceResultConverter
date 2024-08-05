using Newtonsoft.Json;

namespace RaceResultConverter.Zon;

[JsonObject(MemberSerialization.OptIn)]
public class ZonResult : IRaceResult
{
    [JsonProperty("Type")] 
    public string Type { get; set; } = "ZonAF RaceData";

    [JsonProperty("Version")]
    public int Version { get; set; } = 1;

    [JsonProperty("RaceDataProp")]
    public RaceDataProp RaceDataProp { get; set; }

    [JsonProperty("Laps")]
    public Laps[] Laps { get; set; }

    [JsonProperty("EntryLaps")]
    public Dictionary<string, Lap[]> EntryLaps { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
public class RaceDataProp
{
    [JsonProperty("RaceTitle")] public string RaceTitle { get; set; }
    [JsonProperty("PrintTitle")] public string PrintTitle { get; set; }
    [JsonProperty("RaceTime")] public int RaceTime { get; set; }
    [JsonProperty("Racemode")] public int RaceMode { get; set; }
    [JsonProperty("Time")] public string Time { get; set; }
    [JsonProperty("Name")] public string Name { get; set; }
    [JsonProperty("RaceRoundid")] public string RaceRoundid { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
public class Laps
{
    [JsonProperty("0")] public int _0 { get; set; }
    [JsonProperty("1")] public string Name { get; set; }
    [JsonProperty("2")] public int _2 { get; set; }
    [JsonProperty("3")] public float _3 { get; set; }
    [JsonProperty("4")] public string _4 { get; set; }
    [JsonProperty("5")] public double _5 { get; set; }
    [JsonProperty("6")] public string Key { get; set; }
    [JsonProperty("7")] public string _7 { get; set; }
    [JsonProperty("8")] public float _8 { get; set; }
    [JsonProperty("9")] public int _9 { get; set; }
    [JsonProperty("10")] public double _10 { get; set; }
    [JsonProperty("11")] public DateTimeOffset _11 { get; set; }
    [JsonProperty("12")] public int _12 { get; set; }
    [JsonProperty("13")] public object? _13 { get; set; }
    [JsonProperty("14")] public object? _14 { get; set; }
    [JsonProperty("15")] public float _15 { get; set; }
    [JsonProperty("16")] public float _16 { get; set; }
    [JsonProperty("17")] public object? _17 { get; set; }
    [JsonProperty("18")] public string _18 { get; set; }
    [JsonProperty("19")] public int _19 { get; set; }
    [JsonProperty("20")] public string _20 { get; set; }
    [JsonProperty("21")] public int _21 { get; set; }
    [JsonProperty("22")] public int _22 { get; set; }
}

[JsonObject(MemberSerialization.OptIn)]
public sealed record Lap
{
    [JsonProperty("Ave")] public bool Ave { get; set; }
    [JsonProperty("Chk")] public bool Chk { get; set; }
    [JsonProperty("Lap")] public int LapIndex { get; set; }
    [JsonProperty("Time")] public float Time { get; set; }
    [JsonProperty("Hit")] public int Hit { get; set; }
    [JsonProperty("Splash")] public bool Splash { get; set; }
    [JsonProperty("Dobon")] public bool Dobon { get; set; }
    [JsonProperty("Totaltime")] public int Totaltime { get; set; }
}