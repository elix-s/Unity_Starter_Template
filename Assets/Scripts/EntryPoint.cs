using VContainer.Unity;
using Common.GameStateService;

public class EntryPoint : IStartable
{
   private readonly GameStateService _stateMachine;
   
   public EntryPoint(GameStateService stateMachine)
   {
      _stateMachine = stateMachine;
   }
   
   public void Start()
   {
      _stateMachine.ChangeState<StartLoadingState>();
   }
}
