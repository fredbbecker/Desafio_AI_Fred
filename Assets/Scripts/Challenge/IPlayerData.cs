using UnityEngine;
using System;
namespace ChallengeAI {
  public interface IPlayerData {
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
}