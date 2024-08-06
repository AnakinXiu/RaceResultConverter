using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Win32;
using Newtonsoft.Json;
using RaceResultConverter.Zon;

namespace RaceResultConverter;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly OpenFileDialog _openFileDialog;
    public RelayCommand ConvertCommand { get; set; }

    public RelayCommand SelectFileCommand { get; set; }

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            _selectedFilePath = value;
            OnPropertyChanged(nameof(SelectedFilePath));
        }
    }

    public ConvertType ConvertType
    {
        get => _convertType;
        set
        {
            _convertType = value;
            _openFileDialog.Filter = GetFileFilterString();
            OnPropertyChanged(nameof(ConvertType));
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

    private bool _sourceFileSelected;
    private bool _convertResult;
    private string _convertMessage = string.Empty;
    private string _selectedFilePath = string.Empty;
    private ConvertType _convertType;

    private bool SourceFileSelected
    {
        get => _sourceFileSelected;
        set
        {
            _sourceFileSelected = value;
            OnPropertyChanged(nameof(SourceFileSelected));
            ConvertCommand.RaiseCanExecuteChanged();
        }
    }

    public MainWindowViewModel()
    {
        _openFileDialog = new OpenFileDialog
        {
            Filter = GetFileFilterString()
        };

        ConvertCommand = new RelayCommand(Convert, CanConvert);
        SelectFileCommand = new RelayCommand(SelectFile);
    }

    private bool CanConvert() => _sourceFileSelected && _convertType == ConvertType.ZRoundToZon;    //Currently, only support convert ZRound result to Zon's.

    private string GetFileFilterString()
    {
        return _convertType == ConvertType.ZRoundToZon ? "ZRound Race File|*.rcf" : "JSON File|*.json";
    }

    private void SelectFile()
    {
        SourceFileSelected = _openFileDialog.ShowDialog() ?? false;
        SelectedFilePath = _openFileDialog.FileName;
    }

    private static IRaceResult Convert(ConvertType convertType, string jsonFile)
    {
        return convertType switch
        {
            ConvertType.ZRoundToZon => new ZRoundResultConverter(ConvertZRoundToZon).ConvertToTarget(jsonFile),
            ConvertType.ZonToZRound => new ResultConverter<ZonResult, ZRoundResult>(ConvertZonToZRound).ConvertToTarget(jsonFile),
            _ => throw new ArgumentOutOfRangeException(nameof(convertType), convertType, null)
        };
    }

    private void Convert()
    {
        var fullPath = Path.GetDirectoryName(_openFileDialog.FileName);
        var result = Convert(ConvertType, _openFileDialog.FileName);

        ConvertMessage = SaveJson(result, fullPath) ? "Success" : "Failed";
    }

    private static bool SaveJson(IRaceResult result, string outputPath)
    {
        try
        {
            var s = JsonConvert.SerializeObject(result);
            using var file = new FileStream(Path.Combine(outputPath, "Result.json"), FileMode.OpenOrCreate);
            file.SetLength(0);
            var bytes = Encoding.UTF8.GetBytes(s);
            file.Write(bytes, 0, bytes.Length);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static ZonResult ConvertZRoundToZon(ZRoundResult arg)
    {
        return new ZonResult
        {
            RaceDataProp = new RaceDataProp
            {
                RaceTitle = arg.Name,
                PrintTitle = arg.Name,
                Time = arg.Start,
                RaceRoundid = arg.Start,
                RaceMode = 1
            }
        };
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