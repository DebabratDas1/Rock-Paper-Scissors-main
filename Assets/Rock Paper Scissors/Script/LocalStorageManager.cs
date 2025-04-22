using DD.Web3;
using DG.Tweening;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class PointsHistory
{
    public int points;
    public DateTime timestamp;
    public string reason;
    public string _id;
}

[Serializable]
public class RPSUserInfo
{
    public int totalPoints;
    public string _id;
    public string address;
    public int currentPoints;
    public int highestPoints;
    public List<PointsHistory> pointsHistory;
    public DateTime createdAt;
    public DateTime updatedAt;
    public int __v;
}

[Serializable]
public class RPSUserInfoSend
{
    public string address;
    public int points;
    public string reason;

    public RPSUserInfoSend(string add, int pnt, string rsn) {
        address = add;
        points = pnt;
        reason = rsn;
    }
}




public class LocalStorageManager : MonoBehaviour
{
    public static LocalStorageManager Instance;

    [SerializeField] private bool isLocalTesting;

    [DllImport("__Internal")]
    private static extern void SaveData(string key, string value);

    [DllImport("__Internal")]
    private static extern IntPtr LoadData(string key);


    [Header("Api Data:")]
    public RPSUserInfo _RPSUserInfo;


    [Header("Screens:")]
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject tokenScreen;
    [SerializeField] private GameObject getTokenScreen;
    [SerializeField] private GameObject continueTokenScreen;
    [SerializeField] private GameObject purchaseTokenScreen;

    [SerializeField] private GameObject loadingObject;
    [Header("Get Token URL:")]
    [SerializeField] private string getTokeUrl;

    [Header("Address Variable:")]
    private const string addressKey = "address";
    private const string PrefsAddressKey = "Address";
    public string addressValue;


    [Header("Other Scripts")]
    [SerializeField] private BlockchainManager m_blockchainManager;

    [Header("API Variable:")]
    //private const string getApi = "https://rockpapper-sicc.vercel.app/api/points?address=";//+address
    //private const string storeApi = "https://rockpapper-sicc.vercel.app/api/points";//+json {address,points,reason}

    private const string getApi = "https://test.metakraft.live/api/points?address=";//+address
    private const string storeApi = "https://test.metakraft.live/api/points";//+json {address,points,reason}

    private void Awake() {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }

    string result = "";
    private void CallAddAddressKey()
    {
        if (!isLocalTesting) return;

        string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        System.Random rand = new System.Random();

        for (int i = 0; i < 5; i++)
        {
            result += chars[rand.Next(chars.Length)];
        }
        Debug.Log("Random String: " + result);
        //PlayerPrefs.SetString(PrefsAddressKey, result);
        Save();
    }

    private void Start()
    {
        //StartGame();
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("DirectPlay", 0);
        homeScreen.SetActive(false);
        tokenScreen.SetActive(true);
        loadingObject.SetActive(true);
        continueTokenScreen.SetActive(false);
        getTokenScreen.SetActive(false);
        GameManager.Instance.CoinTextsPrint(0);

        //LoadAnim();

        if (!PlayerPrefs.HasKey("isNew"))
        {
            PlayerPrefs.SetInt("isNew", 1);
            CallAddAddressKey();
        }

        Load();
    }

    /*private void LoadAnim() {
        loadingObject.transform
            .DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }*/


    public void CallApi() {
        Debug.Log($"addressValue = {addressValue}");
        StartCoroutine(GetData(getApi + addressValue));
    }



    public IEnumerator GetData(string url) {
        Debug.Log("URL = " + url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        Debug.Log(11);

        //request.SetRequestHeader("Content-Type", "application/json");
        //Debug.Log(22);

        yield return request.SendWebRequest();

        Debug.Log(33);
        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log(44);
            Debug.Log($"Get Response: {request.downloadHandler.text}");
            _RPSUserInfo = JsonUtility.FromJson<RPSUserInfo>(request.downloadHandler.text);


            GameManager.Instance.CoinTextsPrint(_RPSUserInfo.currentPoints);

            if (m_blockchainManager.currentConfig.network == BlockchainNetwork.Soneium)
            {
                getTokenScreen.SetActive(_RPSUserInfo.currentPoints < 1000);
                continueTokenScreen.SetActive(!getTokenScreen.activeSelf);
            }
            else
            { // If not Soneium chain, then show purchase UI if not purchased yet, otherwise continue
                //purchaseTokenScreen.SetActive(_RPSUserInfo.currentPoints < 1000);
                purchaseTokenScreen.SetActive(true);
                //continueTokenScreen.SetActive(!getTokenScreen.activeSelf);
            }

            
            loadingObject.SetActive(false);
        }
        else {
            Debug.Log(55);
            Debug.LogError($"Error: {request.error}");
        }
        Debug.Log(66);
    }

    public IEnumerator SendData(string jsonData) {
        string url = storeApi + jsonData;
        Debug.Log("URL = " + url);
        Debug.Log("JSON Data: " + jsonData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log($"Post Response: {request.downloadHandler.text}");
        }
        else {
            Debug.LogError($"Error: {request.error}");
        }

    }




    public void onClickGetToken() {
        if (PlayerPrefs.GetInt("DirectPlay") == 1) {
            PlayerPrefs.SetInt("DirectPlay", 0);
            HomeUI.Instance.onClickPlayButton();
        }
        else {
            homeScreen.SetActive(true);
        }
        tokenScreen.SetActive(false);
        getTokenScreen.SetActive(false);
        Application.OpenURL(getTokeUrl);
    }
    public void onClickContinue() {
        if (PlayerPrefs.GetInt("DirectPlay") == 1) {
            PlayerPrefs.SetInt("DirectPlay", 0);
            HomeUI.Instance.onClickPlayButton();
        }
        else {
            homeScreen.SetActive(true);
        }
        tokenScreen.SetActive(false);
        continueTokenScreen.SetActive(false);
    }


    public void Save() {
#if UNITY_WEBGL && !UNITY_EDITOR
        string key = addressKey;
        string value = addressValue;

        SaveData(key, value);
        PlayerPrefs.SetString(PrefsAddressKey, result);

        Debug.LogWarning("Data saved to localStorage.");
#else
        Debug.LogWarning("Saving works only in WebGL build.");
        addressValue = PlayerPrefs.GetString(PrefsAddressKey);
        //InForText.text = "WebGL build only.";
#endif
    }

    public void Load() 
    {

        Debug.Log("AddressValue = " + addressValue);

#if UNITY_WEBGL && !UNITY_EDITOR
        //IntPtr valuePtr = LoadData(addressKey);
        //string value = Marshal.PtrToStringAnsi(valuePtr);

        //addressValue = value;
        //PlayerPrefs.SetString(PrefsAddressKey, addressValue);

        Debug.LogWarning("Not Loaded from localStorage: " + addressValue);
#else
        Debug.LogWarning("Loading works only in WebGL build.");
        addressValue = PlayerPrefs.GetString(PrefsAddressKey, "0x123");
        //InForText.text = "WebGL build only.";
#endif
        Debug.Log("Address Local Storage Data = " + PlayerPrefs.GetString(PrefsAddressKey, addressValue));


        Debug.Log("Address value before checking null : " + addressValue);
        if(!string.IsNullOrEmpty(addressValue))
        {
            Debug.Log("Address value before calling API : " + addressValue);

            CallApi();

        }
    }
}
