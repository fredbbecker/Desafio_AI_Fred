using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public class AmmoPickup : Collectable<int>
  {
    private void Awake() {
      Type = CollectableType.Ammo;
      Value = 2;
    }
  }
}
