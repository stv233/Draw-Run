using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CharactersController : MonoBehaviour
{
    [SerializeField] private Vector2 min = new Vector2();
    [SerializeField] private Vector2 max = new Vector2();
    [SerializeField] private List<Character> characters = new List<Character>();
    [SerializeField] private Color characterColor = new Color(255, 255, 255);

    private int state = 0;

    private void Start()
    {
        characters.ForEach(x =>
        {
            AssignCharacter(x);
        });
        FreeDraw.Drawable.OnDrawEnd += PlaceCharacters;
        GameManager.Instance.OnGameEnd.AddListener(() =>
        {
            Vector3 rotation = new Vector3(0, 180, 0);
            characters.ForEach(x =>
            {
                x.SetState(2);
                x.transform.DORotate(rotation, 1f);
            });
        });
    }

    private void OnDestroy()
    {
        FreeDraw.Drawable.OnDrawEnd -= PlaceCharacters;
    }

    public void SetState(int state)
    {
        this.state = state;
        characters.ForEach(x => x.SetState(state));
    }

    public void AddCharacter(Character character)
    {
        characters.Add(character);
        AssignCharacter(character);
    }

    public void AssignCharacter(Character character)
    {
        character.SetState(state);
        character.SetParent(transform);
        character.SetController(this);
        character.SetColor(characterColor);
    }

    public void RemoveCharacter(Character character)
    {
        characters.Remove(character);
        character.SetParent(null);
        character.SetController(null);
        if (characters.Count == 0)
        {
            GameManager.Instance.EndGame();
        }
    }

    public void PlaceCharacters(List<Vector2> points, Rect rect)
    {
        List<Vector2> placePoints = new List<Vector2>();
        points.ForEach(x =>
        {
            var xPercent = ((x.x - rect.x) / (rect.width - rect.x));
            var yPercent = ((x.y - rect.y) / (rect.height - rect.y));
            var xCord = min.x + ((max.x - min.x) * xPercent);
            var yCord = min.y + ((max.y - min.y) * yPercent);
            placePoints.Add(new Vector2(xCord,yCord));
        });
        if (placePoints.Count > characters.Count)
        {
            int extra = (int)System.Math.Floor((double)placePoints.Count / characters.Count);
            placePoints = placePoints.Where((x, i) => (i % extra) == 0).ToList();
            if (placePoints.Count > characters.Count)
            {
                placePoints = placePoints.Take(characters.Count).ToList();
            }
        }
        else if (placePoints.Count < characters.Count)
        {
            int count = characters.Count - placePoints.Count;
            while (placePoints.Count < characters.Count)
            {
                count = characters.Count - placePoints.Count;
                placePoints = placePoints.Concat(placePoints.Take(count > placePoints.Count ? placePoints.Count : count)).ToList();
            }
        }
        for (var i = 0; i < characters.Count; i++)
        {
            characters[i].transform.DOLocalMove(new Vector3(placePoints[i].x, transform.localPosition.y, placePoints[i].y), 1f);
        }
    }
}
