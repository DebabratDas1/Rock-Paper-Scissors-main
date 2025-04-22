using UnityEngine;

public class HomeUI : MonoBehaviour
{
	public static HomeUI Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null)
			Destroy(Instance.gameObject);
		Instance = this;
	}
	public void onClickPlayButton() {
        SFXManager.Instance.onButtonClick();
        GameManager.Instance.GameStart();
        transform.gameObject.SetActive(false);
    }
}
