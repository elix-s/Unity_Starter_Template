using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.AssetsSystem
{
    public interface IAssetUnloader : IDisposable
    {
        void AddResource(Object objectToUnload);
        void AttachInstance(GameObject instance);
    }
}