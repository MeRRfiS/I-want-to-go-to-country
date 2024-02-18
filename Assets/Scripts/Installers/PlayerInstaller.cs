using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [Header("Params")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _holdArea;
    [SerializeField] private Transform _hand;
    [SerializeField] private CharacterController _chController;
    [SerializeField] private Animator _handAnim;

    public override void InstallBindings()
    {
        Container.Bind<IItemFactory>()
                 .To<ItemFactory>()
                 .AsSingle();

        Container.Bind<IPlayerService>()
                 .To<PlayerService>()
                 .AsSingle()
                 .WithArguments(_player, _holdArea, _hand, _chController, _handAnim);
    }
}
