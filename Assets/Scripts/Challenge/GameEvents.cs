using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolEvent : UnityEvent<bool> {}
public class IntBoolEvent : UnityEvent<int,bool> {}
public class IntFloatEvent : UnityEvent<int,float> {}
public class IntEvent : UnityEvent<int> {}
public class IntIntEvent : UnityEvent<int,int> {}
public class IntStringEvent : UnityEvent<int,string> {}
public class StringEvent : UnityEvent<string> {}
public class TriggerEvent : UnityEvent<Component> {}

public class GameEvents
{
    static public IntFloatEvent OnHudPowerBar = new IntFloatEvent();
    static public IntIntEvent OnHudAmmo = new IntIntEvent();
    static public IntIntEvent OnHudScore = new IntIntEvent();
    static public IntEvent OnHudTime = new IntEvent();
    static public IntBoolEvent OnHudHasFlag = new IntBoolEvent();
    static public IntStringEvent OnHudConsole = new IntStringEvent();
    static public UnityEvent OnHudToggleConsole = new UnityEvent();
    static public IntStringEvent OnPlayerNameChange = new IntStringEvent();
    static public StringEvent OnWinnerName = new StringEvent();
    static public TriggerEvent OnTriggerPowerUp = new TriggerEvent();

}
