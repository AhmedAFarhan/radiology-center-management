window.egcareUi = {
    // Focusout fires before the browser has finished moving focus to the next control.
    focusWithin: element => new Promise(resolve => requestAnimationFrame(() =>
        resolve(!!element && element.contains(document.activeElement))))
};
