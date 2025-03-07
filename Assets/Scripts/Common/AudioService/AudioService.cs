using Common.AssetsSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class AudioService 
{
    private readonly IAssetProvider _assetProvider;
    private readonly IAssetUnloader _assetUnloader;
    private readonly IObjectResolver _container;
    
    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;

    public AudioService(IAssetProvider assetProvider, IAssetUnloader assetUnloader, IObjectResolver container)
    {
        _assetProvider = assetProvider;
        _assetUnloader = assetUnloader;
        _container = container;
    }
    
    public async UniTask InstantiateAudioSources()
    {
        GameObject musicPrefab = await _assetProvider.GetAssetAsync<GameObject>("MusicAudioSource");
        _assetUnloader.AddResource(musicPrefab);
        _musicSource = _container.Instantiate(musicPrefab).GetComponent<AudioSource>();
        _musicSource.loop = true; 
        
        GameObject sfxPrefab = await _assetProvider.GetAssetAsync<GameObject>("SFXAudioSource");
        _assetUnloader.AddResource(sfxPrefab);
        _sfxSource = _container.Instantiate(sfxPrefab).GetComponent<AudioSource>();
    }
    
    public void PlayMusic(AudioClip musicClip, float fadeInDuration = 0f)
    {
        if (_musicSource != null)
        {
            _musicSource.Stop();
            _musicSource.clip = musicClip;
            _musicSource.loop = true;

            if (fadeInDuration > 0f)
            {
                _musicSource.volume = 0f;
                _musicSource.Play();
                _musicSource.DOFade(_musicVolume, fadeInDuration);
            }
            else
            {
                _musicSource.volume = _musicVolume;
                _musicSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("Music AudioSource not instantiated.");
        }
    }
    
    public void StopMusic(float fadeOutDuration = 0f)
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            if (fadeOutDuration > 0f)
            {
                _musicSource.DOFade(0f, fadeOutDuration)
                    .OnComplete(() => _musicSource.Stop());
            }
            else
            {
                _musicSource.Stop();
            }
        }
    }
    
    public void PlaySfx(AudioClip sfxClip)
    {
        if (_sfxSource != null)
        {
            _sfxSource.PlayOneShot(sfxClip, _sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFX AudioSource not instantiated.");
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume;
        }
    }
    
    public void SetSfxVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
    }
    
    public void PauseMusic()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.UnPause();
        }
    }
}
