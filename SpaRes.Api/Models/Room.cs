public class Room
{
    public int RoomId { get; set; }
    public int LocationId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; } = 1;
    public bool OutOfService { get; set; }
}