/* =========================================================
AeroSpec - Global JavaScript
File: wwwroot/js/site.js
========================================================= */

(function () {

    "use strict";
"use strict";


/* =====================================================
   1. DOM READY
   ===================================================== */

document.addEventListener("DOMContentLoaded", function () {

    initDeleteConfirmation();
    initFanTypeSelection();
    initHistorySearch();
    initFormEnhancements();
    initAutoDismissAlerts();
    initNumberFormatting();
    initPerformanceCurve();

});


/* =====================================================
   2. DELETE CONFIRMATION
   ===================================================== */

function initDeleteConfirmation() {

    document.addEventListener("submit", function (event) {

        const form = event.target;

        if (!form.matches("form[data-confirm]")) {
            return;
        }

        const message =
            form.getAttribute("data-confirm") ||
            "Are you sure you want to continue?";

        const confirmed = window.confirm(message);

        if (!confirmed) {
            event.preventDefault();
        }

    });

}


/* =====================================================
   3. FAN TYPE / RADIO CARD SELECTION
   ===================================================== */

function initFanTypeSelection() {

    const options =
        document.querySelectorAll(".fan-option");

    if (!options.length) {
        return;
    }

    function refreshSelection() {

        options.forEach(function (option) {

            const input =
                option.querySelector("input[type='radio']");

            if (!input) {
                return;
            }

            option.classList.toggle(
                "is-selected",
                input.checked
            );

        });

    }


    options.forEach(function (option) {

        option.addEventListener("click", function (event) {

            const input =
                option.querySelector("input[type='radio']");

            if (!input) {
                return;
            }

            /*
             * Avoid changing the radio twice when the
             * user clicks directly on the input.
             */
            if (event.target !== input) {
                input.checked = true;
            }

            refreshSelection();

        });

    });


    document.addEventListener("change", function (event) {

        if (
            event.target.matches(
                ".fan-option input[type='radio']"
            )
        ) {
            refreshSelection();
        }

    });


    refreshSelection();

}


/* =====================================================
   4. HISTORY SEARCH
   ===================================================== */

function initHistorySearch() {

    const searchInput =
        document.getElementById("historySearch");

    const table =
        document.getElementById("historyTable");

    const count =
        document.getElementById("historyCount");

    const noResults =
        document.getElementById("historyNoResults");

    if (!searchInput || !table) {
        return;
    }

    const rows =
        Array.from(
            table.querySelectorAll(
                "tbody .history-row"
            )
        );


    function filterRows() {

        const query =
            searchInput.value
                .trim()
                .toLowerCase();

        let visibleCount = 0;


        rows.forEach(function (row) {

            const rowText =
                row.textContent
                    .replace(/\s+/g, " ")
                    .trim()
                    .toLowerCase();

            const matches =
                query === "" ||
                rowText.includes(query);

            row.style.display =
                matches ? "" : "none";

            if (matches) {
                visibleCount++;
            }

        });


        if (count) {

            count.textContent =
                visibleCount +
                (
                    visibleCount === 1
                        ? " selection"
                        : " selections"
                );

        }


        if (noResults) {

            noResults.style.display =
                visibleCount === 0
                    ? ""
                    : "none";

        }

    }


    searchInput.addEventListener(
        "input",
        filterRows
    );

}


/* =====================================================
   5. FORM ENHANCEMENTS
   ===================================================== */

function initFormEnhancements() {

    const forms =
        document.querySelectorAll(
            "form.aero-form, form[data-aero-form]"
        );

    forms.forEach(function (form) {

        form.addEventListener("submit", function () {

            const submitButtons =
                form.querySelectorAll(
                    "button[type='submit'], input[type='submit']"
                );

            submitButtons.forEach(function (button) {

                if (button.disabled) {
                    return;
                }

                /*
                 * Prevent accidental double submission.
                 */
                button.dataset.originalText =
                    button.textContent;

                button.disabled = true;

                if (button.tagName === "BUTTON") {

                    const text =
                        button.textContent.trim();

                    if (text) {
                        button.textContent =
                            "Processing...";
                    }

                }

            });

        });

    });

}


/* =====================================================
   6. AUTO DISMISSIBLE ALERTS
   ===================================================== */

function initAutoDismissAlerts() {

    const alerts =
        document.querySelectorAll(
            ".aero-alert[data-auto-dismiss]"
        );

    alerts.forEach(function (alert) {

        const delay =
            parseInt(
                alert.getAttribute("data-auto-dismiss"),
                10
            ) || 5000;

        window.setTimeout(function () {

            alert.style.transition =
                "opacity 250ms ease, transform 250ms ease";

            alert.style.opacity = "0";
            alert.style.transform = "translateY(-5px)";

            window.setTimeout(function () {

                if (alert.parentNode) {
                    alert.parentNode.removeChild(alert);
                }

            }, 280);

        }, delay);

    });

}


/* =====================================================
   7. NUMBER FORMATTING
   ===================================================== */

function initNumberFormatting() {

    const elements =
        document.querySelectorAll(
            "[data-number-format]"
        );

    elements.forEach(function (element) {

        const value =
            parseFloat(
                element.textContent
                    .replace(/,/g, "")
                    .trim()
            );

        if (Number.isNaN(value)) {
            return;
        }

        const decimals =
            parseInt(
                element.getAttribute(
                    "data-number-format"
                ),
                10
            );

        element.textContent =
            value.toLocaleString(
                undefined,
                {
                    minimumFractionDigits:
                        Number.isNaN(decimals)
                            ? 0
                            : decimals,

                    maximumFractionDigits:
                        Number.isNaN(decimals)
                            ? 2
                            : decimals
                }
            );

    });

}


/* =====================================================
   8. PERFORMANCE CURVE
   ===================================================== */

function initPerformanceCurve() {

    const canvas =
        document.getElementById(
            "fanPerformanceChart"
        );

    if (!canvas) {
        return;
    }


    /*
     * Details.cshtml exposes:
     *
     * window.aeroCurve
     *
     * The data can be supplied by the controller
     * without embedding controller logic inside JS.
     */

    const curve =
        window.aeroCurve;


    if (!curve || !Array.isArray(curve) || !curve.length) {

        const empty =
            document.getElementById(
                "curve-empty"
            );

        if (empty) {
            empty.style.display = "";
        }

        canvas.style.display = "none";

        return;
    }


    /*
     * Chart.js is optional.
     *
     * If it is not loaded, do not break the rest
     * of the website.
     */

    if (typeof Chart === "undefined") {

        console.warn(
            "AeroSpec: Chart.js is not loaded. " +
            "Performance curve cannot be rendered."
        );

        return;
    }


    const points =
        normalizeCurveData(curve);


    if (!points.length) {

        const empty =
            document.getElementById(
                "curve-empty"
            );

        if (empty) {
            empty.style.display = "";
        }

        canvas.style.display = "none";

        return;
    }


    const ctx =
        canvas.getContext("2d");


    new Chart(ctx, {

        type: "line",

        data: {

            datasets: [
                {
                    label: "Fan Performance",

                    data: points,

                    parsing: false,

                    borderWidth: 2,

                    pointRadius: 3,

                    pointHoverRadius: 5,

                    tension: 0.25,

                    fill: false
                }
            ]

        },


        options: {

            responsive: true,

            maintainAspectRatio: false,


            interaction: {
                mode: "nearest",
                intersect: false
            },


            plugins: {

                legend: {
                    display: true
                },

                tooltip: {

                    callbacks: {

                        label: function (context) {

                            const point =
                                context.raw;

                            if (!point) {
                                return "";
                            }

                            return (
                                "Airflow: " +
                                formatNumber(point.x) +
                                " CFM | " +
                                "Pressure: " +
                                formatNumber(point.y) +
                                " in. wg"
                            );

                        }

                    }

                }

            },


            scales: {

                x: {

                    type: "linear",

                    title: {
                        display: true,
                        text: "Airflow (CFM)"
                    },

                    ticks: {

                        callback: function (value) {
                            return formatNumber(value);
                        }

                    }

                },


                y: {

                    title: {
                        display: true,
                        text: "Static Pressure (in. wg)"
                    },

                    beginAtZero: true

                }

            }

        }

    });

}


/* =====================================================
   9. CURVE DATA NORMALIZATION
   ===================================================== */

function normalizeCurveData(curve) {

    const result = [];


    curve.forEach(function (item) {

        if (!item) {
            return;
        }


        /*
         * Support common property naming conventions
         * coming from ASP.NET Core JSON serialization.
         */

        const x =
            readNumber(
                item,
                [
                    "cfm",
                    "CFM",
                    "airflow",
                    "Airflow",
                    "x",
                    "X"
                ]
            );


        const y =
            readNumber(
                item,
                [
                    "sp",
                    "SP",
                    "staticPressure",
                    "StaticPressure",
                    "pressure",
                    "Pressure",
                    "y",
                    "Y"
                ]
            );


        if (
            x === null ||
            y === null
        ) {
            return;
        }


        result.push({
            x: x,
            y: y
        });

    });


    result.sort(function (a, b) {
        return a.x - b.x;
    });


    return result;

}


/* =====================================================
   10. NUMBER HELPERS
   ===================================================== */

function readNumber(object, propertyNames) {

    for (let i = 0; i < propertyNames.length; i++) {

        const property =
            propertyNames[i];

        if (
            Object.prototype.hasOwnProperty.call(
                object,
                property
            )
        ) {

            const value =
                Number(object[property]);

            if (Number.isFinite(value)) {
                return value;
            }

        }

    }

    return null;

}


function formatNumber(value) {

    const number =
        Number(value);

    if (!Number.isFinite(number)) {
        return value;
    }

    return number.toLocaleString(
        undefined,
        {
            maximumFractionDigits: 2
        }
    );

}


/* =====================================================
   11. GLOBAL UTILITY API
   ===================================================== */

window.AeroSpec = window.AeroSpec || {};


window.AeroSpec.confirm =
    function (message) {

        return window.confirm(
            message ||
            "Are you sure you want to continue?"
        );

    };


window.AeroSpec.formatNumber =
    formatNumber;


})();
