using UnityEngine;

namespace ChallengeAI {
  public interface IPlayer {
    IPlayerData Data {get;}
    void Move(Vector3 position);
    void Rotate(float angle);
    void Stop();
    void Fire();
    IPlayerData[] EnemyData {get;}
  }
}