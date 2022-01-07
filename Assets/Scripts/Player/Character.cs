using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField] private CharacterObject characterObject;
    [SerializeField] private GameObject ragDoll;

    private Animator animator;
    private CharactersController controller;

    public CharactersController Controller
    {
        get
        {
            return controller;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetState(int state)
    {
        animator.SetInteger("Stage", state);
    }

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
    }

    public void SetController(CharactersController controller)
    {
        this.controller = controller;
    }

    public void SetTag(string tag)
    {
        characterObject.tag = tag;
    }

    public void EnableRagdoll()
    {
        ragDoll.transform.position = transform.position;
        ragDoll.transform.parent = null;
        ragDoll.gameObject.SetActive(true);
        Controller.RemoveCharacter(this);
        gameObject.SetActive(false);
    }

    public void SetColor(Color color)
    {
        characterObject.GetComponent<SkinnedMeshRenderer>().materials[0].color = color;
    }
}
