/* =========================================================
CENTRI_SELECT - Dashboard JavaScript
File: wwwroot/js/centri-select-global.js
Drives the fan-model-card switcher using data attributes that
are rendered server-side from the real FanSelectionResultViewModel.
No fabricated data — every value read here already exists in the DOM.
========================================================= */

(function () {
    'use strict';

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) {
            el.textContent = value;
        }
    }

    function handleCardSelect(card) {
        var carousel = card.closest('#js-fan-model-carousel');
        if (!carousel) {
            return;
        }

        carousel.querySelectorAll('.fan-model-card').forEach(function (c) {
            c.classList.remove('active');
        });
        card.classList.add('active');

        var sizeId = card.getAttribute('data-size-id');

        setText('js-selected-fan-model', sizeId);
        setText('js-spec-diameter', card.getAttribute('data-diameter'));
        setText('js-spec-rpm', card.getAttribute('data-rpm'));
        setText('js-spec-bhp', card.getAttribute('data-bhp'));
        setText('js-spec-eff', card.getAttribute('data-eff'));
        setText('js-spec-noise', card.getAttribute('data-noise'));
        setText('js-spec-hp', card.getAttribute('data-hp'));

        carousel.querySelectorAll('.fan-model-sub').forEach(function (sub) {
            sub.textContent = '';
        });
        var sub = card.querySelector('.fan-model-sub');
        if (sub) {
            sub.textContent = 'Selected';
        }
    }

    function init() {
        document.querySelectorAll('#js-fan-model-carousel .fan-model-card').forEach(function (card) {
            card.addEventListener('click', function () {
                handleCardSelect(card);
            });
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
