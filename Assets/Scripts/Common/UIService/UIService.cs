using Common.AssetsSystem;
using Cysharp.Threading.Tasks;
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

        public async UniTask ShowLoadingScreen(int delay)
        {
            var panel = await _assetProvider.GetAssetAsync<GameObject>("LoadingScreen");
            var prefab = _container.Instantiate(panel);
            _loadingWindowUnloader.AddResource(panel);
            _loadingWindowUnloader.AttachInstance(prefab);
            
            if (prefab.TryGetComponent(out FadeForCanvasGroup fader))
            {
                fader.Init(delay);
            }
            
            await UniTask.Delay(delay);
            _loadingWindowUnloader.Dispose();
        }
    }
}
