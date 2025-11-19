using CocoDoogy;
using CocoDoogy.Core;
using System;
using System.Collections;
using UnityEngine;

public class AudioSetting : Singleton<AudioSetting>
{
    public static event Action<float> OnMasterChanged = null;
    public static event Action<float> OnBgmChanged = null;
    public static event Action<float> OnSfxChanged = null;


    public static float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;

            OnMasterChanged?.Invoke(masterVolume);
        }
    }
    public static float BgmVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = value;

            OnBgmChanged?.Invoke(bgmVolume);
        }
    }
    public static float SfxVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = value;

            OnSfxChanged?.Invoke(sfxVolume);
        }
    }
    
    private static float masterVolume = 0.75f;
    private static float bgmVolume = 0.75f;
    private static float sfxVolume = 0.75f;


    protected override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        DontDestroyOnLoad(gameObject);

        //VolumeController.OnVolumeChanged += AwakeAudioSetting;
    }

    private void Start()
    {
        AwakeAudioSetting();
    }

    private static void OnMasterSave(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        VolumeController.Instance.SetMasterVolume(value);
    }
    private static void OnBgmSave(float value)
    {
        PlayerPrefs.SetFloat("BgmVolume", value);
        VolumeController.Instance.SetBgmVolume(value);
    }
    private static void OnSfxSave(float value)
    {
        PlayerPrefs.SetFloat("SfxVolume", value);
        VolumeController.Instance.SetSfxVolume(value);
    }

    private void AwakeAudioSetting()
    {
        OnMasterChanged += OnMasterSave;
        OnBgmChanged += OnBgmSave;
        OnSfxChanged += OnSfxSave;

        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.75f);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.75f);
    }
}