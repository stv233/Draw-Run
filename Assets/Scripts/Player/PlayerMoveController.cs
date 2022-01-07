using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(SplineFollower))][RequireComponent(typeof(CharactersController))]
public class PlayerMoveController : MonoBehaviour
{
    private SplineFollower splineFollower;
    private CharactersController charactersController;

    public bool IsMoving
    {
        get
        {
            return splineFollower.follow;
        }
    }

    void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        charactersController = GetComponent<CharactersController>();
        FreeDraw.Drawable.OnDrawEnd += StartGame;
        GameManager.Instance.OnGameEnd.AddListener(StopMove);
    }

    private void OnDestroy()
    {
        FreeDraw.Drawable.OnDrawEnd -= StartGame;
    }

    public void StartGame(List<Vector2> points, Rect rect)
    {
        if (!IsMoving)
        {
            GameManager.Instance.StartGame();
            StartMove();
        }
    }

    public void StartMove()
    {
        splineFollower.follow = true;
        charactersController.SetState(1);
    }

    public void StopMove()
    {
        splineFollower.follow = false;
    }
}
