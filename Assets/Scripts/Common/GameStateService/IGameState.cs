using Cysharp.Threading.Tasks;

public interface IGameState
{
    UniTask Enter(StatePayload payload = null);
    UniTask Exit();
    void Update();
}

