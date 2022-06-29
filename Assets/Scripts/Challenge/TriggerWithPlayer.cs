using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public delegate void OnTriggerWithPlayer(Component trigger,int playerIndex);
  public class TriggerWithPlayer : MonoBehaviour {
    [SerializeField]
    public int Owner {get; protected set;}
    public OnTriggerWithPlayer OnPlayerTrigger;
    protected void Awake() {
      List<Collider> colliders = new List<Collider>();
      colliders.AddRange(GetComponents<Collider>());
      colliders.AddRange(GetComponentsInChildren<Collider>());
      foreach (var collider in colliders)
      {
        collider.isTrigger = true;
      }
    }
    public void Init(int OwnerIndex,OnTriggerWithPlayer onTrigger) {
      Owner = OwnerIndex;
      OnPlayerTrigger += onTrigger;
    }
    private void OnTriggerEnter(Collider other) {
      var index = other.GetComponent<PlayerController>()?.PlayerIndex;
      if(index != null) {
        OnPlayerTrigger?.Invoke(this,(int)index);
      }
    }
  }
}