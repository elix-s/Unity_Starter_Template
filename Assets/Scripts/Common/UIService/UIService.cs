using System.Collections;
using System.Collections.Generic;
using Common.AssetsSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

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

    public async UniTask<GameObject> ShowUIPanel(string assetKey) 
    {
        if (!string.IsNullOrEmpty(assetKey))
        {
            var panel = await _assetProvider.GetAssetAsync<GameObject>(assetKey);
            _assetUnloader.AddResource(panel);

            var prefab = _container.Instantiate(panel);
            _assetUnloader.AttachInstance(prefab.gameObject);

            return prefab;
        }
        else
        {
            return null;
        }
    }
    
    public async UniTask<T> ShowUIPanelWithComponent<T>(string assetKey) where T : Component
    {
        if (!string.IsNullOrEmpty(assetKey))
        {
            var panel = await _assetProvider.GetAssetAsync<GameObject>(assetKey);
            _assetUnloader.AddResource(panel);

            var prefab = _container.Instantiate(panel).GetComponent<T>();
            _assetUnloader.AttachInstance(prefab.gameObject);

            return prefab;
        }
        else
        {
            return null;
        }
    }
    
    public async UniTask HideUIPanel()
    {
        _assetUnloader.Dispose();
    }
    
    public async UniTask ShowMainMenu()
    {
        var panel = await _assetProvider.GetAssetAsync<GameObject>("MainMenu");
        _assetUnloader.AddResource(panel);
        _container.Instantiate(panel);
    }
    
    public async UniTask<GameObject> ShowLoadingScreen()
    {
        var panel = await _assetProvider.GetAssetAsync<GameObject>("LoadingScreen");
        _assetUnloader.AddResource(panel);
        var prefab = _container.Instantiate(panel);
        return prefab;
    }
}
