/* =========================================================
CENTRI_SELECT - Results screen interactivity
File: wwwroot/js/centri-select-global.js
- Table row switcher for the "Select Fan Model" results table: updates the
  selected-fan summary panel purely from data-* attributes already rendered
  server-side. No fabricated values.
- Performance chart hover crosshair/tooltip, driven by window.__aeroCurve
  (a JSON payload of the already server-computed curve points).
========================================================= */

(function () {
    'use strict';

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) { el.textContent = value; }
    }

    function handleRowSelect(row) {
        var table = row.closest('#js-fan-model-table');
        if (!table) { return; }

        table.querySelectorAll('tr.row-hover').forEach(function (r) {
            r.classList.remove('best');
        });
        row.classList.add('best');

        var sizeId = row.getAttribute('data-size-id');
        setText('js-selected-fan-model', sizeId);
        setText('js-spec-diameter', row.getAttribute('data-diameter'));
        setText('js-spec-rpm', row.getAttribute('data-rpm'));
        setText('js-spec-bhp', row.getAttribute('data-bhp'));
        setText('js-spec-eff', row.getAttribute('data-eff'));
        setText('js-spec-noise', row.getAttribute('data-noise'));
        setText('js-spec-hp', row.getAttribute('data-hp'));

        table.querySelectorAll('.recommended-tag').forEach(function (tag) { tag.remove(); });
    }

    function initTable() {
        var rows = document.querySelectorAll('#js-fan-model-table tr[data-size-id]');
        rows.forEach(function (row) {
            row.addEventListener('click', function () { handleRowSelect(row); });
        });
    }

    function fmt(n) {
        if (n === null || n === undefined || isNaN(n)) { return '—'; }
        return Number(n).toLocaleString(undefined, { maximumFractionDigits: 0 });
    }

    function initChartHover() {
        var svg = document.getElementById('chart-svg');
        var hit = document.getElementById('chart-hit');
        var tooltip = document.getElementById('chart-tooltip');
        var cursorLine = document.getElementById('chart-cursor-line');
        var cursorDot = document.getElementById('chart-cursor-dot');
        var data = window.__aeroCurve;

        if (!svg || !hit || !tooltip || !data || !data.points || !data.points.length) { return; }

        hit.addEventListener('mousemove', function (evt) {
            var rect = svg.getBoundingClientRect();
            var scaleX = data.viewWidth / rect.width;
            var mouseXsvg = (evt.clientX - rect.left) * scaleX;

            var nearest = data.points[0];
            var best = Infinity;
            data.points.forEach(function (p) {
                var d = Math.abs(p.x - mouseXsvg);
                if (d < best) { best = d; nearest = p; }
            });

            if (cursorLine) {
                cursorLine.setAttribute('x1', nearest.x);
                cursorLine.setAttribute('x2', nearest.x);
                cursorLine.style.display = 'block';
            }
            if (cursorDot) {
                cursorDot.setAttribute('cx', nearest.x);
                cursorDot.setAttribute('cy', nearest.y);
                cursorDot.style.display = 'block';
            }

            tooltip.style.display = 'block';
            tooltip.style.left = (evt.clientX - rect.left + 14) + 'px';
            tooltip.style.top = (evt.clientY - rect.top - 10) + 'px';
            tooltip.innerHTML = '<b>' + fmt(nearest.cfm) + ' CFM</b><br/>SP: ' + Number(nearest.sp).toFixed(2) + '"&nbsp; Eff: ' + Math.round(nearest.eff) + '%';
        });

        hit.addEventListener('mouseleave', function () {
            tooltip.style.display = 'none';
            if (cursorLine) { cursorLine.style.display = 'none'; }
            if (cursorDot) { cursorDot.style.display = 'none'; }
        });
    }

    function initExcludedToggle() {
        var toggle = document.getElementById('excluded-toggle');
        var panel = document.getElementById('excluded-panel');
        if (!toggle || !panel) { return; }

        var count = toggle.getAttribute('data-count');
        var label = ' excluded size' + (count === '1' ? '' : 's');

        toggle.addEventListener('click', function () {
            var hidden = panel.style.display === 'none';
            panel.style.display = hidden ? 'block' : 'none';
            toggle.textContent = (hidden ? 'Hide' : 'Show') + ' ' + count + label;
        });
    }

    function init() {
        initTable();
        initChartHover();
        initExcludedToggle();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
