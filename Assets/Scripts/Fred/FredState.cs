using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChallengeAI;

public class FredState
{
    static public string IDLE = "idle";
    static public string WALK = "walk";
    static public string CAPTURE_FLAG = "capture_flag";
    static public string WALK_FLAG = "walk_flag";
    static public string WALK_BASE = "walk_Base";
    static public string WALK_RANDOM = "walk_random";
    static public string RECHARGE = "recharge";
    static public string[] StateNames = new string[] {
    // IDLE,
    CAPTURE_FLAG,   
    WALK_FLAG,
    WALK_BASE,
    WALK_RANDOM
  };

}


/*
public class TestState : State {
  public TestState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name,player,changeStateDelegate)
  public override void Enter()
  public override void Exit()
  public override void Update(float deltaTime)
}
*/

/*
    Data
    int PlayerIndex {get;}
    float Energy {get;}
    float Speed {get;}
    int Ammo {get;}
    Vector3 Position {get;}
    Quaternion Rotation {get;}
    float RemainingDistance {get;}
    Nullable<Vector3> FlagPosition {get;}
    FlagState FlagState {get;}
    Vector3[] PowerUps {get;}
    Vector3[] AmmoRefill {get;}
    Vector3 StartPosition {get;}
    bool HasFlag {get;}
    float FlagDistance {get;}
    bool IsCooldownFire {get;}
    bool HasSightEnemy {get;}
  }
*/



public class IdleStateFred : State {
  public IdleStateFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
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
      var nextState = FredState.StateNames.GetRandomItem();
      ChangeState(nextState);
      Log($"NextState:{nextState}");
    }
  }
}

public class WalkStateFred : State {
  public WalkStateFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public Vector3 Destination { get; set; } = Vector3.zero;
  public override void Enter() {
    Agent.Move(Destination);
    Log($"Destination:{Destination}");
  }
  public override void Exit() {
    Log();
  }
  public override void Update(float deltaTime) 
  {
        if(Agent.Data.Energy < 0.25f)
        {
            ChangeState(FredState.RECHARGE);
        }

    if(Vector3.Distance(Agent.Data.Position,Destination) <= 1.17f  || Agent.Data.RemainingDistance <= 0.05f) 
    {
        Debug.Log(Agent.Data.HasFlag);

        var nextState = FredState.StateNames.GetRandomItem();

        if(Agent.Data.HasFlag)
            nextState = FredState.WALK_BASE;

        ChangeState(nextState);
        Log($"NextState:{nextState}");
    }
  }
}

public class CaptureFlagFred : WalkTestState {
  public CaptureFlagFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() 
  {
    Destination = (Vector3)Agent.EnemyData[0].FlagPosition;
    // Destination = (Vector3)Agent.Data.AmmoRefill.ElementAtOrDefault(0);
    // Destination = (Vector3)Agent.Data.PowerUps.ElementAtOrDefault(0);
    Log($"Flag Destination {Destination}");
    base.Enter();
  }
  public override void Update(float deltaTime)
  {
      if(Agent.Data.HasSightEnemy)// && Agent.EnemyData[0].HasFlag)
      {
          if(Agent.Data.Ammo > 0)
          {
                Agent.Fire();
                Agent.Move(Destination);
          }
      }
        

    if(Agent.Data.RemainingDistance <= 0.05f) 
    {
        var nextState = Agent.Data.HasFlag ? FredState.WALK_BASE : FredState.CAPTURE_FLAG;

        if(Agent.Data.Energy < 50.0f)
            nextState = FredState.RECHARGE;

        ChangeState(nextState);

        Log($"NextState:{nextState}");
    }
  }
}

public class WalkToFlagFred : WalkStateFred {
  public WalkToFlagFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() 
  {
    Destination = (Vector3)Agent.Data.FlagPosition;

    Log($"Flag Destination {Destination}");
    base.Enter();
  }
}

public class WalkToBaseFred : WalkStateFred 
{
  public WalkToBaseFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter()
    {
        Destination = (Vector3)Agent.Data.StartPosition;
        Log($"Base Destination {Destination}");
        base.Enter();
    }

    public override void Update(float deltaTime) 
    {
        if(Agent.Data.HasSightEnemy && Agent.EnemyData[0].HasFlag)
        {
            if(Agent.Data.Ammo > 0)
            {
                    Agent.Fire();
                    Agent.Move(Destination);
            }
        }


        float dist = Vector3.Distance(Agent.Data.Position, Agent.Data.StartPosition); 
        Log($"Dist:{dist}");
        if(Agent.Data.Energy < dist)
        {
            ChangeState(FredState.RECHARGE);
            Log($"NextState:{FredState.RECHARGE}");
        }

        if(Agent.Data.RemainingDistance <= 0.05f) 
        {
            var nextState = Agent.Data.HasFlag ? FredState.WALK_BASE : FredState.CAPTURE_FLAG;

            if(Agent.Data.Energy < 50.0f)
                nextState = FredState.RECHARGE;

            ChangeState(nextState);

            Log($"NextState:{nextState}");
        }

    }
}


public class WalkToRandomFred : WalkStateFred {
  public WalkToRandomFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
  public override void Enter() {
    Destination = new Vector3(Random.Range(-18f,18),0,Random.Range(-18f,18));
    base.Enter();
  }
}


public class RechargeStateFred : State 
{
    float rechargeAmount = 1.0f;
    public RechargeStateFred(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter() 
    {
        //Log($"Dist: {Agent.Data.RemainingDistance}");
        //Agent.Move(Agent.Data.Position);
        Agent.Stop();

        rechargeAmount = Vector3.Distance(Agent.Data.Position, Agent.Data.StartPosition); 
    }
    public override void Exit() {
        Log();
    }
    public override void Update(float deltaTime) 
    {
        //Log($"RECHARGING {Agent.Data.Energy} ");

        if(Agent.Data.Energy >= 50.0f) 
        {
            //Log($"RECHARGED");

            if(Agent.Data.HasFlag)
                ChangeState(FredState.WALK_BASE);
            else
                ChangeState(FredState.CAPTURE_FLAG);
        }
  }
}