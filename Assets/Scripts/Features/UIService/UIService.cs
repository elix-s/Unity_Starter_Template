using System.Collections;
using System.Collections.Generic;
using Common.AssetsSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UIService 
{
    private readonly IAssetProvider _assetProvider;
    private IAssetUnloader _assetUnloader;
    private IObjectResolver _container;

    public UIService(IAssetProvider assetProvider, IAssetUnloader assetUnloader, IObjectResolver container)
    {
        _assetProvider = assetProvider;
        _assetUnloader = assetUnloader;
        _container = container;
    }

    public async UniTask ShowMainMenu()
    {
        var panel = await _assetProvider.GetAssetAsync<GameObject>("MainMenu");
        _assetUnloader.AddResource(panel);
        _container.Instantiate(panel);
    }
}
