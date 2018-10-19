
namespace MyApplication.Operations
{
    /// <summary>
    /// Defines custom operation codes
    /// </summary>
    public enum MyOperationCodes : byte 
    {
        EchoOperation = 100,
        GameOperation = 101,
    }

    /// <summary>
    /// Defines custom paramter codes
    /// </summary>
    public enum MyParameterCodes : byte
    {
        Text = 100,
        Response = 101,
    }
}
