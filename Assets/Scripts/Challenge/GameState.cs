using UnityEngine;
namespace ChallengeAI {
  public class GameState {
    public ObservableProperty<float> Time = new ObservableProperty<float>(99f);
    public float AmmoRespawnTime {get; protected set;} = 8f;
    public int AmmoReplenish {get; protected set;} = 3;
    public int AmmoInitial {get; protected set;} = 2;
    public float EnergyRespawnTime {get; protected set;} = 8f;
    public float EnergyReplenish {get; protected set;} = 30f;
    public float EnergyPerSecond {get; protected set;} = 6f;
    public float EnergyRefillPerSecond {get; protected set;} = 5f;
    public float EnergyWaitTimeInitial {get;protected set;} = 3f;
    public float EnergyWaitTime {get;protected set;} = 1f;
    public float FireStopTime {get; protected set;} = 2f;
    public float FireDamage {get; protected set;} = 30f;
    public float PlayerSpeed {get; protected set;} = 5f;
    public float PlayerAngularSpeed {get; protected set;} = 340f;
    public float PlayerAccelaration {get; protected set;} = 12f;

  }
}