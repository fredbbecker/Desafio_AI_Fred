using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChallengeAI {
  public class Arena : MonoBehaviour
  {
    public GameObject block;
    public GameObject wall;
    public GameObject energyPickup;
    public GameObject ammoPickup;
    public GameObject flagPrefab;
    public GameObject startPoint;
    private GameObject goArena;
    private int width = 40;
    private Vector2 offset = new Vector2(-19.5f,-19.5f);
    private List<float> prefabY = new List<float>();
    private List<Vector3> powerUps = new List<Vector3>() {
      new Vector3(-19,0.5f,0),
      new Vector3(19,0.5f,0),
    };

    static private float ammunitionOffeset = 6f;
    private List<Vector3> ammunition = new List<Vector3>() {
      new Vector3(0,0.5f,0),
      new Vector3(-ammunitionOffeset,0.5f, -ammunitionOffeset),
      new Vector3( ammunitionOffeset,0.5f, -ammunitionOffeset+1),
      new Vector3(-ammunitionOffeset,0.5f,  ammunitionOffeset-1),
      new Vector3( ammunitionOffeset,0.5f,  ammunitionOffeset),
    };
    public List<Vector3> ammoSpots {get => ammunition;}

    public List<Vector3> startPoints {get; protected set;} = new List<Vector3>() {
      new Vector3(0,0,19),
      new Vector3(0,0,-19),
    };
    public List<Quaternion> startRotations {get; protected set;} = new List<Quaternion>() {
      Quaternion.Euler(0,180,0),
      Quaternion.identity,
    };
    public List<Vector3> flagInitalPoints {get; protected set;} = new List<Vector3>() {
      new Vector3(0,0,19),
      new Vector3(0,0,-19),
    };


    private List<int> map = new List<int>() {
      1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,2,2,2,2,0,0,1,
      1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,1,
      1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,1,
      1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,1,
      1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,0,0,0,0,2,2,2,2,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
      1,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,
      1,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
      1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    };

    private void Awake() {
      prefabY.Add(0);
      block = Resources.Load<GameObject>("Prefabs/Block");
      prefabY.Add(0.5f);
      wall = Resources.Load<GameObject>("Prefabs/Wall");
      prefabY.Add(1.5f);
      energyPickup = Resources.Load<GameObject>("Prefabs/EnergyPickup");
      ammoPickup = Resources.Load<GameObject>("Prefabs/AmmoPickup");
      startPoint = Resources.Load<GameObject>("Prefabs/StartPoint");
      flagPrefab = Resources.Load<GameObject>("Prefabs/Flag");
      
      // GenerateArena();
      for (int i = 0; i < map.Count; i++)
      {
        int row = (int)(i / width);
        int col = (int)(i%width);
        if(row <= 2 || row >= width - 3 ||
           col <= 2 || col >= width - 3
        ) {
          map[i] = 0;
        } else {
          map[i] = 2;
        }
      }
      // GenerateArena();
      if(goArena == null) {
        goArena = GameObject.Find("Arena");
      }
      GeneratePowerUps();
      //GenerateStartPoints();
      //GenerateFlags();
    }

    private void GenerateStartPoints()
    {
      foreach (var point in startPoints)
      {
        Instantiate(startPoint,point,Quaternion.identity);
      }
    }

    private void GeneratePowerUps()
    {
      foreach (var point in powerUps)
      {
        Instantiate(energyPickup,point,Quaternion.identity,goArena.transform);
      }
      foreach (var point in ammunition)
      {
        Instantiate(ammoPickup,point,Quaternion.identity,goArena.transform);
      }
    }

    private void GenerateFlags() {
      foreach (var point in flagInitalPoints)
      {
        Instantiate(flagPrefab,point,Quaternion.identity,goArena.transform);
      }
    }

    private void GenerateArena()
    { 
      goArena = new GameObject("Arena");
      GameObject goBlock = new GameObject(block.name);
      GameObject goWall = new GameObject(wall.name);
      Transform parent = goArena.transform;
      goBlock.transform.parent = parent;
      goWall.transform.parent = parent;
      for (int i = 0; i < map.Count; i++)
      {
        if(map[i] == 0) {
          continue;
        }
        GameObject prefab = null;
        if(map[i] == 1) {
          prefab = block;
          parent = goBlock.transform;
        } else if(map[i] == 2) {
          prefab = wall;
          parent = goWall.transform;
        }
        if(prefab != null) {
          var go = Instantiate(prefab,new Vector3(offset.x+i%width,prefabY[map[i]],offset.y+(int)(i/width)),Quaternion.identity,parent);
          go.name = $"{prefab.name}_{i}";
        } else {
          Debug.Log($"Prefab {prefab} {i} {map[i]}");
        }
      }
    }

    private void _Update() {
      if(Input.GetKeyDown(KeyCode.G)) {
        var count = goArena.transform.childCount;
        List<int> list = new List<int>();
        for (int i = 0; i < count; i++)
        {
          Transform t = goArena.transform.GetChild(i);
          list.Add(
            t.gameObject.activeSelf ? 1 : 0
          );
        }
      }
    }
  }
}
