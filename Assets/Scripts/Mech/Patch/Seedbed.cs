using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public sealed class Seedbed : MonoBehaviour
{
    [SerializeField] private List<Renderer> _seedbadLods;

    public bool Occupied => _plantObj != null;
    private GameObject _plantObj;
    private EventReference _plantSoundEvent;

    private void Awake()
    {
        _plantSoundEvent = FMODEvents.instance.SeedPlant;
    }

    public void ChangeMaterial(Material material)
    {
        if (material == null) throw new System.ArgumentNullException();

        foreach(var lod in _seedbadLods)
        {
            lod.material = material;
        }
    }

    public bool TryPlant(Seed seed)
    {
        if(Occupied) return false;

        _plantObj = Instantiate(seed.Plant, transform);
        _plantObj.transform.localPosition = new Vector3(0f, 0f, 0f);
        RuntimeManager.PlayOneShot(_plantSoundEvent, transform.position);

        return true;
    }

    public void RemovePlant()
    {
        if (Occupied)
        {
            Destroy(_plantObj);
        }
    }
}
