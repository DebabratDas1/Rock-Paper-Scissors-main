mergeInto(LibraryManager.library, {
  AddAndSwitchChain: function(chainIdPtr, rpcUrlPtr, chainNamePtr, nativeCurrencyPtr, blockExplorerPtr) {
    var chainId = UTF8ToString(chainIdPtr);
    var rpcUrl = UTF8ToString(rpcUrlPtr);
    var chainName = UTF8ToString(chainNamePtr);
    var nativeCurrencyStr = UTF8ToString(nativeCurrencyPtr);
    var blockExplorer = UTF8ToString(blockExplorerPtr);

    console.log("Switch Chain Called");
    console.log("Chain ID: " + chainId);
    console.log("RPC URL: " + rpcUrl);
    console.log("Chain Name: " + chainName);
    console.log("Block Explorer: " + blockExplorer);
    console.log("Native Currency JSON (raw): " + nativeCurrencyStr);

    var nativeCurrency;
    try {
      nativeCurrency = JSON.parse(nativeCurrencyStr);
    } catch (e) {
      console.log("Failed to parse nativeCurrency JSON: " + nativeCurrencyStr);
      console.log(e);
      return;
    }

    if (typeof window.ethereum !== "undefined") {
      console.log("MetaMask detected");

      window.ethereum.request({
        method: 'wallet_switchEthereumChain',
        params: [{ chainId: chainId }]
      }).then(function () {
        console.log("Switched to existing chain " + chainId);
      }).catch(function (switchError) {
        console.log("Switch failed, trying to add chain. Error:");
        console.log(switchError);

        if (switchError.code === 4902) {
          window.ethereum.request({
            method: 'wallet_addEthereumChain',
            params: [{
              chainId: chainId,
              chainName: chainName,
              rpcUrls: [rpcUrl],
              nativeCurrency: nativeCurrency,
              blockExplorerUrls: [blockExplorer],
            }]
          }).then(function () {
            console.log("Chain added, switching now...");
            return window.ethereum.request({
              method: 'wallet_switchEthereumChain',
              params: [{ chainId: chainId }]
            });
          }).catch(function (addError) {
            console.log("Add chain failed:");
            console.log(addError);
          });
        } else {
          console.log("Switch chain failed (non-4902):");
          console.log(switchError);
        }
      });
    } else {
      console.log("MetaMask not available");
    }
  }
});
