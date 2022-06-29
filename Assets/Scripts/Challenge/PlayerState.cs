using UnityEngine;
using UnityEngine.Events;
namespace ChallengeAI {
  public enum PlayerStateAttribute {
    None,
    Index,
    Energy,
    Speed,
    Ammo,
    HasFlag,
    IsCooldownFire,
    HasSightEnemy
  }

  [System.Serializable]
  public class PlayerState {
    public const float ENERGY_MAX = 100f;
    private PlayerStateDelegates delegates;
    public int PlayerIndex {get; set;}
    public string Name {get; set;}
    public ObservableProperty<int> Points = new ObservableProperty<int>();
    public ObservableProperty<float> Energy = new ObservableProperty<float>((energy) => Mathf.Clamp(energy,0,ENERGY_MAX));
    public float EnergyMax {get; set;}
    public ObservableProperty<float> Speed = new ObservableProperty<float>();
    // public Vector3 FlagPosition {get; set;}
    // public Vector3[] PowerUps {get; set;}
    public ObservableProperty<int> Ammo = new ObservableProperty<int>();
    public ObservableProperty<bool> HasFlag = new ObservableProperty<bool>();
    // public float FlagDistance {get; set;}
    public bool IsCooldownFire {get; set;}
    // public bool HasSightEnemy {get; set;}
    public ObservableProperty<bool> HasSightEnemy = new ObservableProperty<bool>();
    public bool IsMoving {get; set;}
    public float WaitTimeTotal {get; set;}
    public float WaitTimeStep {get; set;}
    private PlayerController controller;
    public Transform Transform {get; protected set;}
    public Vector3 LastPosition {get; set;}
    public Vector3 CurrentPosition {get; set;}
    public FlagState FlagState {get; set;}
    public Transform FlagTransform {get; set;}

    public PlayerState(int playerIndex,PlayerController controller) {
      PlayerIndex = playerIndex;
      this.controller = controller;
      SetInitialValues();
    }
    public PlayerState(int playerIndex,PlayerController controller,PlayerStateDelegates delegates) {
      PlayerIndex = playerIndex;
      this.controller = controller;
      SetInitialValues();
      this.delegates = delegates;
    }

    private void SetInitialValues() {
      Transform = controller.GetComponent<Transform>();
      CurrentPosition = LastPosition = Transform.position;
      Points.Value = 0;
      Energy.Value = 100;
      EnergyMax = ENERGY_MAX;
      Speed.Value = 8f;
      // Ammo.Value = 3;
      HasFlag.Value = false;
      IsCooldownFire = false;
      HasSightEnemy.Value = false;
      WaitTimeTotal = 0;
      WaitTimeStep = 0;
      FlagState = FlagState.StartPosition;
    }
    public new string ToString() {
      string str = "";
      str += $"PlayerIndex {PlayerIndex}\n";
      str += $"Points {Points}\n";
      str += $"Energy {Energy}\n";
      str += $"Speed {Speed}\n";
      str += $"Ammo {Ammo}\n";
      str += $"HasFlag {HasFlag}\n";
      str += $"IsCooldownFire {IsCooldownFire}\n";
      str += $"HasSightEnemy {HasSightEnemy}\n";
      str += $"IsMoving {IsMoving}\n";
      str += $"CurrentPosition {CurrentPosition}\n";
      str += $"FlagState {FlagState}";
      return str;
    }
  }
  public class PlayerStateDelegates {
    public UnityAction<float> OnEnergyChange;
    public UnityAction<float> OnSpeedChange;
    public UnityAction<int> OnAmmoChange;
    public UnityAction<bool> OnHasFlagChange;
    public UnityAction<bool> OnIsCooldownFireChange;
    public UnityAction<bool> OnHasSightEnemyChange;
  }
}