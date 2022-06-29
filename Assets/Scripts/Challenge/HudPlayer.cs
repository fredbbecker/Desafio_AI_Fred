using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace ChallengeAI {
  public class HudPlayer : MonoBehaviour
  {
    public int PlayerIndex;
    public HudPowerBar energy;
    public TMP_Text[] texts;

    public TMP_Text[] Texts {get {
      if(texts.Length == 0) {
        texts = GetComponentsInChildren<TMP_Text>();
      }
      return texts;
    }}

    public GameObject ConsolePanel {
      get { 
        return Texts[3]?.transform.parent.gameObject;
      }
    }

    private void Awake() {
      energy = GetComponentInChildren<HudPowerBar>();
      texts = GetComponentsInChildren<TMP_Text>();
      // Debug.Log($"HudPlayer::Awake Texts {texts.Length}");
    }
    public void ToggleConsole() {
      var p = ConsolePanel;
      p?.SetActive(!p.activeSelf);
    }
    public void AddEvents() {
      GameEvents.OnPlayerNameChange.AddListener((index,name) => {
        if(index == PlayerIndex) {
          Texts[0].text = name;
        }
      });
      GameEvents.OnHudAmmo.AddListener((index,ammo) => {
        // Debug.Log($"Hud {index} {ammo} is {index == PlayerIndex} {this.PlayerIndex}");
        if(index == PlayerIndex) {
          Texts[1].text = ammo.ToString();
        }
      });
      GameEvents.OnHudPowerBar.AddListener((index,energyRatio) => {
        if(index == PlayerIndex) {
          energy.Ratio = energyRatio;
        }
      });
      GameEvents.OnHudScore.AddListener((index,score) => {
        // Debug.Log($"Hud {index} {score} is {index == PlayerIndex} {this.PlayerIndex}");
        if(index == PlayerIndex) {
          string s = "";
          for (int i = 0; i < score; i++) s += "I";
          Texts[2].text = s;
        }
      });
      GameEvents.OnHudConsole.AddListener((index,text) => {
        if(index == PlayerIndex) {
          Texts[3].text = text;
        }
      });
      GameEvents.OnHudToggleConsole.AddListener(() => {ToggleConsole();});

      ToggleConsole();
    }
  }
}