using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using RaceResultConverter.DTO;
using RaceResultConverter.DTO.ZRound;

namespace RaceResultConverter.Conversion;

public static class RcfFileParser
{
    private static string Extension => ".rcf";

    public static RcfFileParseResult ParseZRoundResult(string rcfFilePath)
    {
        if (!File.Exists(rcfFilePath))
            return RcfFileParseResult.Failed("rcf file doesn't exist.");

        if (!IsValidRcfFile(rcfFilePath))
            return RcfFileParseResult.Failed("rcf file is invalid.");

        var rcfFile = ParseRcfFile(rcfFilePath);
        var jsonFilePath = GetJsonFilePath(rcfFilePath, rcfFile);

        if (!File.Exists(jsonFilePath))
            return RcfFileParseResult.Failed($"Cannot find related json file: '{jsonFilePath}'.");

        var zRoundResult = ParseJsonFile(jsonFilePath);

        zRoundResult.Duration = rcfFile.Duration;
        zRoundResult.Description = rcfFile.Description;
        zRoundResult.Cars = rcfFile.Cars;

        return RcfFileParseResult.Success(zRoundResult, jsonFilePath);
    }

    private static RcfFile ParseRcfFile(string rcfFilePath)
    {
        var rcf = new RcfFile();

        foreach (var line in File.ReadLines(rcfFilePath))
        {
            var keyValue = line.Split(':');
            if (keyValue.Length != 2 || keyValue[0].Equals("cars"))
                continue;

            if (keyValue[0].Equals("car"))
            {
                var carParams = keyValue[1].Trim().Split(" ");
                var car = new Car
                {
                    Sort = int.TryParse(carParams[0], out var sort) ? sort : 0,
                    Id = int.TryParse(carParams[1], out var id) ? id : 0,
                    SenorNumber = carParams[2],
                    Unknown = int.TryParse(carParams[3], out var unknown) ? unknown : 0,
                    Name = new StringBuilder().AppendJoin(" ", carParams.Skip(4)).ToString()
                };
                rcf.Cars.Add(car);
            }

            InitRcfFile(rcf, keyValue[0], keyValue[1]);
        }

        return rcf;
    }

    private static ZRoundResult ParseJsonFile(string jsonFilePath)
    {
        using var reader = new StreamReader(jsonFilePath);
        var readToEnd = reader.ReadToEnd();

        var result = JsonConvert.DeserializeObject<ZRoundResult>(readToEnd);
        return result ?? throw new JsonSerializationException(readToEnd);
    }

    private static void InitRcfFile(RcfFile rcfFile, string key, string valueStr)
    {
        var propertyInfos = typeof(RcfFile).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var propertyInfo in propertyInfos)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute), true);
            if (attr is not DisplayNameAttribute displayNameAttr || !displayNameAttr.DisplayName.Equals(key))
                continue;

            object value = propertyInfo.PropertyType.Name switch
            {
                "String" => valueStr.Trim(),
                "Int32" => int.TryParse(valueStr, out var intValue) ? intValue : 0,
                "Single" => float.TryParse(valueStr, out var floatValue) ? floatValue : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            propertyInfo.SetValue(rcfFile, value);
        }
    }

    private static bool IsValidRcfFile(string filePath) => Path.GetExtension(filePath).Equals(Extension);

    private static string GetJsonFilePath(string rcfFilePath, RcfFile rcfFile) 
        => Path.Combine(Path.GetDirectoryName(rcfFilePath), $"{rcfFile.Run}.json");
}