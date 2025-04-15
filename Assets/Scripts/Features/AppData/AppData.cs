public class AppData : IVersionedData
{
    public int Version { get; set; } = 1;
    public int Scores { get; set; }
    public string UserName { get; set; }
    public bool IsAuthenticated { get; set; }
}