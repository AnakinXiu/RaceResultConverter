using RaceResultConverter.DTO;

namespace RaceResultConverter.Conversion;

public class ResultConverter<TFrom, TTo> : IResultConverter<TFrom, TTo> where TFrom : IRaceResult where TTo : IRaceResult
{
    public ResultConverter(Func<TFrom, TTo> convert) 
    {
        _convert = convert;
    }

    public virtual TTo ConvertToTarget(TFrom sourceResult)
    {
        return _convert(sourceResult);
    }

    private readonly Func<TFrom, TTo> _convert;
}