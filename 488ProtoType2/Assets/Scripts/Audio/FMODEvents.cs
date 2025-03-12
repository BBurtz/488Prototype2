using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("BGM")]
    [field: SerializeField] public EventReference TempBGM { get; private set; }

    [field: Header("Items")]
    [field: SerializeField] public EventReference Cannon { get; private set; }
    [field: SerializeField] public EventReference Drop { get; private set; }
    [field: SerializeField] public EventReference Pickup { get; private set; }
    [field: SerializeField] public EventReference Repair { get; private set; }

    [field: Header("Other")]
    [field: SerializeField] public EventReference Ambience { get; private set; }
    [field: SerializeField] public EventReference ChestOpen { get; private set; }
    [field: SerializeField] public EventReference Footsteps { get; private set; }
    [field: SerializeField] public EventReference Jump {  get; private set; }
    [field: SerializeField] public EventReference LootUp { get; private set; }

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one FMODEvents in the scene");
        }
        instance = this;
    }
}
