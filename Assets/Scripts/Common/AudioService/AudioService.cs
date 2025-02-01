using Common.AssetsSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public class AudioService 
{
    private readonly IAssetProvider _assetProvider;
    private IAssetUnloader _assetUnloader;
    private IObjectResolver _container;
    private AudioSource _audioSource;

    public AudioService(IAssetProvider assetProvider, IAssetUnloader assetUnloader, IObjectResolver container)
    {
        _assetProvider = assetProvider;
        _assetUnloader = assetUnloader;
        _container = container;
    }

    public async UniTask InstantiateAudioSource()
    {
        var panel = await _assetProvider.GetAssetAsync<GameObject>("AudioSource");
        _assetUnloader.AddResource(panel);
       _audioSource = _container.Instantiate(panel).GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip audioClip)
    {
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }
    
    public void StopAudio()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }
}
