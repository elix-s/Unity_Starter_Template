public interface IGameState
{
    void Enter(StatePayload payload = null);
    void Update();
    void Exit();
}

