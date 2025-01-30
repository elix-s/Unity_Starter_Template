using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.AssetsSystem
{
    public interface IAssetProvider
    {
        UniTask<T> GetAssetAsync<T>(string address, CancellationToken? token = null) where T : UnityEngine.Object;
        void ReleaseAsset<T>(T asset) where T : UnityEngine.Object;
    }
}