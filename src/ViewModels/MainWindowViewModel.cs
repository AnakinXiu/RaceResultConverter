using System.ComponentModel;
using System.Runtime.CompilerServices;
using RaceResultConverter.Enum;

namespace RaceResultConverter.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private ConvertType _convertType;
    private ConvertSourceType _convertSourceType;
    private bool _convertResult;
    private string _convertMessage = string.Empty;

    public ConvertType ConvertType
    {
        get => _convertType;
        set
        {
            _convertType = value;
            OnPropertyChanged(nameof(ConvertType));
        }
    }
  
    public ConvertSourceType ConvertSourceType
    {
        get => _convertSourceType;
        set
        {
            _convertSourceType = value;
            OnPropertyChanged(nameof(ConvertSourceType));
            OnPropertyChanged(nameof(IsSingleConvert));
            OnPropertyChanged(nameof(IsChampionshipConvert));
        }
    }

    public string ConvertMessage
    {
        get => _convertMessage;
        set
        {
            _convertMessage = value;
            OnPropertyChanged(nameof(ConvertMessage));
        }
    }

    public bool ConvertResult
    {
        get => _convertResult;
        set
        {
            _convertResult = value;
            OnPropertyChanged(nameof(ConvertResult));
        }
    }

    public bool IsSingleConvert => ConvertSourceType == ConvertSourceType.Single;

    public bool IsChampionshipConvert => ConvertSourceType == ConvertSourceType.Championship;

    public SingleRaceConvertViewModel SingleRaceConvertViewModel { get; }

    public ChampionshipResultConvertViewModel ChampionshipResultConvertViewModel{ get; }

    public MainWindowViewModel()
    {
        SingleRaceConvertViewModel = new SingleRaceConvertViewModel(() => ConvertType, SetConvertResult);
        ChampionshipResultConvertViewModel = new ChampionshipResultConvertViewModel(() => ConvertType, SetConvertResult);
    }

    private void SetConvertResult(bool result, string message)
    {
        ConvertMessage = message;
        ConvertResult = result;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}