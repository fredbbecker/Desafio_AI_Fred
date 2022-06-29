using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace ChallengeAI
{
  public class FSM
  {
    public State CurrentState { get; protected set; }
    private State nextState;
    public Dictionary<string, State> stateList = new Dictionary<string, State>();
    private float currentTime;
    private float lastTime;
    private float deltaTime;
    public bool Started {get; protected set;} = false;
    public int PlayerIndex {get; protected set;} = -1;
    public string Name {get; protected set;}
    private IPlayer player;
    public FSM(int playerIndex, IPlayer player, FSMInitializer initializer) {
      PlayerIndex = playerIndex;
      Name = initializer.Name;
      var states = initializer.GetStates(player,ChangeState);
      Debug.Log($"FSM p{playerIndex} States:{states.Length}");
      this.player = player;
      AddStates(states);
    }
    private void Awake()
    {
      Started = false;
    }

    public void StartFSM() {
      var list = stateList.ToList();
      Debug.Log($"FSM '{Name}' START with {stateList.Count} states");
      string s = "";
      foreach (var item in list)
      {
        s += $"State {item.Key} ";
      }
      Debug.Log(s);
      var initialState = stateList.ToList().ElementAtOrDefault(0).Value;
      if(initialState != null) {
        StartFSM(initialState);
      }
    }
    public void StartFSM(State initialState)
    {
      CurrentState = initialState;
      CurrentState.Phase = StatePhase.Enter;
      Started = true;
    }
    public void StopFSM() {
      player.Stop();
      Started = false;
    }

    public void AddState(State newState) {
      stateList.Add(newState.Name,newState);
    }
    public void AddStates(State[] newStates) {
      foreach (var state in newStates) {
        stateList.Add(state.Name,state);
      }
    }

    public void ChangeState(string stateName) {
      State newState;
      if(stateList.TryGetValue(stateName,out newState)) {
        ChangeState(newState);
      } else {
        UnityEngine.Debug.Break();
        throw new System.Exception($"State {stateName} doesn't exists.");
      }
    }

    public void ChangeState(State state)
    {
      if(CurrentState != null)
      {
        CurrentState.Phase = StatePhase.Exit;
      }
      nextState = state;
    }

    public void Update(float deltaTime) {
      if(Started) {
        Loop(deltaTime);
      }
    }

    void Loop(float deltaTime)
    {
      switch (CurrentState?.Phase)
      {
        case StatePhase.Enter:
          CurrentState.Enter();
          CurrentState.Phase = StatePhase.Update;
          break;
        case StatePhase.Update:
          CurrentState.Update(deltaTime);
          break;
        case StatePhase.Exit:
          CurrentState.Exit();
          CurrentState = nextState;
          CurrentState.Phase = StatePhase.Enter;
          break;
      }
    }
  }
}