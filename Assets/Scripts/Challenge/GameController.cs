using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
namespace ChallengeAI
{
  public enum GamePhase {
    None, Menu, Battle, Battle1, End
  }
  public class GameController : MonoBehaviour
  {
    static public GameController Instance {get; protected set;}
    private Arena arena;
    private ObservableProperty<GamePhase> gamePhase = new ObservableProperty<GamePhase>();
    private int playersCount = 2;
    private float rayCastDistance = 100f;
    private List<PlayerController> playerControllers;
    [SerializeField]
    private List<PlayerState> playerStates;
    private Dictionary<Player,PlayerContainer> players = new Dictionary<Player, PlayerContainer>();
    private GameState gameState;
    private List<Collectable> ammoPickups = new List<Collectable>();
    private List<Collectable> energyPickups = new List<Collectable>();
    private List<LaserController> weapons = new List<LaserController>();
    private List<FSM> fsms = new List<FSM>();
    public Button startButton;
    private int selectedPlayer = -1;
    protected PlayerController SelectedPlayer {
      get => selectedPlayer > -1
        ? playerControllers[selectedPlayer] ?? null
        : null;
    }
    private float deltaTime = 0;
    private ObservableProperty<int> winner = new ObservableProperty<int>(-1);
    public FSMInitializer Player1FSM;
    public FSMInitializer Player2FSM;
    public Color[] PlayerColors = new Color[] {
      Color.HSVToRGB(215,100,75), // 004FBE
      Color.HSVToRGB(25,100,100),// FF6A00
    };
    public TMPro.TMP_Text debug;
    public Transform moveTo;
    private bool debugConsole = false;
    private void Awake() {
      Instance = this;

      // Game State
      gameState = new GameState();
      gameState.Time.OnChange += (time) => {
        GameEvents.OnHudTime.Invoke((int)time);
        if(time <= 0 && gamePhase.Value == GamePhase.Battle) {
          var w = CheckWinner();
          if(w == -1) {
            gamePhase.Value = GamePhase.Battle1;
          }
        }
      };
      winner.OnChange += (player) => {
        if(player == -1) return;
        if(player > playersCount - 1) return;
        gamePhase.Value = GamePhase.End;
      };

      arena = gameObject.AddComponent<Arena>();
      PrepareHud();
      PreparePlayers();
      PrepareCollectables();
      PrepareFlags();
      PrepareStartPoints();
      PrepareWeapons();
      PrepareFSMs();
      AddEvents();

      moveTo = Instantiate<Transform>(Resources.Load<Transform>("Prefabs/MoveTo"));

      startButton = FindObjectOfType<Button>();
      startButton.onClick.AddListener(() => {
        gamePhase.Value = GamePhase.Battle;
      });

      gamePhase.OnChange += phase => {
        switch (phase)
        {
          case GamePhase.Menu:

            break;
          case GamePhase.Battle:
            startButton.gameObject.SetActive(false);
            StartFSM();
            selectedPlayer = 1;
            break;
          case GamePhase.Battle1:
            // Sudden death
            playerStates.ForEach(ps => ps.Points.OnChange += (p) => CheckWinner());
            break;
          case GamePhase.End:
            StopFSM();
            GameEvents.OnWinnerName.Invoke(playerStates[winner.Value].Name);
            break;
          default:
            break;
        } 
      };
    }

    private void PrepareHud() {
      var hudPlayer = FindObjectsOfType<HudPlayer>();
      for (int i = 0; i < hudPlayer.Length; i++) {
        hudPlayer[i].PlayerIndex = i;
        hudPlayer[i].AddEvents();
      }
    }
    private void PreparePlayers() {
      var playerPrefab = Resources.Load<PlayerController>("Prefabs/Player");
      playerControllers = new List<PlayerController>(playersCount);
      playerStates = new List<PlayerState>(playersCount);
      for (var i = 0; i < playersCount; i++)
      {
        int index = i;
        playerControllers.Insert(i,
          Instantiate<PlayerController>(
            playerPrefab,
            arena.startPoints[i],
            arena.startRotations[i]
          )
        );
        playerControllers[i].gameObject.name = $"Player{i+1}";
        playerControllers[i].SetPlayerIndex(i);
        playerControllers[i].Color = PlayerColors[i];
        var nma = playerControllers[i].GetComponent<NavMeshAgent>();
        nma.speed = gameState.PlayerSpeed;
        nma.acceleration = gameState.PlayerAccelaration;
        nma.angularSpeed = gameState.PlayerAngularSpeed;
        // Create PlayerState
        PlayerState state = new PlayerState(i,playerControllers[i]);
        state.Points.OnChange += (points) => {
          GameEvents.OnHudScore.Invoke(state.PlayerIndex,points);
        };
        state.Energy.OnChange += (energy) => {
          GameEvents.OnHudPowerBar.Invoke(index,energy/state.EnergyMax);
          // state.Speed.Value = state.IsCooldownFire
          //   ? 0
          //   : energy > 0 
          //     ? gameState.PlayerSpeed 
          //     : 0;
          state.Speed.Value = energy > 0 
            ? gameState.PlayerSpeed
            : 0;
        };
        state.Speed.OnChange += (speed) => {
          nma.speed = speed;
        };
        state.Ammo.OnChange +=    (ammo) => {
          GameEvents.OnHudAmmo.Invoke(index,ammo);
        };
        state.HasFlag.OnChange += (hasFlag) => GameEvents.OnHudHasFlag.Invoke(index,hasFlag);
        state.HasSightEnemy.OnChange += (hasSightEnemy) => {
          playerControllers[index].HasTarget = hasSightEnemy;
        };
        state.Ammo.Value = gameState.AmmoInitial;
        state.HasFlag.Value = false;
        playerStates.Add(state);
        // Player Fire
        playerControllers[i].SetPlayerDelegates((pIdx) => {
          var ps = GetPlayerState(pIdx);
          if(ps.Ammo.Value > 0 && !ps.IsCooldownFire) {
            ps.Ammo.Value--;
            StartCoroutine(PlayerFire(pIdx));
            weapons[pIdx].Fire();
            if(ps.HasSightEnemy.Value) {
              var other = GetOtherPlayerIndex(pIdx);
              GetOtherPlayerState(pIdx).Energy.Value -= gameState.FireDamage;
              if(ps.FlagState == FlagState.Catched) {
                ps.FlagState = FlagState.Dropped;
                var flag = GetPlayerController(other).GetComponentInChildren<Flag>();
                flag.Drop();
              }
            }
          }
        });
        // Create PlayerData delegates
        PlayerDataDelegates delegates = new PlayerDataDelegates() {
          Ammo            = () => state.Ammo.Value,
          Energy          = () => state.Energy.Value,
          Position        = () => playerControllers[index].transform.position,
          Rotation        = () => playerControllers[index].transform.rotation,
          RemainingDistance=() => playerControllers[index].RemainingDistance,
          FlagDistance    = () => state.FlagTransform
            ? Vector3.Distance(state.Transform.position.ToX0Z(), state.FlagTransform.position.ToX0Z())
            : -1,
          FlagPosition    = () => state.FlagTransform?.position,
          FlagState       = () => state.FlagState,
          PowerUps        = () => GetEnergyPoints(),
          AmmoRefill      = () => GetAmmoPoints(),
          StartPosition   = () => new Vector3() + arena.flagInitalPoints[index],
          HasFlag         = () => state.HasFlag.Value,
          HasSightEnemy   = () => state.HasSightEnemy.Value,
          IsCooldownFire  = () => state.IsCooldownFire,
          PlayerIndex     = () => state.PlayerIndex,
          Speed           = () => state.Speed.Value,
          // EnemyInfo       = () => null
        };
        PlayerDataHandler dataHandler = new PlayerDataHandler(delegates);
        Player player = new Player(dataHandler,playerControllers[i]);
        PlayerContainer container = new PlayerContainer(
          i,
          player,
          delegates,
          dataHandler,
          state,
          playerControllers[i]
        );
        players.Add(container.Player,container);
      }
      var pcs = players.ToList().Select(kvp => kvp.Value).ToList();
      pcs.ForEach(pc => {
        var others = pcs.Where(c => c.Index != pc.Index).ToList();
        var enemies = others.Select(o => o.DataHandler).ToArray();
        pc.Player.SetEnemyData(enemies);
      });
    }

    private void PrepareCollectables() {
      var collects = FindObjectsOfType<Collectable>();
      // int ammoCount = 0;
      foreach (var item in collects) {
        item.OnTriggerCollect += (item,playerIndex,type,v) => {
          OnCollect(item,playerIndex,type,v);
        };
        if(item.Type == CollectableType.Energy) {
          energyPickups.Add(item);
        }
        if(item.Type == CollectableType.Ammo) {
          ammoPickups.Add(item);
        }
      }
      ammoPickups.Where(ap => ap.transform.position != arena.ammoSpots[0])
        .ToList().ForEach(ammoPickup => ammoPickup.gameObject.SetActive(false));
    }

    private void PrepareFlags() {
      var flagPrefab = Resources.Load<Flag>("Prefabs/Flag");
      for(int i = 0; i < arena.flagInitalPoints.Count; i++) {
        var flagPosition = arena.flagInitalPoints[i];
        var flag = Instantiate<Flag>(flagPrefab,flagPosition,Quaternion.identity,arena.transform);
        flag.gameObject.name = $"Flag{i+1}";
        flag.Init(i,OnTriggerFlag);
        flag.Color = PlayerColors[i];
        playerStates[i].FlagTransform = flag.transform;
      }
      
      // var flags = FindObjectsOfType<Flag>().OrderBy(f => f.Owner).ToArray();
      // for (int i = 0; i < flags.Length; i++)
      // {
      // }
    }

    private void PrepareStartPoints() {
      var startPointPrefab = Resources.Load<StartPoint>("Prefabs/StartPoint");
      for (int i = 0; i < arena.flagInitalPoints.Count; i++)
      {
        var startPointPosition = arena.flagInitalPoints[i];
        var startPoint = Instantiate<StartPoint>(startPointPrefab,startPointPosition,Quaternion.identity);
        startPoint.transform.position = arena.startPoints[i];
        startPoint.Init(i,OnTriggerStartPoint);
        startPoint.Color = PlayerColors[i];
      }
    }

    private void PrepareWeapons() {
      var weaponPrefab = Resources.Load<LaserController>("Prefabs/Laser");
      for (int i = 0; i < playersCount; i++)
      {
        weapons.Add(Instantiate(weaponPrefab));
      }
    }

    private void PrepareFSMs() {
      var listPlayers = players.ToList();
      for (int i = 0; i < listPlayers.Count; i++) {
        var index = i;
        var player = listPlayers[i].Key;
        FSMInitializer playerFSM = null;
        try {
          playerFSM = PlayersFSM.Fsm[index]?? null;
          Debug.Log($"Player {i} FSM:{playerFSM}");
        }
        catch (System.Exception) {
          playerFSM = new FSMInitializer();
        }
        if(playerFSM != null) {
          playerStates[index].Name = playerFSM.Name;
          GameEvents.OnPlayerNameChange.Invoke(index,playerFSM.Name);
          var fsm = new FSM(index,player,playerFSM);
          fsms.Add(fsm);
        }
      }
    }

    private int GetFlagIndexFromStartPoint(int flagIndex) {
      // for (int i = 0; i < arena.flagInitalPoints.Count; i++)
      // {
        var flagInitial = arena.flagInitalPoints[flagIndex];
        for (int j = 0; j < arena.startPoints.Count; j++)
        {
          var flagStart = arena.startPoints[j];
          if(Vector3.Distance(flagInitial,flagStart) < 0.05) {
            return j;
          }
        }
      // }
      return -1;
    }

    private void AddEvents() {

    }

    private int GetPlayerIndex(Player player) {
      PlayerContainer container;
      if(players.TryGetValue(player, out container)) {
        return container.Index;
      }
      // for (int i = 0; i < playerControllers.Length; i++)
      // {
      //   if(player == playerControllers[i]) {
      //     return i;
      //   }
      // }
      return -1;
    }

    private void OnCollect(Collectable item, int playerIndex, CollectableType type, object itemValue) {
      PlayerState state = playerStates[playerIndex];
      switch (type)
      {
        case CollectableType.Energy:
          state.Energy.Value += (float)itemValue;
          StartCoroutine(RespawnEnergy(item));
          break;
        case CollectableType.Ammo:
          state.Ammo.Value += (int)itemValue;
          StartCoroutine(RespawnAmmo(item));
          break;
        default:
          break;
      }
    }

    private void OnTriggerFlag(Flag flag,int playerIndex) {
      var ps = GetPlayerState(playerIndex);
      if(flag.Owner == playerIndex) { // My flag
        if(ps.FlagState == FlagState.Dropped) {
          ps.FlagState = FlagState.StartPosition;
          flag.transform.position = arena.startPoints[playerIndex];
        }
      } else {
        ps.HasFlag.Value = true;
        GetPlayerState(flag.Owner).FlagState = FlagState.Catched;
        var pc = GetPlayerController(playerIndex);
        flag.transform.parent = pc.transform;
      }
    }
    public List<int> Points = new List<int>() {0,0};
    private void OnTriggerStartPoint(Component startPoint,int playerIndex) {
      var ps = GetPlayerState(playerIndex);
      var sp = startPoint as StartPoint;
      var otherPs = GetOtherPlayerState(playerIndex);
      // Debug.Log($"Player {playerIndex} SP {sp.Owner} Flag {ps.FlagState} OtherFlag {GetOtherPlayerState(playerIndex).FlagState}");
      if(sp.Owner == playerIndex) { // My start
        if(ps.FlagState == FlagState.StartPosition
          && otherPs.FlagState == FlagState.Catched
        ) {
          // Add point
          ps.Points.Value++;
          Points[playerIndex] = ps.Points.Value;
          ps.HasFlag.Value = false;
          otherPs.FlagState = FlagState.StartPosition;
          var otherFlag = GetPlayerController(playerIndex).GetComponentInChildren<Flag>();
          if(otherFlag != null) {
            otherFlag.transform.parent = null;
            otherFlag.transform.position = arena.startPoints[otherPs.PlayerIndex];
            otherFlag.transform.rotation = Quaternion.identity;
          }
        }
      }
    }

    IEnumerator RespawnAmmo(Collectable item ) {
      item.gameObject.SetActive(false);
      yield return new WaitForSeconds(gameState.AmmoRespawnTime);
      ammoPickups.GetRandomItem().gameObject.SetActive(true);
    }

    IEnumerator RespawnEnergy(Collectable item ) {
      item.gameObject.SetActive(false);
      yield return new WaitForSeconds(gameState.EnergyRespawnTime);
      item.gameObject.SetActive(true);
    }

    IEnumerator PlayerFire(int playerIndex) {
      var ps = GetPlayerState(playerIndex);
      ps.IsCooldownFire = true;
      yield return new WaitForSeconds(gameState.FireStopTime);
      ps.IsCooldownFire = false;
    }

    int CheckWinner() {
      int playerWithMaxPoints = -1;
      if(playerStates[0].Points.Value > playerStates[1].Points.Value) {
        playerWithMaxPoints = 0;
      } else if(playerStates[1].Points.Value > playerStates[0].Points.Value) {
        playerWithMaxPoints = 1;
      }
      winner.Value = playerWithMaxPoints;
      return playerWithMaxPoints;
    }

    Vector3[] GetEnergyPoints() {
      return energyPickups
        .Where(e => e.gameObject.activeSelf)
        .Select(s => s.transform.position).ToArray();
    }

    Vector3[] GetAmmoPoints() {
      return ammoPickups
        .Where(e => e.gameObject.activeSelf)
        .Select(s => s.transform.position).ToArray();
    }

    private void UpdateGameState() {
      if(gamePhase.Value == GamePhase.Battle) {
        gameState.Time.Value -= deltaTime;
      }
    }
    private void UpdatePlayerState() {
      playerStates.ForEach(state => {
        var idx = state.PlayerIndex;
        state.CurrentPosition = state.Transform.position;
        state.IsMoving = Vector3.Distance(state.CurrentPosition,state.LastPosition) > 0.005f;

        if(state.IsMoving) {
          state.WaitTimeStep = state.WaitTimeTotal = 0;
          state.Energy.Value -= gameState.EnergyPerSecond * deltaTime;
        } else {
          state.WaitTimeStep += deltaTime;
          state.WaitTimeTotal += deltaTime;
          if(state.WaitTimeStep >= gameState.EnergyWaitTime + gameState.EnergyWaitTimeInitial) {
            state.WaitTimeStep -= gameState.EnergyWaitTime;
            state.Energy.Value += gameState.EnergyRefillPerSecond;
          }
        }

        // if(state.PlayerIndex == 0) {
        //   debug.text = $"P0 {state.CurrentPosition} {state.LastPosition} {state.IsMoving} {state.Energy.Value}";
        // }

        // Vector3 origin = state.CurrentPosition + new Vector3(0,0.5f,0.5f);
        Vector3 origin = state.CurrentPosition + state.Transform.up * 0.5f + state.Transform.forward * 0.5f;
        var hits = Physics.RaycastAll(origin,state.Transform.forward,rayCastDistance);
        
        var drawDistance = hits.Length > 0 ? hits[0].distance : rayCastDistance;
        var directionDistance = drawDistance * state.Transform.forward;
        var hitPoint = origin + directionDistance;
        weapons[idx].SetPoints(origin,hitPoint);
        var hasTarget = IsHitPlayer(ref hits);
        state.HasSightEnemy.Value = hasTarget;
        playerControllers[idx].SightDistance = drawDistance;
        

        var drawColor = hits.Length > 0 ? Color.red : Color.green;
        Debug.DrawRay(origin,directionDistance,drawColor );

        state.LastPosition = state.CurrentPosition;
      });
    }

    private void UpdateFSM() {
      fsms.ForEach(fsm => fsm.Update(deltaTime));
    }

    private void StartFSM() {
      fsms.ForEach(fsm => fsm.StartFSM());
    }

    private void StopFSM() {
      fsms.ForEach(fsm => fsm.StopFSM());
    }

    private bool IsHitPlayer(ref RaycastHit[] hits) {
      if(hits.Length == 0) return false;
      foreach (var hit in hits)
      {
        if(hit.collider.gameObject.tag == "Player") {
          return true;
        }
      }
      return false;
    }

    private Player GetPlayer(int playerIndex) => players.Where(p => p.Value.Index == playerIndex).Select(p => p.Value.Player).First();
    private PlayerState GetPlayerState(int playerIndex) => playerStates[playerIndex];
    private PlayerState GetOtherPlayerState(int playerIndex) => playerStates[(playerIndex+1)%playerStates.Count];
    private int GetOtherPlayerIndex(int playerIndex) => ((playerIndex+1)%playerStates.Count);
    private PlayerController GetPlayerController(int playerIndex) => playerControllers[playerIndex];
    public T GetPlayerStateAttribute<T>(Player player,PlayerStateAttribute attribute) {
      int index = GetPlayerIndex(player);
      if(index == -1) {
        throw new Exception($"Not found index for {player}");
      }
      PlayerState ps = playerStates[index];
      object result;
      switch(attribute) {
        case PlayerStateAttribute.Index: result = ps.PlayerIndex; break;
        case PlayerStateAttribute.Energy: result = ps.Energy; break;
        case PlayerStateAttribute.Speed: result = ps.Speed; break;
        case PlayerStateAttribute.Ammo: result = ps.Ammo; break;
        case PlayerStateAttribute.HasFlag: result = ps.HasFlag; break;
        case PlayerStateAttribute.IsCooldownFire: result = ps.IsCooldownFire; break;
        case PlayerStateAttribute.HasSightEnemy: result = ps.HasSightEnemy; break;
        default: result = null; break;
      }
      return (T)result;
    }

    private T Casting<T>(object o) => (T)o;

    private void ChangePlayerState<T>(int index,PlayerStateAttribute attribute, object value) {
      var ps = playerStates[index];
      // T v = value;
      switch (attribute)
      {
        case PlayerStateAttribute.Energy:
          ps.Energy.Value = Casting<float>(value);
          break;
        default:
          break;
      }
    }

    private void UpdateDebug() {
      if(debugConsole) {
        playerStates.ForEach(ps => {
          var str = fsms[ps.PlayerIndex].CurrentState?.ToString();
          str += "\n"+ps.ToString();
          str += $"\nStartPosition {GetPlayer(ps.PlayerIndex).Data.StartPosition}";
          GameEvents.OnHudConsole.Invoke(ps.PlayerIndex,str);
        });
      }

      if(Input.GetKeyDown(KeyCode.P)) {
        debugConsole = !debugConsole;
        GameEvents.OnHudToggleConsole.Invoke();
      }

      // string text = "";
      // playerStates.ForEach(ps => text += $"Player {ps.PlayerIndex} flag:{ps.FlagState}\n");
      // debug.text = text;

      if(Input.GetKeyDown(KeyCode.S)) {
        SelectedPlayer?.Stop();
      }
      if(Input.GetKeyDown(KeyCode.A)) {
        var idx = SelectedPlayer?.PlayerIndex;
        var other = GetOtherPlayerIndex((int)idx);
        var diff = playerStates[(int)other].CurrentPosition - playerStates[(int)idx].CurrentPosition;
        var angle = Vector2.Angle(Vector2.up, diff.XZ());
        SelectedPlayer?.Rotate(angle);
      }
      if(Input.GetKeyDown(KeyCode.F)) {
        SelectedPlayer?.Fire();
      }
      // if(Input.GetKeyDown(KeyCode.N)) {
      //   playerControllers[0].Fire();
      // }
      // if(Input.GetKeyDown(KeyCode.M)) {
      //   playerControllers[1].Fire();
      // }
      if(Input.GetKeyDown(KeyCode.J)) {
        var angle = SelectedPlayer?.transform.rotation.eulerAngles.y;
        SelectedPlayer?.Rotate((float)angle-45);
      }
      if(Input.GetKeyDown(KeyCode.L)) {
        var angle = SelectedPlayer?.transform.rotation.eulerAngles.y;
        SelectedPlayer?.Rotate((float)angle+45);
      }
      if(Input.GetKeyDown(KeyCode.Alpha1)) {
        selectedPlayer = 0;
      }
      if(Input.GetKeyDown(KeyCode.Alpha2)) {
        selectedPlayer = 1;
      }
      // if(Input.GetKeyDown(KeyCode.Keypad0)) {
      //   playerStates.ForEach(ps => ps.FlagState = FlagState.Dropped);
      // }
      // if(Input.GetKeyDown(KeyCode.Keypad1)) {
      //   playerStates.ForEach(ps => ps.FlagState = FlagState.Catched);
      // }
      if(Input.GetMouseButtonDown(0)) {
        if(SelectedPlayer != null) {
          Vector3 v = new Vector3(Input.mousePosition.x,0,Input.mousePosition.y);
          var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          RaycastHit hitInfo;
          if(Physics.Raycast(ray,out hitInfo)) {
            moveTo.position = hitInfo.point;
            SelectedPlayer.MoveToDestination(hitInfo.point);
          }
        }
      }
      if(Input.GetKeyDown(KeyCode.Alpha0)) {
        StartFSM();
      }
      if(Input.GetKeyDown(KeyCode.BackQuote)) {
        
      }
    }

    private void Update() {
      deltaTime = Time.deltaTime;
      UpdateGameState();
      UpdatePlayerState();
      UpdateFSM();
      UpdateDebug();
    }
  }

}