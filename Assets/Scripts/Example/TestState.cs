using ChallengeAI;
public class TestState : State {
  public TestState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate) {}

  public override void Enter()
  {
    // Implementar lógica de entrada do State
    throw new System.NotImplementedException();
  }

  public override void Exit()
  {
    // Implementar lógica de update do State
    throw new System.NotImplementedException();
  }

  public override void Update(float deltaTime)
  {
    // Implementar lógica de saída do State
    throw new System.NotImplementedException();
  }
}
