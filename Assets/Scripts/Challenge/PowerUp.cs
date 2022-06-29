using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChallengeAI {
  public class PowerUp : MonoBehaviour
  {
    public float RotateSpeed = 20f;
    public float TranslateSpeed = 2f;
    public float yAmplitude = 0.08f;
    public float initialY;
    public float y;
    private void Start() {
      initialY = transform.position.y;
      transform.Rotate(0,Random.Range(0,360f),0);
    }

    void Update()
    {
      transform.Rotate(0,Time.deltaTime * RotateSpeed,0);
      y = initialY + Mathf.Sin(Time.time * TranslateSpeed) * yAmplitude;
      transform.position = new Vector3(transform.position.x,y,transform.position.z);
    }
  }
}