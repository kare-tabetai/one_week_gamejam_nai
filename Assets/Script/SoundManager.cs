using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SoundManager : MonoSingleton<SoundManager>
{
    const float kStopFadeTime = 0.1f;

    [SerializeField]
    AudioClip m_default_bgm;
    [SerializeField]
    bool m_is_quaternote_event_playing = false;
    [SerializeField, Tooltip("4分音符のタイミングで呼ばれる,シーンごとに初期化する")]
    UnityEvent m_quater_note_event = null;

    [System.Serializable]
    class AudioSourceData
    {
        public AudioSource audio_source = null;
        public float volume_rate = 1.0f;// 大本の音量設定
        public bool is_fade_running = false;
    }

    const int BGM_AUDIO_SOURCE_LENGTH = 3;
    [SerializeField]
    AudioSourceData[] m_bgm_audio_sourcies = new AudioSourceData[BGM_AUDIO_SOURCE_LENGTH];
    [SerializeField]
    AudioSourceData m_se_audio_sourcies;
    [SerializeField]
    AudioLowPassFilter m_lo_pass_filter;
    [SerializeField]
    AudioHighPassFilter m_hi_pass_filter;

    bool m_is_running_lo_path_croutine;
    bool m_is_running_hi_path_croutine;
    int m_tempo = -1;
    float m_timer = 0;
    float m_quarter_note_time = -1;

    protected override void Awake()
    {
        if (m_isnstance != null)
        {
            print(typeof(SoundManager) + "was instantiated");
            Instance.m_quater_note_event = null;
            Instance.m_quater_note_event = m_quater_note_event;//wip:ここの受け渡しがうまくいかない
            Destroy(gameObject);
            return;
        }
        m_isnstance = this;
        SubAwake();
    }

    protected override void SubAwake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (m_default_bgm)
        {
            PlayBgmSingle(m_default_bgm);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
    }

    public void StartQuarterNoteEvent(int _tempo)
    {
        m_is_quaternote_event_playing = true;
        m_tempo = _tempo;
        m_timer = 0;
        m_quarter_note_time = 60f / m_tempo;
    }
    public void StopQuarterNoteEvent()
    {
        m_is_quaternote_event_playing = false;
        m_tempo = -1;
        m_timer = 0;
    }

    private void Update()
    {
        if (m_is_quaternote_event_playing)
        {
            Debug.Assert(m_tempo != -1 && m_quarter_note_time != -1);

            m_timer += Time.deltaTime;
            if (m_quarter_note_time <= m_timer)
            {
                m_timer = (m_timer - m_quarter_note_time) % m_quarter_note_time;
                if (m_quater_note_event != null) { m_quater_note_event.Invoke(); }
                print("quaternoteevent");
            }
        }
    }

    public void SetBGMVolume(float rate)
    {
        float db = Rate2Db(rate);
        for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
        {
            m_bgm_audio_sourcies[i].volume_rate = rate;
            if (!m_bgm_audio_sourcies[i].is_fade_running)
            {
                m_bgm_audio_sourcies[i].audio_source.outputAudioMixerGroup.audioMixer.SetFloat("BGM", db);
            }
        }
    }

    public void SetSEVolume(float rate)
    {
        float db = Rate2Db(rate);
        m_se_audio_sourcies.volume_rate = rate;
        m_se_audio_sourcies.audio_source.outputAudioMixerGroup.audioMixer.SetFloat("SE", db);
    }

    public float GetBGMVolume()
    {
        return m_bgm_audio_sourcies[0].volume_rate;
    }

    public float GetSEVolume()
    {
        return m_se_audio_sourcies.audio_source.volume;
    }

    public void SetMuteBGM(bool mute)
    {
        if (mute)
        {
            for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
            {
                m_bgm_audio_sourcies[i].audio_source.mute = true;
            }
        }
        else
        {
            for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
            {
                m_bgm_audio_sourcies[i].audio_source.mute = false;
            }
        }
    }

    public void SetMuteSE(bool mute)
    {
        m_se_audio_sourcies.audio_source.mute = mute;
    }

    public void Pause(bool b)
    {
        if (b)
        {
            for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
            {
                m_bgm_audio_sourcies[i].audio_source.Pause();
            }
            m_se_audio_sourcies.audio_source.Pause();
        }
        else
        {
            for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
            {
                m_bgm_audio_sourcies[i].audio_source.UnPause();
            }
            m_se_audio_sourcies.audio_source.UnPause();
        }
    }

    public void PlaySE(AudioClip clip)
    {
        m_se_audio_sourcies.audio_source.PlayOneShot(clip);
    }

    /// <summary>
    /// 今流れているのに重ねてbgm再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgmAdd(AudioClip clip, bool loop = true)
    {
        if (IsPlayingBGM(clip)) { return; }

        int count = 1;
        while (true)
        {
            if (m_bgm_audio_sourcies.Length <= count)
            {
                Debug.LogError("AudioSourceの数以上のBGMを再生しようとしています");
                return;
            }
            if (!m_bgm_audio_sourcies[count].audio_source.isPlaying)
            {
                break;
            }
            else
            {
                count++;
            }
        }
        var audioSource = m_bgm_audio_sourcies[count].audio_source;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    /// <summary>
    /// 今流れているのは止めてbgm再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgmSingle(AudioClip clip, bool loop = true)
    {
        if (IsPlayingBGM(clip)) { return; }

        StopAllBGMImd();
        var audioSource = m_bgm_audio_sourcies.First().audio_source;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();

        float db = Rate2Db(m_bgm_audio_sourcies.First().volume_rate);
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat("BGM", db);
    }

    public bool IsPlayingBGM(AudioClip clip)
    {
        foreach (var audioSource in m_bgm_audio_sourcies)
        {
            if (audioSource.audio_source.clip == clip)
            {
                return true;
            }
        }
        return false;
    }

    public List<AudioClip> GetPlayingList()
    {
        var clips = new List<AudioClip>();
        foreach (var audioSource in m_bgm_audio_sourcies)
        {
            if (audioSource.audio_source.clip)
            {
                clips.Add(audioSource.audio_source.clip);
            }
        }
        return clips;
    }

    public void StopBGM(AudioClip clip)
    {
        foreach (var audioSource in m_bgm_audio_sourcies)
        {
            if (audioSource.audio_source.clip == clip)
            {
                float start_volume = audioSource.audio_source.volume;
                FadeOutBGM(
                    audioSource.audio_source.clip,
                    () =>
                {
                    audioSource.audio_source.Stop();
                    audioSource.audio_source.clip = null;
                    audioSource.audio_source.volume = start_volume;
                },
                kStopFadeTime);

                return;
            }
        }
        Debug.LogWarning("止めるBGMがありません");
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
        {
            if (m_bgm_audio_sourcies[i].audio_source.isPlaying)
            {
                float start_volume = m_bgm_audio_sourcies[i].audio_source.volume;
                FadeOutBGM(
                m_bgm_audio_sourcies[i].audio_source.clip,
                null,
                kStopFadeTime);
            }
        }
    }

    public void StopAllBGMImd()
    {
        for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
        {
            if (m_bgm_audio_sourcies[i].audio_source.isPlaying)
            {
                m_bgm_audio_sourcies[i].audio_source.Stop();
                //m_bgm_audio_sourcies[i].audio_source.outputAudioMixerGroup.audioMixer.SetFloat("BGM", Rate2Db(0.0f));
                m_bgm_audio_sourcies[i].audio_source.clip = null;
            }
        }
    }

    public void SetActiveFilter(bool active)
    {
        m_lo_pass_filter.enabled = active;
        m_hi_pass_filter.enabled = active;
    }

    const float DefaultFadeTime = 1;

    public void FadeInBGM(AudioClip clip, Action action = null, float fadeTime = DefaultFadeTime)
    {
        for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
        {
            if (!m_bgm_audio_sourcies[i].audio_source.isPlaying)
            {
                StartCoroutine(FadeInCoroutine(i, action, fadeTime));
                return;
            }
        }
        Debug.LogError("AudioSourceの数以上のBGMを再生しようとしています");
    }
    public void FadeOutBGM(AudioClip clip, Action action = null, float fadeTime = DefaultFadeTime)
    {
        for (int i = 0; i < m_bgm_audio_sourcies.Length; i++)
        {
            if (m_bgm_audio_sourcies[i].audio_source.clip == clip)
            {
                m_bgm_audio_sourcies[i].audio_source.clip = clip;
                m_bgm_audio_sourcies[i].audio_source.Play();
                StartCoroutine(FadeOutCoroutine(i, action, fadeTime));
                return;
            }
        }
        Debug.LogWarning("FadeOutするBGMがありません");
    }

    public void SetLowPassRate(float hz_rate)
    {
        float hz = rate2Hz(hz_rate);
        m_lo_pass_filter.cutoffFrequency = hz;
    }

    public void SetHighPassRate(float hz_rate)
    {
        float hz = rate2Hz(hz_rate);
        m_hi_pass_filter.cutoffFrequency = hz;
    }

    public IEnumerator FadeLowPassCoroutine(float end_rate, Action action, float fade_time)
    {
        while (m_is_running_hi_path_croutine) { yield return null; }
        m_is_running_hi_path_croutine = true;

        float start_rate = Hz2Rate(m_lo_pass_filter.cutoffFrequency);
        float rate_distance = end_rate - start_rate;

        float timer = 0;
        while (timer < fade_time)
        {
            yield return null;
            timer += Time.deltaTime;
            float ratio = timer / fade_time;
            SetLowPassRate(start_rate + rate_distance * ratio);
        }
        SetLowPassRate(end_rate);
        if (action != null) { action.Invoke(); }
        m_is_running_hi_path_croutine = false;
    }

    public IEnumerator FadeHiPassCoroutine(float end_rate, Action action, float fade_time)
    {
        while (m_is_running_lo_path_croutine) { yield return null; }
        m_is_running_lo_path_croutine = true;

        float start_rate = Hz2Rate(m_hi_pass_filter.cutoffFrequency);
        float rate_distance = end_rate - start_rate;

        float timer = 0;
        while (timer < fade_time)
        {
            yield return null;
            timer += Time.deltaTime;
            float ratio = timer / fade_time;
            SetHighPassRate(start_rate + rate_distance * ratio);
        }
        SetHighPassRate(end_rate);
        if (action != null) { action.Invoke(); }
        m_is_running_lo_path_croutine = false;
    }

    IEnumerator FadeInCoroutine(int audio_source_index, Action action, float fade_time)
    {
        while (m_bgm_audio_sourcies[audio_source_index].is_fade_running) { yield return null; }
        m_bgm_audio_sourcies[audio_source_index].is_fade_running = true;

        AudioSourceData audioSourceData = m_bgm_audio_sourcies[audio_source_index];

        float timer = 0;
        while (timer < fade_time)
        {
            yield return null;
            timer += Time.deltaTime;
            float ratio = timer / fade_time;
            float volume_rate = ratio * audioSourceData.volume_rate;
            float db = Rate2Db(volume_rate);
            audioSourceData.audio_source.outputAudioMixerGroup.audioMixer.SetFloat("BGM", db);
        }
        audioSourceData.audio_source.outputAudioMixerGroup.audioMixer.SetFloat("BGM", Rate2Db(audioSourceData.volume_rate));
        if (action != null) { action.Invoke(); }

        m_bgm_audio_sourcies[audio_source_index].is_fade_running = false;
    }
    IEnumerator FadeOutCoroutine(int audio_source_index, Action action, float fade_time)
    {
        while (m_bgm_audio_sourcies[audio_source_index].is_fade_running) { yield return null; }
        m_bgm_audio_sourcies[audio_source_index].is_fade_running = true;

        AudioSourceData audioSourceData = m_bgm_audio_sourcies[audio_source_index];

        float timer = 0;
        while (timer < fade_time)
        {
            yield return null;
            timer += Time.deltaTime;
            float ratio = 1 - timer / fade_time;
            float volume_rate = ratio * audioSourceData.volume_rate;
            float db = Rate2Db(volume_rate);
            audioSourceData.audio_source.outputAudioMixerGroup.audioMixer.SetFloat("BGM", db);
        }
        audioSourceData.audio_source.Stop();
        audioSourceData.audio_source.outputAudioMixerGroup.audioMixer.SetFloat("BGM", Rate2Db(0.0f));
        audioSourceData.audio_source.clip = null;
        if (action != null) { action.Invoke(); }

        m_bgm_audio_sourcies[audio_source_index].is_fade_running = false;
    }

    // 10hzで0、22000hzで1になるような対数変換を返す
    float Hz2Rate(float hz)
    {
        return (Mathf.Log(hz) - Mathf.Log(10f)) / (Mathf.Log(22000f) - Mathf.Log(10f));
    }
    float rate2Hz(float rate)
    {
        var t = Mathf.Log(22000f) - Mathf.Log(10f);
        return Mathf.Exp(rate * t) * 10f;
    }

    /// デシベル変換
    /// 0, 1, 10の音圧→-80, 0, 20のデシベル
    float Rate2Db(float rate)
    {
        if (rate == 0.0f) { return -80; }
        return 20f * Mathf.Log10(rate);
    }

    /// 音圧変換
    /// -80, 0, 20のデシベル→0, 1, 10の音圧
    float Db2Rate(float db)
    {
        return Mathf.Pow(10, db / 20);
    }
}
