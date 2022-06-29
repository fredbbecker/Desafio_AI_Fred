using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public class EnergyPickup : Collectable<float>
  {
    public void Awake() {
      Type = CollectableType.Energy;
      Value = 30f;
    }
  }
}
