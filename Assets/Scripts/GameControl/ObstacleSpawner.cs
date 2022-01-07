using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class ObstacleSpawner : MonoBehaviour
{

    [SerializeField] private List<Obstacle> obstacles = new List<Obstacle>();
    [SerializeField] private SplineComputer spline;
    [SerializeField] private float maxOffset = 0.1f;
    [SerializeField] private float start = 0.1f;
    [SerializeField] private float end = 0.9f;
    [SerializeField] private float count = 5;

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        for (var i = 0; i < count; i++)
        {
            var obstacle = Instantiate(obstacles[Random.Range(0, obstacles.Count)]);
            obstacle.Follower.spline = spline;
            obstacle.Follower.followSpeed = 0;
            obstacle.Follower.SetPercent(Random.Range(start, end));
            obstacle.Follower.motion.offset = new Vector2(Random.Range(-maxOffset, maxOffset), obstacle.Follower.motion.offset.y);
            obstacle.Follower.Move(0);
            obstacle.Follower.follow = false;
        }
    }
}
