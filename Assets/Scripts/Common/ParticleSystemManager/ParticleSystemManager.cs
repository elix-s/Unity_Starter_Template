using System;
using System.Threading;
using Common.AssetsSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    public static async UniTask<GameObject> InstantiateEffectAsyncS(string address, float destructionTime = 0, Transform parent = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogError("Address is null or empty!");
            return null;
        }

        var assetProvider = new AssetProvider();
        var assetUnloader = new AssetUnloader();
        
        GameObject effect = await assetProvider.GetAssetAsync<GameObject>(address);
        
        if (effect == null)
        {
            Debug.LogError($"Not loading effect: {address}");
            return null;
        }

        assetUnloader.AddResource(effect);
        
        var prefab = Instantiate(effect, parent);
        assetUnloader.AttachInstance(prefab);
        
        if (destructionTime > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(destructionTime), cancellationToken: cancellationToken);
            assetUnloader.Dispose();
        }

        return effect;
    }
}
