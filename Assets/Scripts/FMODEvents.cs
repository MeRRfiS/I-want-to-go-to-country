using FMODUnity;
using UnityEngine;

public sealed class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    public static FMODEvents GetInstance() => instance;

    [field: Header("Timeline SFX")]
    [field: SerializeField] public EventReference WalkOnGrass { get; private set; }

    [field: Header("Action SFX")]
    [field: SerializeField] public EventReference SeedPlant { get; private set; }
    [field: SerializeField] public EventReference ItemBroke { get; private set; }
    [field: SerializeField] public EventReference TreeChop { get; private set; }
    [field: SerializeField] public EventReference SeedbedDig { get; private set; }
    [field: SerializeField] public EventReference Harvest { get; private set; }
    [field: SerializeField] public EventReference Water { get; private set; }
    [field: SerializeField] public EventReference Explosion { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference BackroundMusic { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
