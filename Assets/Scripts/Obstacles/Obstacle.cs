using UnityEngine;
using System;

[RequireComponent(typeof(Dreamteck.Splines.SplineFollower))]
public class Obstacle : MonoBehaviour
{

    public event Action<GameObject> OnObstacle;

    [SerializeField] protected GameObject fx;

    private Dreamteck.Splines.SplineFollower follower;

    public Dreamteck.Splines.SplineFollower Follower
    {
        get
        {
            if (follower == null)
            {
                follower = GetComponent<Dreamteck.Splines.SplineFollower>();
            }
            return follower;
        }
    }

    private void OnEnable()
    {
        OnObstacle += WhenObstacle;
        follower = GetComponent<Dreamteck.Splines.SplineFollower>();
    }

    public void InvokeOnObstacle(GameObject gameObject)
    {
        OnObstacle?.Invoke(gameObject);
    }

    public virtual void WhenObstacle(GameObject gameObject)
    {
        SpawnFX();
        gameObject.SetActive(false);
    }

    public virtual void SpawnFX()
    {
        if (fx != null)
        {
            Instantiate(fx, transform.position, transform.rotation);
        }
    }

}
