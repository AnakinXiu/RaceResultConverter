using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace RaceResultConverter;

public static class JsonUtil
{
    public static bool SaveAsJson(IRaceResult result, string outputPath)
    {
        try
        {
            if (result == null)
                return false;

            var s = JsonConvert.SerializeObject(result);
            using var file = new FileStream(outputPath, FileMode.OpenOrCreate);
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
}