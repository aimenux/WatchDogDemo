namespace WayTwo;

public sealed record Settings
{
    public bool IsWatchDogEnabled { get; init; } = true;
    public string WatchPageUsername { get; init; } = "admin";
    public string WatchPagePassword { get; init; } = "admin";
}