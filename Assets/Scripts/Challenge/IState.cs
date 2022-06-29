namespace ChallengeAI {
  public interface IState {
    // string Name {get; protected set;}
    StatePhase Phase {get; set;}
    void Enter();
    void Exit();
    void Update(float deltaTime);

    FSMChangeState ChangeState {get;}
  }
}