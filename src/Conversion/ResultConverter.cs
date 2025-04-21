using System.IO;
using Newtonsoft.Json;
using RaceResultConverter.DTO;

namespace RaceResultConverter.Conversion;

public class ResultConverter<TFrom, TTo> : IResultConverter<TFrom, TTo> where TFrom : IRaceResult where TTo : IRaceResult
{
    public ResultConverter(Func<TFrom, TTo> convert) 
    {
        _convert = convert;
    }

    public virtual TTo ConvertToTarget(string jsonFilePath)
    {
        using var reader = new StreamReader(jsonFilePath);
        var readToEnd = reader.ReadToEnd();

        var result = JsonConvert.DeserializeObject<TFrom>(readToEnd);
        return result == null ? throw new JsonSerializationException(readToEnd) : _convert(result);
    }

    private readonly Func<TFrom, TTo> _convert;
}