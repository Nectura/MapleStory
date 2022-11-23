namespace Common.Networking.Abstract.Interfaces;

public interface IGameServer
{
    bool IsRunning { get; }
    ValueTask DisposeAsync();
    void Start();
}