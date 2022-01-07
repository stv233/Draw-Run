using System.Collections.Generic;
using UnityEngine;

public class FinishObstacle : Obstacle
{
    public override void WhenObstacle(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<Character>(out Character character))
        {
            List<Vector2> points = new List<Vector2>();
            for (var i = 0; i <= 100; i += 5)
            {
                for (var j = 0; j <= 100; j += 5)
                {
                    points.Add(new Vector2(i, j));
                }
            }
            character.Controller.PlaceCharacters(points,new Rect(0,0,100,100));
        }
        GameManager.Instance.EndGame();
    }
}
