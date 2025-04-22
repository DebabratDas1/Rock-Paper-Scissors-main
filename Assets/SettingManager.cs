using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public GameObject Musicobj, Soundobj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

        if (!SFXManager.Instance._SfxData.isSound) {
            Soundobj.SetActive(false);
        }
        else {
            Soundobj.SetActive(true);
        }
        if (!SFXManager.Instance._SfxData.isMusic) {
            Musicobj.SetActive(false);
        }
        else {
            Musicobj.SetActive(true);
        }

        //OnClickMusic();
        //OnClickSound();
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClickQuit() {
        Application.Quit();
    }

    public void OnClickMusic() {

        if (!SFXManager.Instance._SfxData.isMusic) {

            Musicobj.SetActive(true);
            //PlayerPrefs.SetInt("music", 1);
            SFXManager.Instance._SfxData.isMusic = !SFXManager.Instance._SfxData.isMusic;
            SFXManager.Instance.onBgSoundPlay();
            SFXManager.Instance.SaveDataToPrefs();
        }
        else {
            Musicobj.SetActive(false);
            SFXManager.Instance._SfxData.isMusic = !SFXManager.Instance._SfxData.isMusic;
            //PlayerPrefs.SetInt("music", 0);
            SFXManager.Instance.onBgSoundStop();
            SFXManager.Instance.SaveDataToPrefs();
        }
    }

    public void OnClickSound() {

        if (!SFXManager.Instance._SfxData.isSound) {
            Soundobj.SetActive(true);
            SFXManager.Instance._SfxData.isSound = !SFXManager.Instance._SfxData.isSound;
            //PlayerPrefs.SetInt("sound", 1);
        }
        else {
            Soundobj.SetActive(false);
            SFXManager.Instance._SfxData.isSound = !SFXManager.Instance._SfxData.isSound;
            //PlayerPrefs.SetInt("sound", 0);
        }
    }

}
