using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public class LaserController : MonoBehaviour {
    public static float Duration = 0.15f;
    [SerializeField]
    private List<LineRenderer> lines = new List<LineRenderer>();
    private void Awake() {
      lines.AddRange(GetComponentsInChildren<LineRenderer>());
      gameObject.SetActive(false);
    }
    public void SetPoints(params Vector3[] ps) {
      foreach (var line in lines) {
        line.SetPositions(ps);
      }
    }
    public void Fire() {
      gameObject.SetActive(true);
      StartCoroutine(Activate());
    }
    private IEnumerator Activate() {
      gameObject.SetActive(true);
      yield return new WaitForSeconds(LaserController.Duration);
      gameObject.SetActive(false);
    }
  }
}