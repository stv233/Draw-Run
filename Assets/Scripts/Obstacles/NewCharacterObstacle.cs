using UnityEngine;

public class NewCharacterObstacle : Obstacle
{
    [SerializeField] private Character character;

    public override void WhenObstacle(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Character>(out Character character))
        {
            character.Controller.AddCharacter(this.character);
            SpawnFX();
        }
        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;
        }
    }
}
