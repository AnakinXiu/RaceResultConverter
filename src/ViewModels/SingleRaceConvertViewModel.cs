using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using RaceResultConverter.Conversion;
using RaceResultConverter.DTO;
using RaceResultConverter.DTO.Zon;
using RaceResultConverter.DTO.ZRound;
using RaceResultConverter.Enum;
using RaceResultConverter.Utils;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace RaceResultConverter.ViewModels;

public class SingleRaceConvertViewModel : INotifyPropertyChanged
{
    private string _selectedFilePath = string.Empty;
    private readonly OpenFileDialog _openFileDialog;
    private readonly Func<ConvertType> _getConvertType;
    private readonly Action<bool, string> _setConvertResult;

    public RelayCommand ConvertCommand { get; set; }

    public RelayCommand SelectFileCommand { get; set; }

    private bool CanConvert() => SourceFileSelected;

    private bool SourceFileSelected => !string.IsNullOrEmpty(SelectedFilePath) && File.Exists(SelectedFilePath);

    private ConvertType ConvertType => _getConvertType();

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            _selectedFilePath = value;
            OnPropertyChanged(nameof(SelectedFilePath));
            ConvertCommand.RaiseCanExecuteChanged();
        }
    }

    public SingleRaceConvertViewModel(Func<ConvertType> getConvertType, Action<bool, string> setConvertResult)
    {
        _getConvertType = getConvertType;
        _setConvertResult = setConvertResult;
        _openFileDialog = new OpenFileDialog
        {
            Filter = GetFileFilterString()
        };

        ConvertCommand = new RelayCommand(Convert, CanConvert);
        SelectFileCommand = new RelayCommand(SelectFile);
    }

    private string GetFileFilterString()
    {
        return ConvertType == ConvertType.ZRoundToZon ? "ZRound Race File|*.rcf" : "JSON File|*.json";
    }

    private IRaceResult InnerConvert(string jsonFile)
    {
        return ConvertType switch
        {
            ConvertType.ZRoundToZon => new ZRoundResultConverter().ConvertToTarget(RcfFileParser.ParseZRoundResult(jsonFile)),
            ConvertType.ZonToZRound => new ResultConverter<ZonResult, ZRoundResult>(zonResult => new ZRoundResult()).ConvertToTarget(null),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void Convert()
    {
        var result = InnerConvert(_openFileDialog.FileName);

        var saveFileDialog = new SaveFileDialog
        {
            AddExtension = true,
            CreatePrompt = true,
            FileName = "Result",
            DefaultExt = ".json",
            Filter = "JSON File|*.json",
            ValidateNames = true
        };

        if (saveFileDialog.ShowDialog() ?? false)
        {
            var convertResult = JsonUtil.SaveAsJson(result, saveFileDialog.FileName);
            _setConvertResult(convertResult, convertResult ? Resource.ConvertResult_Succeed : Resource.ConvertResult_Failed);

            return;
        }

        _setConvertResult(false, Resource.ConvertResult_Cancelled);
    }


    private void SelectFile()
    {
        if (_openFileDialog.ShowDialog() ?? false)
            SelectedFilePath = _openFileDialog.FileName;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}