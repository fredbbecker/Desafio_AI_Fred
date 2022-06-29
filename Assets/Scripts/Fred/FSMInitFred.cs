using ChallengeAI;

public class FSMInitFred : FSMInitializer
{
    public override string Name => "Fred";
    public override void Init()
    {
        RegisterState<CaptureFlagFred>(FredState.CAPTURE_FLAG);
        RegisterState<WalkToFlagFred>(FredState.WALK_FLAG);
        RegisterState<WalkToRandomFred>(FredState.WALK_RANDOM);
        RegisterState<WalkToBaseFred>(FredState.WALK_BASE);
        RegisterState<RechargeStateFred>(FredState.RECHARGE);
    }
}
