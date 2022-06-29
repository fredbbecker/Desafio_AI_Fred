using UnityEngine;

namespace ChallengeAI
{
  public class Player : IPlayer
  {
    private IPlayerData data;
    public IPlayerData[] enemyData = null;
    private PlayerController controller;
    public Player(IPlayerData dataHandler, PlayerController controller) {
      data = dataHandler;
      this.controller = controller;
    }

    public void SetEnemyData(IPlayerData[] enemyData) {
      this.enemyData = enemyData;
      // Debug.Log($"Player::SetEnemyData {enemyData.Length} '{this.enemyData == null}' {this.enemyData?.Length}");
    }

    public IPlayerData Data => data;
    public IPlayerData[] EnemyData => enemyData;

    public void Fire()
    {
        controller.Fire();
    }

    public void Move(Vector3 position)
    {
        controller.MoveToDestination(position);
    }

    public void Rotate(float angle)
    {
        controller.Rotate(angle);
    }

    public void Stop()
    {
        controller.Stop();
    }
  }
}