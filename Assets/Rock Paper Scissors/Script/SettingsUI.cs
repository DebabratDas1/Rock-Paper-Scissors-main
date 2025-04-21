using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] List<Button> SfxButtons;
    [SerializeField] List<Image> SfxImages;
    [SerializeField] List<Sprite> soundSprites;
    [SerializeField] List<Sprite> musicSprites;
    [SerializeField] List<Sprite> vibrationsSprites;
    [SerializeField] List<List<Sprite>> SfxSprites = new List<List<Sprite>>();

    private void OnEnable() {
        SetSprite();
    }

    private void SetSprite() {
        SfxSprites.Clear();
        SfxSprites.Add(soundSprites);
        SfxSprites.Add(musicSprites);
        SfxSprites.Add(vibrationsSprites);

        SetSFX();
    }
    private void SetSFX() {
        SetSound(0);
        SetMusic(1);
        SetVibration(2);
    }


    public void onClickBackButtons() {
        SFXManager.Instance.onButtonClick();
        transform.gameObject.SetActive(false);
    }
    public void onClickSoundButtons() {
        SFXManager.Instance.onButtonClick();
        SFXManager.Instance._SfxData.isSound = !SFXManager.Instance._SfxData.isSound;
        SFXManager.Instance.SaveDataToPrefs();
        SetSound(0);
    }
    public void onClickMusicButtons() {
        SFXManager.Instance.onButtonClick();
        SFXManager.Instance._SfxData.isMusic = !SFXManager.Instance._SfxData.isMusic;
        SFXManager.Instance.onBgSoundPlay();
        SFXManager.Instance.SaveDataToPrefs();
        SetMusic(1);
    }
    public void onClickVibrationButtons() {
        SFXManager.Instance.onButtonClick();
        SFXManager.Instance._SfxData.isVibration = !SFXManager.Instance._SfxData.isVibration;
        SFXManager.Instance.SaveDataToPrefs();
        SetVibration(2);
    }


    private void SetSound(int idx) {
        SfxImages[idx].sprite = SfxSprites[idx][SFXManager.Instance._SfxData.isSound ? 1 : 0];
    }
    private void SetMusic(int idx) {
        SfxImages[idx].sprite = SfxSprites[idx][SFXManager.Instance._SfxData.isMusic ? 1 : 0];
    }
    private void SetVibration(int idx) {
        SfxImages[idx].sprite = SfxSprites[idx][SFXManager.Instance._SfxData.isVibration ? 1 : 0];
    }

}