using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MusicType
{
    NONE = 0000,
    MUSIC_SUCCESS = 2000,
    MUSIC_FAILURE = 3000,
    MUSIC_AMBIENT = 4000,
    MUSIC_ACTION = 5000,
    MUSIC_ROMANCE = 6000,
}
public enum SFXType
{
    NONE = 0000,
    UI_CLICK = 1000,
    UI_HOVERON = 1001,
    UI_HOVEROFF = 1002,
    UI_INV_TAKE = 3000,
    UI_INV_USE = 3001,
    UI_INV_OPEN = 3002,
    UI_INV_CLOSE = 3003,

    THRUSTER_FIRE = 4000,
    HIT_OBJECT = 4100,
    REFUELING = 4200,
    DOWNLOADING = 4300,

    AMBIENT_NORMAL = 7000,
    AMBIENT_WIND = 7001,
    AMBIENT_RAIN = 7002,
    AMBIENT_NIGHT = 7003,
    AMBIENT_NIGHT_CAMPFIRE = 7004,
}

[System.Serializable]
public class MusicClip
{
    public MusicType m_type = MusicType.NONE;
    public AudioClip m_clip;
}

[System.Serializable]
public class SFXClip
{
    public SFXType m_type = SFXType.NONE;
    public AudioClip m_clip;
}
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    [SerializeField]
    private List<AudioSource> allSources = new List<AudioSource> { };
    public AudioSource musicSource;
    public CustomAudioSource ambientSource;
    [NaughtyAttributes.ReorderableList]
    public MusicClip[] m_music;
    public float audioVolume = 1f;
    public float musicVolume = 1f;

    public GameObject muted;
    private Coroutine m_musicFader;
    private Coroutine m_ambienceFader;
    private Coroutine m_musicQueuer;
    private MusicType m_currentAmbientClipType = MusicType.NONE;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetSources()
    {
        for (int i = 0; i < allSources.Count; i++)
        {
            if (allSources[i] == null)
            {
                allSources.RemoveAt(i);
            }
        }
    }
    public void AddSource(AudioSource source)
    { // Automagically happens on initialization of audio source
        if (!allSources.Contains(source))
        {
            allSources.Add(source);
            source.volume = audioVolume;
        }
    }
    public void RemoveSource(AudioSource source)
    {
        if (allSources.Contains(source))
        {
            allSources.Remove(source);
        }
    }

    public void SetAudioVolume(float volume)
    {
        foreach (AudioSource source in allSources)
        {
            if (source != null && source != musicSource)
            {
                source.volume = volume;
            }
            ;
        }
        if (muted != null)
        {
            if (volume == 0f)
            {
                muted.SetActive(true);
            }
            else
            {
                muted.SetActive(false);
            }
        }
        ;
        audioVolume = volume;
    }
    public void SetAudioVolumeReverse(float volume)
    {
        SetAudioVolume(1f - volume);
    }
    public void SetMusicVolumeReverse(float volume)
    {
        SetMusicVolume(1f - volume);
    }
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        musicVolume = volume;
    }

    public void PlayMusic(int index)
    { // for use with unity actions - references the index nr. of the music in the list
        if (index >= 0 && index < m_music.Length)
        {
            Debug.Log("<color=green>Playing music at index " + index + "</color>");
            PlayMusic(m_music[index].m_clip);
        }
    }
    public void PlayMusic(AudioClip clip)
    { // simple simple
        PlayMusic(clip, true);
    }
    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (m_musicFader != null)
        {
            StopCoroutine(m_musicFader);
        }
        if (clip == null)
        {
            if (fade)
            {
                m_musicFader = StartCoroutine(FaderSwitcher(null, musicSource, musicVolume, m_musicFader));
                return;
            }
            else
            {
                musicSource.Stop();
                return;
            }
        }
        else
        {
            if (clip != musicSource.clip)
            {
                if (fade)
                {
                    m_musicFader = StartCoroutine(FaderSwitcher(clip, musicSource, musicVolume, m_musicFader));
                }
                else
                {
                    musicSource.clip = clip;
                    musicSource.Play();
                }
                ;
            }
            ;
        }
    }

    public void PlayAmbient(SFXType type)
    {
        SFXClip randomClip = ambientSource.GetRandomClip(type);
        if (randomClip != null)
        {
            PlayAmbient(randomClip.m_clip, true);
        }
        ;
    }
    public void PlayAmbient(AudioClip clip)
    {
        PlayAmbient(clip, true);
    }
    public void PlayAmbient(AudioClip clip, bool fade = true)
    {
        if (m_ambienceFader != null)
        {
            StopCoroutine(m_ambienceFader);
        }
        if (clip == null)
        {
            if (fade)
            {
                m_ambienceFader = StartCoroutine(FaderSwitcher(null, ambientSource.AudioSource, audioVolume, m_ambienceFader));
                return;
            }
            else
            {
                ambientSource.AudioSource.Stop();
                return;
            }
        }
        else
        {
            if (clip != ambientSource.AudioSource.clip)
            {
                if (fade)
                {
                    m_ambienceFader = StartCoroutine(FaderSwitcher(clip, ambientSource.AudioSource, audioVolume, m_ambienceFader));
                }
                else
                {
                    ambientSource.PlayAndLoop(clip);
                }
                ;
            }
            ;
        }
    }

    public void PlayMusic(MusicType type, bool fade = true)
    {

        MusicClip randomClip = GetRandomMusicClip(type);
        if (randomClip != null)
        {
            PlayMusic(randomClip.m_clip, fade);
        }
        else
        {
            Debug.LogWarning("Could not find any music of type " + type.ToString() + ", stopping music instead.");
            musicSource.Stop();
        }
    }

    public void QueueMusic(MusicType musicToQueue, MusicType musicToPlayAfter)
    { // a simple 'queue music' type thing to play e.g. victory tunes
        // we don't enqueue things in more complicated ways...
        if (m_musicQueuer != null)
        {
            StopCoroutine(m_musicQueuer);
        }
        MusicClip firstClip = GetRandomMusicClip(musicToQueue);
        MusicClip secondClip = GetRandomMusicClip(musicToPlayAfter);
        if (firstClip != null && secondClip != null)
        {
            m_musicQueuer = StartCoroutine(MusicQueue(firstClip.m_clip, secondClip.m_clip));
        }
    }

    IEnumerator MusicQueue(AudioClip clipToPlayFirst, AudioClip clipToPlaySecond)
    {
        // First we fade into the start clip
        if (m_musicFader != null)
        {
            StopCoroutine(m_musicFader);
        }
        yield return m_musicFader = StartCoroutine(FaderSwitcher(clipToPlayFirst, musicSource, musicVolume, m_musicFader));
        // then we wait until it is done (-1 second)
        yield return new WaitForSeconds(clipToPlayFirst.length - 1f);
        // then we fade into the next one!
        yield return m_musicFader = StartCoroutine(FaderSwitcher(clipToPlaySecond, musicSource, musicVolume, m_musicFader));
        // and we're done!
        m_musicQueuer = null;
    }

    IEnumerator FaderSwitcher(AudioClip targetClip, AudioSource targetSource, float targetVolume, Coroutine thisRoutine)
    {
        // Volume down
        while (targetSource.volume > 0f)
        {
            targetSource.volume -= 0.1f;
            yield return null;
        }
        targetSource.volume = 0f;
        if (targetClip != null)
        {
            targetSource.clip = targetClip;
            targetSource.Play();
        }
        else
        {
            targetSource.Stop();
        }
        // Volume up
        while (targetSource.volume < targetVolume)
        {
            targetSource.volume += 0.1f;
            yield return null;
        }
        targetSource.volume = musicVolume;
        thisRoutine = null;
    }

    MusicClip GetRandomMusicClip(MusicType type)
    {
        List<MusicClip> randomList = new List<MusicClip> { };
        foreach (MusicClip clip in m_music)
        {
            if (clip.m_type == type)
            {
                randomList.Add(clip);
            }
        }
        if (randomList.Count > 0)
        {
            return randomList[Random.Range(0, randomList.Count)];
        }
        else
        {
            Debug.LogWarning("No music of type " + type.ToString() + "found!");
            return null;
        }
    }

    #region convenienceMethods
    public void PlayAmbientMusic()
    {
        PlayMusic(MusicType.MUSIC_AMBIENT);
    }
    public void PlayActionMusic()
    {
        PlayMusic(MusicType.MUSIC_ACTION);
    }

    public void QueueVictoryMusic()
    {
        QueueMusic(MusicType.MUSIC_SUCCESS, MusicType.MUSIC_AMBIENT);
    }
    public void QueueDefeatMusic()
    {
        QueueMusic(MusicType.MUSIC_FAILURE, MusicType.MUSIC_AMBIENT);
    }
    public void MusicFadeOut()
    {
        PlayMusic(null, true);
    }

    #endregion

}