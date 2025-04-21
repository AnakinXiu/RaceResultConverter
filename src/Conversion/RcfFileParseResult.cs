using RaceResultConverter.DTO.ZRound;

namespace RaceResultConverter.Conversion;

public class RcfFileParseResult
{
    public static RcfFileParseResult Failed(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };

    public static RcfFileParseResult Success(ZRoundResult result) => new() { IsValid = true, ZRoundResult = result };

    private RcfFileParseResult()
    { }

    public bool IsValid { get; private set; }

    public string ErrorMessage { get; private set; } = string.Empty;

    public ZRoundResult ZRoundResult { get; private set; }
}