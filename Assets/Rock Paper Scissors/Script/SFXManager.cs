using System;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public class SFXData
{
    public bool isMusic;//bg
    public bool isSound;
    public bool isVibration;

    public SFXData() {
        isMusic = true;
        isSound = true;
        isVibration = true;
    }
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public SFXData _SfxData;

    [SerializeField] private AudioSource BgSource;
    [SerializeField] private AudioSource ClickSource;
    [SerializeField] private AudioSource PlaySource;

    [SerializeField] private AudioClip BgClip;
    [SerializeField] private AudioClip ClickClip;
    [SerializeField] private AudioClip[] winLostClip;
    [SerializeField] private AudioClip[] RPSClip;



    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            LoadDataFromPrefs();
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        //Debug.Log("Music = " + PlayerPrefs.GetInt("music"));
        if (_SfxData.isMusic) {
            onBgSoundPlay();
        }

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        BgSource.loop = true;
    }

    public void onBgSoundPlay() {
        BgSource.mute = !_SfxData.isMusic;
        if (BgSource.mute) return;
        BgSource.clip = BgClip;
        BgSource.volume = .1f;
        BgSource.loop = true;
        BgSource.Play();
    }

    public void onBgSoundStop() {

        BgSource.Stop();
    }

    public void onVibrationPlay() {
        if (!_SfxData.isVibration) return;
        //Handheld.Vibrate();
    }

    public void onButtonClick() {
        if (!_SfxData.isSound) return;
        ClickSource.clip = ClickClip;
        ClickSource.loop = false;
        ClickSource.volume = 1f;
        if (_SfxData.isSound) {
            ClickSource.Play();
        }

    }

    public void onWinLostSoundPlay(int idx) {
        if (!_SfxData.isSound) return;
        PlaySource.clip = winLostClip[idx];
        PlaySource.loop = false;
        PlaySource.volume = 1f;
        PlaySource.Play();
    }

    public void onSelectRPS(int idx) {
        if (!_SfxData.isSound) return;
        PlaySource.clip = RPSClip[idx];
        PlaySource.loop = false;
        PlaySource.volume = 1f;
        if (_SfxData.isSound) {
            PlaySource.Play();
        }

    }

    public void SaveDataToPrefs() {
        PlayerPrefs.SetString("SFXData", JsonUtility.ToJson(_SfxData));
        PlayerPrefs.Save();
    }

    private void LoadDataFromPrefs() {
        _SfxData = PlayerPrefs.HasKey("SFXData") ? JsonUtility.FromJson<SFXData>(PlayerPrefs.GetString("SFXData")) : new SFXData();
    }

}
