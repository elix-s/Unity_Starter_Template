using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.AssetsSystem
{
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<T> GetAssetAsync<T>(string address, CancellationToken? token = null) where T : UnityEngine.Object
        {
#if !QA_MODE
            return await GetAssetAsyncInternal<T>(address, token);
#endif
            
            try
            {
                return await Addressables.LoadAssetAsync<T>(address);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return default;
        }

        private async UniTask<T> GetAssetAsyncInternal<T>(string address, CancellationToken? token = null) where T : UnityEngine.Object
        {
            if (token.HasValue)
            {
                return await Addressables.LoadAssetAsync<T>(address).WithCancellation(token.Value);
            }
            
            return await Addressables.LoadAssetAsync<T>(address);
        }
        
        public void ReleaseAsset<T>(T asset) where T : UnityEngine.Object
        {
            try
            {
                Addressables.Release(asset);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error release asset: " + ex.Message);
            }
        }
    }
}