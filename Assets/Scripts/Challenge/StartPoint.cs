using UnityEngine;
namespace ChallengeAI {
  public class StartPoint : TriggerWithPlayer {
    private Color color;
    public Color Color {
      get => color;
      set {
        color = value;
        var rs = GetComponentsInChildren<MeshRenderer>();
        foreach (var r in rs)
        {
          r.material.color = color;
        }
      }
    }
  }
}