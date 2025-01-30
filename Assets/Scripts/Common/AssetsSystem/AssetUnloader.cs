using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Common.AssetsSystem
{
    public class AssetUnloader : IAssetUnloader
    {
        private List<Object> _objectsToUnload = new List<Object>();
        
        private List<GameObject> _instances = new List<GameObject>();
        
        public void AddResource(Object objectToUnload)
        {
            if(!_objectsToUnload.Contains(objectToUnload))
                _objectsToUnload.Add(objectToUnload);
        }
        
        public void AttachInstance(GameObject instance)
        {
            if(!_instances.Contains(instance))
                _instances.Add(instance);
        }
        
        public void Dispose()
        {
            foreach (var go in _instances)
            {
                Object.Destroy(go);
            }
            
            foreach (var obj in _objectsToUnload)
            {
                //Debug.Log($"Release object: {obj.name}");
                Addressables.Release(obj);
            }
            
            _objectsToUnload.Clear();
            _instances.Clear();
        }
    }
}