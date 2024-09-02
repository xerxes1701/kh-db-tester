public class WBinAblageUnlimited
{
    public int Id { get; set; }
    public byte[]? Data { get; set; }
}

public class WBinAblageLimited
{
    public int Id { get; set; }
    public byte[]? Data { get; set; }
    public int BlockNumber { get; set; }
}

