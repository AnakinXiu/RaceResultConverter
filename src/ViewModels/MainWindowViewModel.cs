using System.ComponentModel;
using System.Runtime.CompilerServices;
using RaceResultConverter.DTO;
using RaceResultConverter.Enum;

namespace RaceResultConverter.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{

    private ConvertType _convertType;
    private ConvertSourceType _convertSourceType;

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

    public bool IsSingleConvert => ConvertSourceType == ConvertSourceType.Single;
    public bool IsChampionshipConvert => ConvertSourceType == ConvertSourceType.Championship;

    public SingleRaceConvertViewModel SingleRaceConvertViewModel { get; }
    public ChampionshipResultConvertViewModel ChampionshipResultConvertViewModel{ get; }

    public MainWindowViewModel()
    {
        SingleRaceConvertViewModel = new SingleRaceConvertViewModel(() => ConvertType);
        ChampionshipResultConvertViewModel = new ChampionshipResultConvertViewModel();
    }

    private static ZRoundResult ConvertZonToZRound(ZonResult arg)
    {
        return new ZRoundResult();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}