using UnityEngine;

public class SawObstacle : Obstacle
{
    private Vector3 rotation = new Vector3(0, 0, 25f);

    private void Update()
    {
        transform.Rotate(rotation);
    }

    public override void WhenObstacle(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Character>(out Character character))
        {
            SpawnFX();
            character.EnableRagdoll();
        }
    }
}
