using RaceResultConverter.DTO;

namespace RaceResultConverter.Conversion;

public interface IResultConverter<TFrom, TTo> where TFrom : IRaceResult where TTo : IRaceResult
{
    TTo ConvertToTarget(string jsonFilePath);
}