using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace RaceResultConverter;

public class RcfFileParser
{
    public string Extension => ".rcf";

    public RcfFile ParseRaceResult(string filePath)
    {
        if (!File.Exists(filePath) || !IsValidRcfFile(filePath))
            return null;

        var rcf = new RcfFile();
        foreach (var line in File.ReadLines(filePath))
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

    private void InitRcfFile(RcfFile rcfFile, string key, string valueStr)
    {
        var propertyInfos = typeof(RcfFile).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var propertyInfo in propertyInfos)
        {
            var attr = propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute), true);
            if (attr is not  DisplayNameAttribute displayNameAttr || !displayNameAttr.DisplayName.Equals(key))
                continue;
            
            object value = propertyInfo.PropertyType.Name switch
            {
                "String" => valueStr.Trim(),
                "Int32" => int.TryParse(valueStr, out var intValue) ? intValue : 0,
                "Single"=> float.TryParse(valueStr, out var floatValue) ? floatValue : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
            propertyInfo.SetValue(rcfFile, value);
        }
    }

    private bool IsValidRcfFile(string filePath) => Path.GetExtension(filePath).Equals(Extension);
}