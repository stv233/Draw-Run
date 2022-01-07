public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public UnityEngine.Events.UnityEvent OnGameStart;
    public UnityEngine.Events.UnityEvent OnGameEnd;

    public void StartGame()
    {
        OnGameStart?.Invoke();
    }

    public void EndGame()
    {
        OnGameEnd.Invoke();
    }
}
