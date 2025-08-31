using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (AudioSource))]
public class CustomAudioSource : MonoBehaviour { // Add this to all objects needing an audio source

    [NaughtyAttributes.ReorderableList]
    public SFXClip[] sounds = { null };
    [SerializeField]
    private AudioSource source;
    public bool RandomizeAndPlayOnStart = false;
    [Tooltip ("When we 'expect' a lot of false positives, e.g. items that sometimes do or sometimes don't have certain types, set to false")]
    public bool m_debugWarnings = false;

    private Dictionary<SFXType, List<SFXClip>> m_soundsDict = new Dictionary<SFXType, List<SFXClip>> { };

    void Awake () {
        UpdateSoundDictionary (sounds);
    }
    void Start () {

        if (source == null) {
            source = GetComponent<AudioSource> ();
        };
        if (sounds.Length > 0) {
            if (sounds[0].m_clip == null) {
                sounds[0].m_clip = AudioSource.clip;
            }
        };
        if (AudioManager.instance != null) {
            AudioManager.instance.AddSource (AudioSource);
        }
        if (RandomizeAndPlayOnStart) {
            RandomizeAndPlay ();
        }
    }

    public void RandomizeSound () {
        if (sounds.Length > 0) {
            AudioClip randomClip = sounds[Random.Range (0, sounds.Length)].m_clip;
            AudioSource.clip = randomClip;
        };
    }
    public AudioSource AudioSource {
        get {
            if (source == null) {
                source = GetComponent<AudioSource> ();
            }
            return source;
        }
        set {
            source = value;
        }
    }
    public void RandomizeAndPlay () {
        RandomizeSound ();
        Play ();
    }
    public SFXClip GetRandomClip (SFXType type) {
        List<SFXClip> clipList = new List<SFXClip> { };
        m_soundsDict.TryGetValue (type, out clipList);
        if (clipList != null) {
            return clipList[Random.Range (0, clipList.Count)];
        } else {
            if (m_debugWarnings) {
                Debug.LogWarning ("No SFXtype of type " + type + " found!", gameObject);
            };
            return null;
        }
    }
    public void PlayRandomType (SFXType type) {
        SFXClip randomClip = GetRandomClip (type);
        if (randomClip != null) {
            AudioSource.clip = randomClip.m_clip;
            Play ();
        }
    }
    public void PlayRandomTypeByIndex (int index) { // for the damned onclick shits
        if (index >= 0 && index < sounds.Length) {
            SFXClip clip = sounds[index];
            PlayRandomType (clip.m_type);
        }
    }
    public void PlayClipByIndex (int index) {
        if (index >= 0 && index < sounds.Length) {
            SFXClip clip = sounds[index];
            AudioSource.clip = clip.m_clip;
            Play ();
        }
    }
    public void Play () {
        if (AudioSource.clip == null) {
            // Debug.LogWarning ("No clip found!", source);
        } else {
            AudioSource.Play ();
        };
    }
    public void PlayAndLoop (AudioClip clip) {
        if (clip != null) {
            AudioSource.clip = clip;
            AudioSource.loop = true;
            AudioSource.Play ();
        }
    }
    public void StopLoop (bool stopSound = true) {
        AudioSource.loop = false;
        if (stopSound) {
            AudioSource.Stop ();
        }
    }
    public void PlayOneShot (AudioClip clip) {
        AudioSource.PlayOneShot (clip);
    }
    public void PlayOneShot (SFXType type) {
        SFXClip randomClip = GetRandomClip (type);
        if (randomClip != null) {
            AudioSource.PlayOneShot (randomClip.m_clip);
        }
    }
    public void UpdateSoundDictionary (SFXClip[] newSounds, bool clearOld = false) {
        if (clearOld) {
            m_soundsDict.Clear ();
        }
        if (newSounds != null) {
            foreach (SFXClip clip in newSounds) { // make a list of all them soundssss babeh
                if (m_soundsDict.ContainsKey (clip.m_type)) {
                    m_soundsDict[clip.m_type].Add (clip);
                } else {
                    List<SFXClip> newList = new List<SFXClip> { clip };
                    m_soundsDict.Add (clip.m_type, newList);
                    // Debug.Log ("Added new sound list of type " + clip.m_type + " to dictionary. List is now: " + m_soundsDict[clip.m_type].Count + " elements deep.");
                }
            }
            // Debug.Log ("Added " + m_soundsDict.Count + " sounds to dictionary!");
        };
    }
    void OnDestroy () {
        AudioManager.instance.RemoveSource (AudioSource);
    }
}