using System;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

public class InAppWallets : MonoBehaviour
{
  /*  [Header("Connection UI")]
    [SerializeField] private Button connectWalletButton;
    [SerializeField] private GameObject emailInputCanvas;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private Button emailSubmitButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private ulong chainId = 1;

    [Header("Wallet Info Display")]
    [SerializeField] private GameObject walletInfoPanel;
    [SerializeField] private TMP_Text walletAddressText;
    [SerializeField] private TMP_Text walletBalanceText;
    [SerializeField] private Button refreshBalanceButton;

    void Start()
    {
        // Initialize UI states
        if (walletInfoPanel != null)
            walletInfoPanel.SetActive(false);

        // Hide email input canvas initially
        if (emailInputCanvas != null)
            emailInputCanvas.SetActive(false);



        // Set up connect wallet button
        if (connectWalletButton != null)
        {
            connectWalletButton.onClick.RemoveAllListeners();
            connectWalletButton.onClick.AddListener(ShowEmailInput);
            // Set up email authentication
            if (emailSubmitButton != null)
                emailSubmitButton.onClick.RemoveAllListeners();
            emailSubmitButton.onClick.AddListener(ConnectWithEmail);
            // Set up refresh balance button
            if (refreshBalanceButton != null)
            {
                refreshBalanceButton.onClick.RemoveAllListeners();
                refreshBalanceButton.onClick.AddListener(RefreshWalletBalance);
                CheckExistingConnection();
            }


        }

    }
    private void ShowEmailInput()
    {
        // Show email input canvas
        if (emailInputCanvas != null)
            emailInputCanvas.SetActive(true);
        // Hide connect wallet button
        if (connectWalletButton != null)
            connectWalletButton.gameObject.SetActive(false);
        if (statusText != null)
            statusText.text = "Enter your email to connect";
    }
    private async void CheckExistingConnection()
    {
        try
        {
            // Check if a wallet is already connected from a previous session
            var wallet = ThirdwebManager.Instance.GetActiveWallet();
            if (wallet != null)
            {
                // Hide connect wallet button
                if (connectWalletButton != null)
                    connectWalletButton.gameObject.SetActive(false);
                // Hide email input canvas
                if (emailInputCanvas != null)
                    emailInputCanvas.SetActive(false);
                // Show wallet info panel
                if (walletInfoPanel != null)
                    walletInfoPanel.SetActive(true);
                // Get and display wallet address
                var address = await wallet.GetAddress();
                if (walletAddressText != null)
                {
                    string formattedAddress = FormatAddress(address);
                    walletAddressText.text = $"Address: {formattedAddress}";
                    // Get and display wallet balance

                }
                await UpdatewalletBalance(wallet);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking existing connection: {e.Message}");
            if (statusText != null)
                statusText.text = "Error checking wallet connection";
        }
    }
    private async void RefreshWalletBalance()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        if (wallet != null)
            await UpdatewalletBalance(wallet);
    }

    private async void ConnectWithEmail()
    {
        if (emailInputField == null || string.IsNullOrEmpty(emailInputField.text))
        {
            if (statusText != null)
                statusText.text = "Please enter a valid email address";
            return;

          
        }
        try
        {
            if (statusText != null)
                statusText.text = "Connecting...";
            var InAppWalletoptions = new InAppWalletOptions(email: emailInputField.text);
            var options = new WalletOptions(
            provider: WalletProvider.InAppWallet,
            chainId: chainId,
            inAppWalletOptions: InAppWalletoptions
            );
            var wallet = await ThirdwebManager.Instance.ConnectWallet(options);
            // Handle successful connection
            if (wallet != null)
            {
                HandleSuccessfulconnection(wallet);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error connecting with email: {e.Message}");
            if (statusText != null)
                statusText.text = $"Connection error: (e.Message)";
        }
    }

    private async void HandleSuccessfulconnection(IThirdwebWallet wallet)
    {
        try
        {
            // Hide connect wallet button
            if (connectWalletButton != null)
                connectWalletButton.gameObject.SetActive(false);
            // Hide email input canvas
            if (emailInputCanvas != null)
                emailInputCanvas.SetActive(false);
            // Show wallet info panel
            if (walletInfoPanel != null)
                walletInfoPanel.SetActive(true);
            // Get and display wallet address
            var address = await wallet.GetAddress();
            if (walletAddressText != null)
            {
                string formattedAddress = FormatAddress(address);
                walletAddressText.text = $"Address: {formattedAddress}";
            }
            await UpdatewalletBalance(wallet);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error handling successful connection: {e.Message}");
            if (statusText != null)
                statusText.text = "Error displaying wallet info";
        }
    }

    public async System.Threading.Tasks.Task UpdatewalletBalance(IThirdwebWallet wallet)
    {
        if (walletBalanceText != null)
        {
            try
            {
                walletBalanceText.text = "Loading balance...";
                var balance = await wallet.GetBalance(chainId: chainId);
                var chainDetails = await Utils.GetChainMetadata(
                client: ThirdwebManager.Instance.Client,
                chainId: chainId
                );
                var symbol = chainDetails?.NativeCurrency?.Symbol ?? "ETH";
                var balanceEth = Utils.ToEth(
                wei: balance.ToString(),
                decimalsToDisplay: 4,
                addCommas: true
                );
                walletBalanceText.text = $"Balance: {balanceEth} {symbol}";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating balance: {e.Message}");
                walletBalanceText.text = "Error loading balance";
            }
        }
    }
    private string FormatAddress(string address)
    {
        // Format address to show first 6 and last 4 characters
        return address.Length > 10
        ? $" {address.Substring(0, 6)}... {address.Substring(address.Length - 4)}"
        : address;

    }*/
}