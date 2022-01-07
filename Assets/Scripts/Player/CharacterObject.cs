using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] private Character character;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (other.TryGetComponent<Obstacle>(out Obstacle obstacle))
            {
                obstacle.InvokeOnObstacle(character.gameObject);
            }
        }
    }
}
