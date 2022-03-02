using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundFadeManager : MonoBehaviour
{
    
    private float _fadingSpeed = 2.0f;
    
    [SerializeField]
    private float _fadingDuration = 0.5f;
    public float FadingDuration {
        get {return 1.0f / _fadingSpeed;}
        set{_fadingSpeed = 1.0f / Mathf.Max(0.0f, value);}
    }
    
    [SerializeField]
    private bool _play = false;
    public bool Play {
        get {return _play;}
        set {
            if( _play == value ) {
                return;
            }
            _play = value;
            if( _play ) {
                audioSource.volume = 1.0f;
                StopCoroutine(currentCoroutine);
            } else {
                currentCoroutine = FadeOut();
                StartCoroutine(currentCoroutine);
            }
        }
    }
    
    
    private AudioSource audioSource;
    
    private IEnumerator currentCoroutine = null;
    
    
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        currentCoroutine = FadeOut();
        audioSource.volume = _play ? 1 : 0;
        Play = _play;
        FadingDuration = _fadingDuration;
    }
    
    
    
    
    
    private IEnumerator FadeOut() {
        
        while( audioSource.volume > 0.01f ) {
            audioSource.volume = Mathf.Max(0.0f, audioSource.volume - _fadingSpeed * Time.deltaTime);
            yield return null;
        }
        
        audioSource.volume = 0.0f;
    }
    
}
