using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
namespace ChallengeAI {
  public class PlayerController : MonoBehaviour
  {
    private Transform tf;
    private NavMeshAgent nma;
    private LineRenderer sight;
    public int PlayerIndex {get; protected set;} = -1;
    private System.Action<int> fire;
    private float sightDistance = 1f;
    public float SightDistance {
      get => sightDistance;
      set {
        sightDistance = value;
        sight?.SetPosition(1,Vector3.forward*sightDistance);
      }
    }
    private float alphaEnd = 12f/255f;
    private float alphaStart = 48f/255f;
    private Color sightTarget0 = new Color(0,0.8f,0,220f/255f);
    private Color sightTarget1 = Color.green.Alpha(48f/255f);
    private Color sightColor = Color.white;
    private bool hasTarget;
    public bool HasTarget {
      get => hasTarget;
      set {
        hasTarget = value;
        sight.startColor = hasTarget? sightTarget0 : sightColor.Alpha(alphaStart);
        sight.endColor = hasTarget? sightTarget1 : sightColor.Alpha(alphaEnd);
      }
    }
    private Color color;
    public Color Color {
      get => color;
      set {
        color = value;
        var r = GetComponentsInChildren<Renderer>().Where(r => r.gameObject.name == "Body").First();
        r.material.color = color;
      }
    }
    public float RemainingDistance {get => nma.remainingDistance;}
    private void Awake() {
      tf = GetComponent<Transform>();
      nma= GetComponent<NavMeshAgent>();
      sight = GetComponentInChildren<LineRenderer>();
    }

    public void SetPlayerIndex(int index) {
      if(PlayerIndex == -1) {
        PlayerIndex = index;
      }
    }
    public void SetPlayerDelegates(System.Action<int> fireDelegate) {
      if(fire == null) {
        fire = fireDelegate;
      }
    }
    public void MoveToDestination(Vector3 destination) {
      if(_rotation != null) StopCoroutine(_rotation);
      nma.SetDestination(destination);
      nma.isStopped = false;
      
    }
    Coroutine _rotation;
    public void Rotate(float angle) {
      Stop();
      _rotation = StartCoroutine(Rotation(angle));
    }
    IEnumerator Rotation(float targetAngle) {
      float currentVelocity = 0;
      float diff = 9999;
      while(diff > 0.008f) {
        diff = Mathf.Abs(Mathf.DeltaAngle(targetAngle,tf.rotation.eulerAngles.y));
        var currentAngle = tf.rotation.eulerAngles.y;
        var newAngle = Mathf.SmoothDampAngle(currentAngle,targetAngle,ref currentVelocity,0.2f,40);
        tf.rotation = Quaternion.Euler(0,newAngle,0);
        yield return null;
      }
    }
    public void Stop() {
      nma.isStopped = true;
      if(_rotation != null) StopCoroutine(_rotation);
    }
    public void Fire() {
      Stop();
      // Fire Logic
      fire?.Invoke(PlayerIndex);
    }
  }
}