(function () {
    var CARD_ROWS = '.mud-sm-table .mud-table-body .mud-table-row, .mud-xs-table .mud-table-body .mud-table-row';
    var CHEVRON_SVG = '<svg viewBox="0 0 24 24" aria-hidden="true" focusable="false"><path d="M7.41 8.59 12 13.17l4.59-4.58L18 10l-6 6-6-6z"/></svg>';

    function isCardMode() {
        return window.matchMedia('(max-width: 960px)').matches;
    }

    function isSkippableRow(row) {
        return row.classList.contains('mud-table-empty-row')
            || row.classList.contains('mud-table-loading-row')
            || !!row.querySelector('td[colspan]');
    }

    function isInteractive(target) {
        return !!(target.closest && target.closest('button, a, input, select, textarea, label, .mud-input-control, .mud-menu'));
    }

    function ensureChevron(row) {
        if (row.querySelector('.card-chevron')) return;
        var firstCell = row.querySelector('td.mud-table-cell');
        if (!firstCell) return;
        var chevron = document.createElement('span');
        chevron.className = 'card-chevron';
        chevron.setAttribute('aria-hidden', 'true');
        chevron.innerHTML = CHEVRON_SVG;
        firstCell.appendChild(chevron);
    }

    function decorate(root) {
        if (!isCardMode()) return;
        var rows = root.querySelectorAll(CARD_ROWS);
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (isSkippableRow(row)) continue;
            ensureChevron(row);
            if (!row.classList.contains('card-init')) {
                row.classList.add('card-init');
                row.setAttribute('aria-expanded', 'false');
            }
        }
    }

    document.addEventListener('click', function (e) {
        if (!isCardMode() || isInteractive(e.target)) return;
        var row = e.target.closest ? e.target.closest('tr.mud-table-row') : null;
        if (!row || !row.closest('.mud-sm-table, .mud-xs-table') || isSkippableRow(row)) return;
        var expanded = row.classList.toggle('card-expanded');
        row.setAttribute('aria-expanded', expanded ? 'true' : 'false');
    });

    var pending = false;
    var observer = new MutationObserver(function () {
        if (pending) return;
        pending = true;
        requestAnimationFrame(function () {
            pending = false;
            decorate(document);
        });
    });
    observer.observe(document.body, { childList: true, subtree: true });

    window.initResponsiveCards = decorate;
    document.addEventListener('DOMContentLoaded', function () { decorate(document); });
})();
