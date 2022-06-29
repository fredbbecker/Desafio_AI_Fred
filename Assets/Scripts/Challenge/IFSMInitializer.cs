namespace ChallengeAI {
  public interface IFSMInitializer {
    State[] GetStates(IPlayer player, FSMChangeState changeStateDelegate);
  }
}