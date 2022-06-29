using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChallengeAI;

public class ExampleState
{
  static public string IDLE = "idle";
  static public string WALK = "walk";
  static public string CAPTURE_FLAG = "capture_flag";
  static public string WALK_FLAG = "walk_flag";
  static public string WALK_RANDOM = "walk_random";
  static public string[] StateNames = new string[] {
    // IDLE,
    CAPTURE_FLAG,
    WALK_FLAG,
    WALK_RANDOM
  };
}

public class IdleTestState : State {
  public IdleTestState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  private float waitingTime;
  public override void Enter() {
    waitingTime = Random.Range(1f,3f);
    Log($"waitingTime:{waitingTime}");
  }
  public override void Exit() {
    Log();
  }
  public override void Update(float deltaTime) {
    waitingTime -= deltaTime;
    if(waitingTime <= 0) {
      var nextState = ExampleState.StateNames.GetRandomItem();
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
  }
}

public class WalkTestState : State {
  public WalkTestState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public Vector3 Destination { get; set; } = Vector3.zero;
  public override void Enter() {
    Agent.Move(Destination);
    Log($"Destination:{Destination}");
  }
  public override void Exit() {
    Log();
  }
  public override void Update(float deltaTime) {
    if(Vector3.Distance(Agent.Data.Position,Destination) <= 1.17f
      || Agent.Data.RemainingDistance <= 0.05f
    ) {
      var nextState = ExampleState.StateNames.GetRandomItem();
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
  }
}

public class CaptureFlagTest : WalkTestState {
  public CaptureFlagTest(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() {
    Destination = (Vector3)Agent.EnemyData[0].FlagPosition;
    // Destination = (Vector3)Agent.Data.AmmoRefill.ElementAtOrDefault(0);
    // Destination = (Vector3)Agent.Data.PowerUps.ElementAtOrDefault(0);
    Log($"Flag Destination {Destination}");
    base.Enter();
  }
  public override void Update(float deltaTime)
  {
    if(Agent.Data.RemainingDistance <= 0.05f) {
      var nextState = ExampleState.CAPTURE_FLAG;
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
  }
}

public class WalkToFlagTest : WalkTestState {
  public WalkToFlagTest(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() {
    Destination = (Vector3)Agent.Data.FlagPosition;
    Log($"Flag Destination {Destination}");
    base.Enter();
  }
}

public class WalkToRandomTest : WalkTestState {
  public WalkToRandomTest(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() {
    Destination = new Vector3(Random.Range(-18f,18),0,Random.Range(-18f,18));
    base.Enter();
  }
}