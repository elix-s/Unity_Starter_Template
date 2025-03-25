public interface IGameState
{
    void Enter(object data = null);
    void Update();
    void Exit();
}

