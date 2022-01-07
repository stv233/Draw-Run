using UnityEngine;

public class MineObstacle : Obstacle
{
    public override void WhenObstacle(GameObject gameObject)
    {
        SpawnFX();
        if (gameObject.TryGetComponent<Character>(out Character character))
        {
            character.EnableRagdoll();
        }
        this.gameObject.SetActive(false);
    }
}
