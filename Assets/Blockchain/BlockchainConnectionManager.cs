using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DD.Web3
{
    public class ConnectionManager : MonoBehaviour
    {
        [SerializeField] private Button connectWalletButton;
        [SerializeField] private TMP_Text statusText;

        //[SerializeField] private BigInteger chainId = 1868;
        //[SerializeField] private string dropErc20ContractAddress = "0xeb9415D0B989B18231E6977819c24DEF47c855A8";

        [Header("Wallet Info Display")]
        [SerializeField] private GameObject walletInfoPanel;
        [SerializeField] private TMP_Text walletAddressText;
        [SerializeField] private TMP_Text walletBalanceText;
        [SerializeField] private TMP_Text dropERC20BalanceText;
        [SerializeField] private TMP_Text dropERC721BalanceText;


        [SerializeField] private GameObject loadingScreen;

        [SerializeField] private Button disconnectButton;



        [SerializeField] private BlockchainManager blockchainManager;

        [SerializeField] private WalletProvider externalWalletProvider = WalletProvider.WalletConnectWallet;
        [SerializeField] private bool forceMetaMaskOnWebGL = false;


        private IThirdwebWallet wallet;
        private ThirdwebContract dropErc20Contract = null;
        private ThirdwebContract dropErc721Contract = null;

        
        private string walletAddress = "";

        private void Start()
        {

            ThirdwebManager.Instance.onThirdwebInitialized += ThirdwebInitialized;

            // Make sure cursor is visible when UI needs interaction
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


            loadingScreen.SetActive(false);


            if (walletInfoPanel != null)
            {
                walletInfoPanel.SetActive(false);
            }
            if (connectWalletButton != null)
            {
                connectWalletButton.gameObject.SetActive(true);
            }
            if (connectWalletButton != null)
            {
                connectWalletButton.onClick.RemoveAllListeners();
                connectWalletButton.onClick.AddListener(ConnectWallet);
            }
            if (disconnectButton != null)
            {
                disconnectButton.gameObject.SetActive(false);
            }
        }

        private void ThirdwebInitialized()
        {
            loadingScreen.SetActive(false);

            if (connectWalletButton != null)
            {
                connectWalletButton.gameObject.SetActive(true);
            }
        }

        private void ConnectWallet()
        {
            if (statusText != null)
                statusText.text = "Connecting walllet .... ";

            ConnectExternalWallet();

        }

        private async void DisconnectWallet()
        {
            if (wallet != null)
            {
                await wallet.Disconnect();
                walletAddress = "";
                LocalStorageManager.Instance.addressValue = "";
                LocalStorageManager.Instance._RPSUserInfo.address = "";

            }


            if (connectWalletButton != null)
                connectWalletButton.gameObject.SetActive(true);
            if (statusText != null)
                statusText.text = "Connect Wallet";
            // Show wallet info if UI elements exist
            if (walletInfoPanel != null)
            {
                walletInfoPanel.SetActive(false);
            }
        }



        private async void ConnectExternalWallet()
        {
            try
            {
                loadingScreen.SetActive(true);
                connectWalletButton.gameObject.SetActive(false);
                //var providerToUse = externalWalletProvider;
                // adding wallet options

                var externalWalletProvider =
                        Application.platform == RuntimePlatform.WebGLPlayer && forceMetaMaskOnWebGL ? WalletProvider.MetaMaskWallet : WalletProvider.WalletConnectWallet;
                Debug.Log("Wallet provider = " + externalWalletProvider);
                var options = new WalletOptions(provider: externalWalletProvider, chainId: blockchainManager.currentConfig.ChainId);


                /*//var options = new WalletOptions(
                provider: providerToUse,
                chainId: chainId
                );*/
                wallet = await ThirdwebManager.Instance.ConnectWallet(options);
                if (wallet != null)
                {

                    var address = await wallet.GetAddress();


                    walletAddress = address;
                    LocalStorageManager.Instance.addressValue = walletAddress;
                    LocalStorageManager.Instance._RPSUserInfo.address = walletAddress;


                    if (disconnectButton.gameObject)
                    {
                        disconnectButton.gameObject.SetActive(true);
                        disconnectButton.onClick.RemoveAllListeners();
                        disconnectButton.onClick.AddListener(DisconnectWallet);
                    }
                    // Hide Connect button
                    if (connectWalletButton != null)
                        connectWalletButton.gameObject.SetActive(false);
                    if (statusText != null)
                        statusText.text = "Wallet connected!";

                    await wallet.SwitchNetwork(blockchainManager.currentConfig.ChainId);
                    SwitchChain();


                    // Show wallet info if UI elements exist
                    if (walletInfoPanel != null)
                    {
                        walletInfoPanel.SetActive(true);

                        if (walletAddressText != null)
                        {
                            // Format address to show first 6 and last 4 characters
                            string formattedAddress = address.Length > 10
                            ? $"{address.Substring(0, 4)}...{address.Substring(address.Length - 3)}"
                            : address;
                            walletAddressText.text = formattedAddress
                                
                                ;
                        }
                        // Get balance if balance text exists
                        if (walletBalanceText != null)
                        {
                            var balance = await wallet.GetBalance(chainId: blockchainManager.currentConfig.ChainId);
                            var chainDetails = await Utils.GetChainMetadata(
                            client: ThirdwebManager.Instance.Client,
                            chainId: blockchainManager.currentConfig.ChainId
                            );
                            var symbol = chainDetails?.NativeCurrency?.Symbol ?? "ETH";
                            var balanceEth = Utils.ToEth(
                            wei: balance.ToString(),
                            decimalsToDisplay: 4,
                            addCommas: true
                            );
                            walletBalanceText.text = $"Balance: {balanceEth} {symbol}";
                        }

                        // Add chain if not

                        GetDropErc20Balance();
                        GetDropErc721Balance();
                        
                        //await wallet.SignAuthorization(chainId, dropErc20ContractAddress, true);

                        LocalStorageManager.Instance.StartGame();
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error connecting wallet: " + e.Message);
                loadingScreen.SetActive(false);
                connectWalletButton.gameObject.SetActive(true);
                //if (statusText != null)
                //statusText.text = "Error connecting wallet: " + e.Message;
            }
        }
        private async void GetDropErc20Balance()
        {
            try
            {
                
                dropErc20Contract = await ThirdwebManager.Instance.GetContract(address: blockchainManager.currentConfig.dropERC20ContractAddress, chainId: blockchainManager.currentConfig.ChainId);
                var symbol = await dropErc20Contract.ERC20_Symbol();
                    //"abc";
                var balance = await dropErc20Contract.ERC20_BalanceOf(ownerAddress: walletAddress);
                var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 0, addCommas: false);
                Debug.Log($" {symbol} Balance: {balanceEth} {symbol}  \n And NormBalance = {balance}");
                dropERC20BalanceText.text = balanceEth;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private async void GetDropErc721Balance()
        {
            try
            {

                dropErc721Contract = await ThirdwebManager.Instance.GetContract(address: blockchainManager.currentConfig.purchasingTokenContractAddress, chainId: blockchainManager.currentConfig.ChainId);
                var symbol = await dropErc721Contract.ERC721_Symbol();
                //"abc";
                var balance = await dropErc721Contract.ERC721_BalanceOf(ownerAddress: walletAddress);
                Debug.Log("ERC 721 balance = " + balance);
                var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 0, addCommas: false);
                Debug.Log($" {symbol} Balance: {balanceEth} {symbol}  \n And NormBalance = {balance}");
                dropERC721BalanceText.text = "ERC721 Balance = " + balance;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }


        public async void ClaimDropERC20(int amt)
        {
            Debug.Log($"Amount to be claimed {GameManager.Instance.playerScore.ToString()}");
            //Debug.Log($"Amount to be claimed {amt.ToString()}");

            await wallet.SwitchNetwork(blockchainManager.currentConfig.ChainId);
            SwitchChain();
            try
            {
                loadingScreen.SetActive(true);
                /*var allowance = await dropErc20Contract.ERC20_Allowance(
                    blockchainManager.currentConfig.dropERC20ContractAddress,
                    walletAddress
                    );

                Debug.Log("Allowance = " + allowance);*/

                /*var approveReceipt = await dropErc20Contract.ERC20_Approve(
                    wallet,
                    walletAddress,
                    BigInteger.Parse(amt.ToString())
                    );
                Debug.Log("Approve Receipt :  " + approveReceipt.ToString());*/
                //Debug.Log("Allowance = " + allowance);

                /*var claimCond = await dropErc20Contract.DropERC20_GetActiveClaimCondition();

                Debug.Log("Price per token : " + claimCond.PricePerToken);
                Debug.Log("Price per token : " + claimCond.Metadata);
                Debug.Log("Price per token : " + claimCond.QuantityLimitPerWallet);
                Debug.Log("Price per token : " + claimCond.Currency);
*/

                
                    var transactionReceipt = await dropErc20Contract.DropERC20_Claim(wallet, walletAddress, GameManager.Instance.playerScore.ToString());
                    //var transactionReceipt = await dropErc20Contract.DropERC20_Claim(wallet, walletAddress, amt.ToString());

                    Debug.Log(transactionReceipt.ToString());

                if (!transactionReceipt.Status.Value.Equals(BigInteger.One))
                {
                    Debug.Log(" ERC20 Claim Transaction Failed failed ");
                }
                else
                {


                    Debug.Log("ERC 20 Claim done " + transactionReceipt.ToString());
                    int.TryParse(amt.ToString(), out int claimedAmount);


                    // DO ANY KIND OF OPERATIONS HERE

                    // For Example sending data

                    // Saving Claimed Times Count
                    SaveClaim();

                    RPSUserInfoSend infoSend = new RPSUserInfoSend(LocalStorageManager.Instance._RPSUserInfo.address, claimedAmount, "Purchased NFTS");
                    string jsonData = JsonUtility.ToJson(infoSend);
                    Debug.Log(jsonData);
                    LocalStorageManager.Instance.SendData(jsonData);

                    /////////////////////////////////


                }


                loadingScreen.SetActive(false);

            }
            catch (Exception e)
            {
                loadingScreen.SetActive(false);
                Debug.Log("Claiming Error   : " + e);
            }

        }

        public async void ClaimDropERC721(TMP_InputField tokenAmountIF)
        {
            BigInteger.TryParse(tokenAmountIF.text, out BigInteger amt);
            Debug.Log($"Amount to be claimed {amt}");

            await wallet.SwitchNetwork(blockchainManager.currentConfig.ChainId);
            SwitchChain();
            try
            {
                loadingScreen.SetActive(true);
                var transactionReceipt = await dropErc721Contract.DropERC721_Claim(wallet, walletAddress, amt);
                Debug.Log(transactionReceipt.ToString());
                loadingScreen.SetActive(false);

                if (!transactionReceipt.Status.Value.Equals(BigInteger.One))
                {
                    Debug.Log(" ERC Purchase Transaction Failed failed ");
                }
                else
                {
                    

                    Debug.Log("ERC 721 purchase done "+transactionReceipt.ToString());

                    int.TryParse(amt.ToString(), out int pointToIncrease);
                    RPSUserInfoSend infoSend = new RPSUserInfoSend(LocalStorageManager.Instance._RPSUserInfo.address, pointToIncrease, "Purchased NFTS");

                    string jsonData = JsonUtility.ToJson(infoSend);
                    Debug.Log(jsonData);
                    LocalStorageManager.Instance.SendData(jsonData);


                }

            }
            catch (Exception e)
            {
                loadingScreen.SetActive(false);
                Debug.Log("Erc721 Claiming Error   : " + e);
            }

            GetDropErc721Balance();

        }

        #region SaveClaim

        [DllImport("__Internal")]
        private static extern void SaveData(string key, string value);

        [DllImport("__Internal")]
        private static extern IntPtr LoadData(string key);

        private const string CLAIM_COUNT = "ClaimCount";

        public void SaveClaim()
        {
            int claimCount = 0;
#if UNITY_WEBGL && !UNITY_EDITOR

            IntPtr valuePtr = LoadData(CLAIM_COUNT);
            string value = Marshal.PtrToStringAnsi(valuePtr);

            string savedClaimCountString = value;
            int.TryParse(savedClaimCountString, out claimCount);

            claimCount++;

            SaveData(CLAIM_COUNT, claimCount.ToString());

            //addressValue = value;
            //PlayerPrefs.SetString(PrefsAddressKey, addressValue);

#else
            if (PlayerPrefs.HasKey(CLAIM_COUNT))
            {
                string savedClaimCountString = PlayerPrefs.GetString(CLAIM_COUNT);
                int.TryParse(savedClaimCountString, out claimCount);
            }
            claimCount++;
            PlayerPrefs.SetString(CLAIM_COUNT, claimCount.ToString());

#endif
            Debug.Log("ClaimCount = " + claimCount);
        }

#endregion

        #region SwitchNetwork
        //#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
    private static extern void AddAndSwitchChain(string chainId, string rpcUrl, string chainName, string nativeCurrencyJson, string blockExplorerUrl);
//#endif

        public void SwitchChain()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //string chainId = new HexBigInteger( blockchainManager.currentConfig.ChainId ).ToString(); // Mumbai Polygon Testnet

            BigInteger chainIdDecimal = blockchainManager.currentConfig.ChainId;
            //string hexChainId = "0x" + chainIdDecimal.ToString("X"); // Ensure no padding
            string hexChainId = "0x" + (chainIdDecimal == 0 ? "0" : chainIdDecimal.ToString("X").TrimStart('0'));


            Debug.Log("Chain id in hex :  " + hexChainId);


            string rpcUrl = blockchainManager.currentConfig.rpcUrl;
        string chainName = blockchainManager.currentConfig.networkName;
        string blockExplorer = blockchainManager.currentConfig.blockExplorerUrl;
        string nativeCurrencyJson = blockchainManager.currentConfig.nativeCurrencyJson;

        AddAndSwitchChain(hexChainId, rpcUrl, chainName, nativeCurrencyJson, blockExplorer);
#else
            Debug.LogWarning("Chain switching only works in WebGL builds.");
#endif
        }
        #endregion
    }

    /*string chainId = "1868"; // Mumbai Polygon Testnet
    string rpcUrl = "https://rpc.soneium.org";
    string chainName = "Soneium";
    string blockExplorer = "https://soneium.blockscout.com";
    string nativeCurrencyJson = "{\"name\":\"Soneium\",\"symbol\":\"ETH\",\"decimals\":18}";*/

}
