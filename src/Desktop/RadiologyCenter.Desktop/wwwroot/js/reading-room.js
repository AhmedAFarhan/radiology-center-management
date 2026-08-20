window.readingRoomKeydown = function (dotnetRef) {
    window.__rrDotnet = dotnetRef;
    window.__rrKeyHandler = function (e) {
        if (e.key === 'Escape' && window.__rrDotnet) {
            window.__rrDotnet.invokeMethodAsync('OnViewerEscape');
        }
    };
    window.addEventListener('keydown', window.__rrKeyHandler);
};

window.unregisterReadingRoomKeydown = function () {
    if (window.__rrKeyHandler) {
        window.removeEventListener('keydown', window.__rrKeyHandler);
        window.__rrKeyHandler = null;
    }
    window.__rrDotnet = null;
};