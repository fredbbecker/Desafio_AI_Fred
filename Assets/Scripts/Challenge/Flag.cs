using System.Collections;
using UnityEngine;
namespace ChallengeAI {
  public delegate void OnFlagTrigger(Flag flag, int playerIndex);
  public enum FlagState {
    None, StartPosition, Catched, Dropped
  }
  public class Flag : MonoBehaviour {
    [SerializeField]
    public int Owner {get; protected set;}
    public Vector3 StartPosition {get; protected set;}
    public OnFlagTrigger OnTriggerFlag;

    private Material material;
    private Material Material {get{
      SetMaterial();
      return material;
    }}
    [SerializeField]
    private Color color;
    public Color Color {get => color; set => Material.color = color = value;}
    private new Collider collider;
    private void Awake() {
      SetStartPosition();
      SetMaterial();
      collider = GetComponent<Collider>();
    }
    private void SetStartPosition() {StartPosition = transform.position;}
    private void SetMaterial() {
      if(material == null) {
        var mr = GetComponentsInChildren<MeshRenderer>()[0];
        material = mr.material;
      }
    }
    public void Init(int OwnerIndex,OnFlagTrigger onTrigger) {
      Owner = OwnerIndex;
      OnTriggerFlag += onTrigger;
      SetStartPosition();
      SetMaterial();
    }
    private void OnTriggerEnter(Collider other) {
      var index = other.GetComponent<PlayerController>()?.PlayerIndex;
      if(index != null) {
        OnTriggerFlag?.Invoke(this,(int)index);
      }
    }
    public void Drop() {
      gameObject.transform.parent = null;
      StartCoroutine(ColliderEnable());
    }
    IEnumerator ColliderEnable() {
      collider.enabled = false;
      yield return new WaitForSeconds(2f);
      collider.enabled = true;
    }
    private void Update() {
      Material.color = Color;
    }
  }
}