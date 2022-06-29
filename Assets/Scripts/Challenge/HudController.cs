using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
namespace ChallengeAI {
  public class HudController : MonoBehaviour
  {
    public TMP_Text Timer;
    public TMP_Text Winner;
    private HudPowerBar[] energyBars;
    private void Awake() {
      var texts = transform.GetComponentsInChildren<TMP_Text>();
      Timer = texts.Where(t => {
        return t.gameObject.name == "Timer";
      }).First();
      Winner = texts.Where(t => {
        return t.gameObject.name == "Wins";
      }).First();
      Winner.gameObject.SetActive(false);

      energyBars = GetComponentsInChildren<HudPowerBar>();

      GameEvents.OnHudTime.AddListener((time) => {
        Timer.text = Mathf.Max(time,0).ToString("00");
      });

      GameEvents.OnWinnerName.AddListener((name) => {
        Winner.text = $"{name} wins!";
        Winner.gameObject.SetActive(true);
      });
    }
  }
}
