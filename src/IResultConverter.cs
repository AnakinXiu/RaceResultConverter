namespace RaceResultConverter;

public interface IResultConverter<TFrom, TTo> where TFrom : IRaceResult where TTo : IRaceResult
{
    TTo ConvertToTarget(string jsonFilePath);
}