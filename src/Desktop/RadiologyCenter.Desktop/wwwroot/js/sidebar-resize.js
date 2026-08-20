window.trackSidebarResize = function (dotnetRef) {
    window.__sidebarDotnet = dotnetRef;
    window.__sidebarResizeHandler = function () {
        var width = window.innerWidth || document.documentElement.clientWidth;
        if (window.__sidebarDotnet) {
            window.__sidebarDotnet.invokeMethodAsync('OnWindowResized', width);
        }
    };
    window.addEventListener('resize', window.__sidebarResizeHandler);
    window.__sidebarResizeHandler();
};

window.untrackSidebarResize = function () {
    if (window.__sidebarResizeHandler) {
        window.removeEventListener('resize', window.__sidebarResizeHandler);
        window.__sidebarResizeHandler = null;
    }
    window.__sidebarDotnet = null;
};