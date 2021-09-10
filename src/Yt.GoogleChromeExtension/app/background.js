chrome.browserAction.onClicked.addListener(function (tab) {
    // No tabs or host permissions needed!
    chrome.tabs.executeScript({
        file: 'jquery-2.2.0.min.js'
    });
    chrome.tabs.executeScript({
        file: 'popup.js'
    });
});