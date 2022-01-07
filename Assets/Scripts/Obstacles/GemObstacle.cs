using UnityEngine;

public class GemObstacle : Obstacle
{
    public override void WhenObstacle(GameObject gameObject)
    {
        SpawnFX();
        this.gameObject.SetActive(false);
    }
}
