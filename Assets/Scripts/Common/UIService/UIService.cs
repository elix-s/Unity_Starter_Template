using Common.AssetsSystem;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Common.UIService
{
    public class UIService
    {
        private readonly IAssetProvider _assetProvider;
        private IAssetUnloader _assetUnloader;
        private IObjectResolver _container;
        private IAssetUnloader _loadingWindowUnloader;

        public UIService(IAssetProvider assetProvider, IAssetUnloader assetUnloader, IObjectResolver container,
            IAssetUnloader loadingWindowUnloader)
        {
            _assetProvider = assetProvider;
            _assetUnloader = assetUnloader;
            _container = container;
            _loadingWindowUnloader = loadingWindowUnloader;
        }

        public async UniTask<GameObject> ShowUIPanel(string assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                return null;
            }
            
            var panel = await _assetProvider.GetAssetAsync<GameObject>(assetKey);
            
            if (panel == null)
            {
                return null;
            }
            
            _assetUnloader.AddResource(panel);
            var prefab = _container.Instantiate(panel);
            _assetUnloader.AttachInstance(prefab.gameObject);

            return prefab;
        }
        
        public async UniTask<T> ShowUIPanelWithComponent<T>(string assetKey) where T : Component
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                return null;
            }
            
            var panel = await _assetProvider.GetAssetAsync<GameObject>(assetKey);
            
            if (panel == null)
            {
                return null;
            }
            
            _assetUnloader.AddResource(panel);
            
            var instance = _container.Instantiate(panel);
            var component = instance.GetComponent<T>();
            
            if (component == null)
            {
                Debug.LogError($"Component {typeof(T).Name} not found.");
                return null;
            }
            
            _assetUnloader.AttachInstance(instance);

            return component;
        }
        
        public void HideUIPanel()
        {
            _assetUnloader.Dispose();
        }
    }
}
