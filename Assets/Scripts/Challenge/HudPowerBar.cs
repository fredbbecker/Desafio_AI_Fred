using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ChallengeAI {
  public class HudPowerBar : MonoBehaviour
  {
    public int PlayerIndex {get; set;}
    public Image PowerBar;
    public Image PowerBar2;
    public float WidthMax;
    // public float Ratio {get; set;} = 1f;
    public float Ratio = 1f;
    public float p1t = 0.01f;
    private RectTransform rectTransform;

    private void Awake()
    {
      WidthMax = PowerBar.rectTransform.rect.width;
      rectTransform = PowerBar.transform as RectTransform;
    }

    private void Update()
    {
      PowerBar.fillAmount = Mathf.Lerp(PowerBar.fillAmount,Ratio,p1t);  
    }
  }
}
