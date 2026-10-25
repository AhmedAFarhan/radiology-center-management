window.setAppLanguage = function (lang, dir) {
    document.documentElement.lang = lang;
    document.documentElement.dir = dir;
    var errorMessage = document.getElementById('blazor-error-message');
    if (errorMessage) {
        errorMessage.textContent = lang === 'ar' ? 'حدث خطأ غير متوقع.' : 'An unhandled error has occurred.';
    }
    var reload = document.querySelector('#blazor-error-ui .reload');
    if (reload) {
        reload.textContent = lang === 'ar' ? 'إعادة التحميل' : 'Reload';
    }
    var dismiss = document.querySelector('#blazor-error-ui .dismiss');
    if (dismiss) {
        dismiss.setAttribute('aria-label', lang === 'ar' ? 'إغلاق رسالة الخطأ' : 'Dismiss error');
    }
};
