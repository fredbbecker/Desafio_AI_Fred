using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChallengeAI
{
  public enum StatePhase {
    Enter, Update, Exit
  }
  public enum AIState
  {
      None,
      Idle,
      Walk,
      Chase,
      Return
  }

  public delegate void FSMChangeState(string newState);
  public delegate void StateChangePhase(StatePhase newPhase);

  public abstract class State : IState
  {
    public string Name {get; protected set;}
    public StatePhase Phase {get; set;}
    public IPlayer Agent { get; protected set; }
    private FSMChangeState _changeState;
    public FSMChangeState ChangeState { get => _changeState; }

    public State(string name, IPlayer player, FSMChangeState changeStateDelegate)
    {
      Name = name;
      Agent = player;
      _changeState = changeStateDelegate;
    }

    public void Init(AgentContainer container, FSMChangeState changeStateDelegate) {
      //Agent = container;
      _changeState = changeStateDelegate;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(float deltaTime);

    public virtual string InfoString {get => $"State:{Name} Phase:{Phase}";}
    public virtual void Log(params string[] messages) {
      string msg;
      if(messages != null && messages.Length > 0) {
        List<string> strs = new List<string>();
        strs.Add(InfoString);
        strs.AddRange(messages);
        msg = string.Join(" ",strs);
      } else {
        msg = InfoString;
      }
      Debug.Log(msg);
    }

    public override string ToString()
    {
      return $"State:{Name} phase:{Phase}";
    }
  }

}

