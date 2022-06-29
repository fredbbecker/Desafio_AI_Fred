using ChallengeAI;

public class FSMInitExample : FSMInitializer //IFSMInitializer
{
  public override string Name => "Example";
  public override void Init()
  {
    // RegisterState<IdleTestState>(ExampleState.IDLE);
    // RegisterState<WalkTestState>(ExampleState.WALK);
    RegisterState<CaptureFlagTest>(ExampleState.CAPTURE_FLAG);
    RegisterState<WalkToFlagTest>(ExampleState.WALK_FLAG);
    RegisterState<WalkToRandomTest>(ExampleState.WALK_RANDOM);
  }
}
