using System.ComponentModel;

namespace RaceResultConverter;

public class RcfFile
{
    [DisplayName("manager")]
    public string Manager { get; set; } = string.Empty;

    [DisplayName("run")]
    public string Run { get; set; } = string.Empty;

    [DisplayName("description")] 
    public string Description { get; set; } = string.Empty;

    [DisplayName("lemans")]
    
    public int Lemans { get; set; }

    [DisplayName("duration")]
    public int Duration { get; set; }
    
    [DisplayName("min_lap_time")]
    public float MinLapTime { get; set; }

    [DisplayName("classification")]
    public int Classification { get; set; }

    [DisplayName("StartInterval")]
    public int StartInterval { get; set; }

    [DisplayName("TimeToFinish")]
    public float TimeToFinish { get; set; }

    public List<Car> Cars { get; set; } = new();
}