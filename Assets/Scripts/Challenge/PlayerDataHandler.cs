using System;
using UnityEngine;

namespace ChallengeAI {
  public class PlayerDataDelegates {
    public Func<int> PlayerIndex;
    public Func<float> Energy;
    public Func<float> Speed;
    public Func<int> Ammo;
    public Func<Vector3> Position;
    public Func<Quaternion> Rotation;
    public Func<float> RemainingDistance;
    public Func<Nullable<Vector3>> FlagPosition;
    public Func<FlagState> FlagState;
    public Func<Vector3[]> PowerUps;
    public Func<Vector3[]> AmmoRefill;
    public Func<Vector3> StartPosition;
    public Func<bool> HasFlag;
    public Func<float> FlagDistance;
    public Func<bool> IsCooldownFire;
    public Func<bool> HasSightEnemy;
    // public Func<object> EnemyInfo;
  }
  public class PlayerDataHandler : IPlayerData
  {
    private PlayerDataDelegates delegates;
    public PlayerDataHandler(PlayerDataDelegates delegates) {
      this.delegates = delegates;
    }
    public int PlayerIndex => delegates.PlayerIndex();
    public float Energy => delegates.Energy();
    public float Speed => delegates.Speed();
    public int Ammo => delegates.Ammo();
    public Vector3 Position => delegates.Position();
    public Quaternion Rotation => delegates.Rotation();
    public float RemainingDistance => delegates.RemainingDistance();
    public Nullable<Vector3> FlagPosition => delegates.FlagPosition();
    public FlagState FlagState => delegates.FlagState();
    public Vector3[] PowerUps => delegates.PowerUps();
    public Vector3[] AmmoRefill => delegates.AmmoRefill();
    public Vector3 StartPosition => delegates.StartPosition();
    public bool HasFlag => delegates.HasFlag();
    public float FlagDistance => delegates.FlagDistance();
    public bool IsCooldownFire => delegates.IsCooldownFire();
    public bool HasSightEnemy => delegates.HasSightEnemy();
    // public object EnemyInfo => throw new System.NotImplementedException();
  }

  public class PlayerContainer {
    public PlayerContainer(int index, Player player, PlayerDataDelegates delegates, PlayerDataHandler dataHandler, PlayerState state, PlayerController controller) {
      Index = index;
      Player = player;
      Delegates = delegates;
      DataHandler = dataHandler;
      State = state;
      Controller = controller;
    }
    public int Index {get; protected set;}
    public Player Player {get; protected set;}
    public PlayerDataDelegates Delegates {get; protected set;}
    public PlayerDataHandler DataHandler {get; protected set;}
    public PlayerState State {get; protected set;}
    public PlayerController Controller {get; protected set;}
  }
}