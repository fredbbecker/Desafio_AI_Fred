using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public enum CollectableType {
    None, Energy, Ammo
  }
  public delegate void OnCollectableTrigger(Collectable item, int playerIndex,CollectableType type, object itemValue);
  public abstract class Collectable : MonoBehaviour {
    public OnCollectableTrigger OnTriggerCollect;
    public CollectableType Type {get; protected set;}
  }
  public abstract class Collectable<T> : Collectable
  {
    public T Value {get; protected set;}
    public void OnTriggerEnter(Collider other) {
      var index = other.GetComponent<PlayerController>()?.PlayerIndex;
      if(index != null) {
        OnTriggerCollect?.Invoke(this,(int)index,Type,Value);
      }
    }
  }
}
