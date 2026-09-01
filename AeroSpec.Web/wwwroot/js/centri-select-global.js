/* =========================================================
CENTRI_SELECT - Global JavaScript Module
File: wwwroot/js/centri-select-global.js
Handles: Interactions, Charts, Animations, State Management
========================================================= */

// Self-executing module pattern
(function () {
    'use strict';

    // =========================================================
    // 1. GLOBAL CONFIG & STATE
    // =========================================================

    const CentriSelect = {
        state: {
            selectedFanModel: 'AF-140-6D',
            operatingParams: {
                airflow: 12500,
                pressure: 1600,
                density: 1.2,
                temperature: 25
            },
            applicationDetails: {
                ductType: 'spiral',
                motorVoltage: 380,
                frequency: 50,
                environment: 'industrial'
            },
            performanceLimits: {
                maxSpeed: 1800,
                maxPower: 15,
                sizeRange: 'large'
            }
        },

        fanDatabase: {
            'AF-140-6D': {
                name: 'Centrifugal Airfoil Fan - AF-140-6D',
                impellerDia: 1400,
                speed: 1450,
                power: 9.8,
                efficiency: 88.5,
                noise: 82,
                orientation: 'Horizontal',
                driveType: 'Direct Drive',
                material: 'Steel',
                price: 15000
            },
            'AF-130-6D': {
                name: 'Centrifugal Airfoil Fan - AF-130-6D',
                impellerDia: 1300,
                speed: 1480,
                power: 8.5,
                efficiency: 86.2,
                noise: 80,
                orientation: 'Horizontal',
                driveType: 'Belt Drive',
                material: 'Aluminium',
                price: 12000
            },
            'AF-150-6C': {
                name: 'Centrifugal Airfoil Fan - AF-150-6C',
                impellerDia: 1500,
                speed: 1420,
                power: 11.2,
                efficiency: 89.8,
                noise: 84,
                orientation: 'Vertical',
                driveType: 'Direct Drive',
                material: 'Steel',
                price: 18000
            }
        },

        chart: null,

        // =========================================================
        // 2. INITIALIZATION
        // =========================================================

        init: function () {
            this.setupEventListeners();
            this.renderInitialChart();
            this.updateSpecGrid();
            this.setupInputValidation();
            console.log('✓ CentriSelect initialized');
        },

        // =========================================================
        // 3. EVENT LISTENERS
        // =========================================================

        setupEventListeners: function () {
            // Fan selection cards
            document.querySelectorAll('.fan-model-card').forEach(card => {
                card.addEventListener('click', (e) => this.handleFanSelection(e));
            });

            // Operating parameters inputs
            document.getElementById('airflow-input')?.addEventListener('change', (e) => {
                this.state.operatingParams.airflow = parseFloat(e.target.value);
                this.updateChart();
            });

            document.getElementById('pressure-input')?.addEventListener('change', (e) => {
                this.state.operatingParams.pressure = parseFloat(e.target.value);
                this.updateChart();
            });

            document.getElementById('density-input')?.addEventListener('change', (e) => {
                this.state.operatingParams.density = parseFloat(e.target.value);
            });

            document.getElementById('temperature-input')?.addEventListener('change', (e) => {
                this.state.operatingParams.temperature = parseFloat(e.target.value);
            });

            // Application details
            document.getElementById('duct-type-select')?.addEventListener('change', (e) => {
                this.state.applicationDetails.ductType = e.target.value;
            });

            document.getElementById('motor-voltage-select')?.addEventListener('change', (e) => {
                this.state.applicationDetails.motorVoltage = parseInt(e.target.value);
            });

            document.getElementById('frequency-select')?.addEventListener('change', (e) => {
                this.state.applicationDetails.frequency = parseInt(e.target.value);
            });

            document.getElementById('environment-select')?.addEventListener('change', (e) => {
                this.state.applicationDetails.environment = e.target.value;
            });

            // Performance limits
            document.getElementById('max-speed-input')?.addEventListener('change', (e) => {
                this.state.performanceLimits.maxSpeed = parseFloat(e.target.value);
                this.updateChart();
            });

            document.getElementById('max-power-input')?.addEventListener('change', (e) => {
                this.state.performanceLimits.maxPower = parseFloat(e.target.value);
            });

            // Action buttons
            document.getElementById('generate-report-btn')?.addEventListener('click', () => this.generateReport());
            document.getElementById('add-to-project-btn')?.addEventListener('click', () => this.addToProject());
        },

        // =========================================================
        // 4. FAN SELECTION HANDLER
        // =========================================================

        handleFanSelection: function (event) {
            const card = event.currentTarget;
            const fanModel = card.dataset.fanModel;

            // Remove active state from all cards
            document.querySelectorAll('.fan-model-card').forEach(c => {
                c.classList.remove('active');
            });

            // Add active state to selected card
            card.classList.add('active');

            // Update state
            this.state.selectedFanModel = fanModel;

            // Update UI
            this.updateSelectedFanDisplay();
            this.updateSpecGrid();
            this.updateChart();

            // Trigger animation
            this.pulseElement(card);
            this.showNotification(`✓ ${this.fanDatabase[fanModel].name} selected`);
        },

        // =========================================================
        // 5. CHART RENDERING
        // =========================================================

        renderInitialChart: function () {
            const ctx = document.getElementById('performance-chart')?.getContext('2d');
            if (!ctx) return;

            this.generateChartData((fanCurve, systemCurve) => {
                this.chart = new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: fanCurve.map(point => point.x.toLocaleString('en-IN')),
                        datasets: [
                            {
                                label: 'Fan Curve',
                                data: fanCurve.map(point => point.y),
                                borderColor: '#0072ff',
                                backgroundColor: 'rgba(0, 114, 255, 0.1)',
                                borderWidth: 3,
                                fill: true,
                                tension: 0.4,
                                pointRadius: 0,
                                pointHoverRadius: 6,
                                pointBackgroundColor: '#0072ff',
                                pointBorderColor: '#00f0ff',
                                pointBorderWidth: 2,
                                shadowColor: 'rgba(0, 114, 255, 0.5)',
                                shadowBlur: 10
                            },
                            {
                                label: 'System Curve',
                                data: systemCurve.map(point => point.y),
                                borderColor: '#ff007f',
                                backgroundColor: 'rgba(255, 0, 127, 0.1)',
                                borderWidth: 3,
                                fill: true,
                                tension: 0.4,
                                pointRadius: 0,
                                pointHoverRadius: 6,
                                pointBackgroundColor: '#ff007f',
                                pointBorderColor: '#00f0ff',
                                pointBorderWidth: 2
                            },
                            {
                                label: 'Duty Point (DP)',
                                data: [
                                    {
                                        x: this.state.operatingParams.airflow,
                                        y: this.state.operatingParams.pressure
                                    }
                                ],
                                type: 'scatter',
                                pointRadius: 8,
                                pointBackgroundColor: '#00f0ff',
                                pointBorderColor: '#ff6b00',
                                pointBorderWidth: 3,
                                pointStyle: 'star'
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {
                            legend: {
                                display: true,
                                labels: {
                                    color: '#b0b0b0',
                                    font: { family: "'Inter', sans-serif", size: 12, weight: '600' },
                                    padding: 15,
                                    usePointStyle: true,
                                    pointStyle: 'circle'
                                }
                            },
                            tooltip: {
                                backgroundColor: 'rgba(15, 32, 53, 0.95)',
                                borderColor: '#00f0ff',
                                borderWidth: 1,
                                titleColor: '#00f0ff',
                                bodyColor: '#b0b0b0',
                                padding: 10,
                                cornerRadius: 8,
                                titleFont: { weight: 'bold', size: 12 },
                                bodyFont: { size: 11 }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                max: 3000,
                                ticks: {
                                    color: '#7a7a7a',
                                    font: { family: "'Inter', sans-serif", size: 11 }
                                },
                                grid: {
                                    color: 'rgba(0, 240, 255, 0.05)',
                                    lineWidth: 1
                                },
                                title: {
                                    display: true,
                                    text: 'Static Pressure (Pa)',
                                    color: '#00f0ff',
                                    font: { weight: 'bold', size: 12 }
                                }
                            },
                            x: {
                                ticks: {
                                    color: '#7a7a7a',
                                    font: { family: "'Inter', sans-serif", size: 11 }
                                },
                                grid: {
                                    color: 'rgba(0, 240, 255, 0.05)',
                                    lineWidth: 1
                                },
                                title: {
                                    display: true,
                                    text: 'Airflow (m³/h)',
                                    color: '#00f0ff',
                                    font: { weight: 'bold', size: 12 }
                                }
                            }
                        }
                    }
                });
            });
        },

        generateChartData: function (callback) {
            const fanCurve = [];
            const systemCurve = [];

            for (let x = 0; x <= 20000; x += 1000) {
                // Fan curve: quadratic relationship
                fanCurve.push({
                    x: x,
                    y: 2500 - (x / 10000) ** 2 * 2500
                });

                // System curve: pressure increases with flow
                systemCurve.push({
                    x: x,
                    y: (x / 10000) * 2000
                });
            }

            callback(fanCurve, systemCurve);
        },

        updateChart: function () {
            if (!this.chart) return;

            this.generateChartData((fanCurve, systemCurve) => {
                this.chart.data.labels = fanCurve.map(point => point.x.toLocaleString('en-IN'));
                this.chart.data.datasets[0].data = fanCurve.map(point => point.y);
                this.chart.data.datasets[1].data = systemCurve.map(point => point.y);
                this.chart.data.datasets[2].data = [
                    {
                        x: this.state.operatingParams.airflow,
                        y: this.state.operatingParams.pressure
                    }
                ];
                this.chart.update();
            });
        },

        // =========================================================
        // 6. UI UPDATE METHODS
        // =========================================================

        updateSelectedFanDisplay: function () {
            const fanData = this.fanDatabase[this.state.selectedFanModel];
            const banner = document.getElementById('selected-fan-banner');
            
            if (banner) {
                banner.innerHTML = `
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h3 class="mb-1">${fanData.name}</h3>
                            <p class="text-muted mb-0">Model: <strong>${this.state.selectedFanModel}</strong></p>
                        </div>
                        <div class="text-right">
                            <div class="badge badge-cyan">Active Selection</div>
                        </div>
                    </div>
                `;
            }
        },

        updateSpecGrid: function () {
            const fanData = this.fanDatabase[this.state.selectedFanModel];
            const specGrid = document.getElementById('spec-metrics-grid');

            if (specGrid) {
                specGrid.innerHTML = `
                    <div class="spec-card glass-panel-sm">
                        <div class="spec-label">Impeller Diameter</div>
                        <div class="spec-value">${fanData.impellerDia} mm</div>
                    </div>
                    <div class="spec-card glass-panel-sm">
                        <div class="spec-label">Speed</div>
                        <div class="spec-value">${fanData.speed} RPM</div>
                    </div>
                    <div class="spec-card glass-panel-sm">
                        <div class="spec-label">Power</div>
                        <div class="spec-value">${fanData.power} kW</div>
                    </div>
                    <div class="spec-card glass-panel-sm">
                        <div class="spec-label">Efficiency</div>
                        <div class="spec-value">${fanData.efficiency}%</div>
                    </div>
                    <div class="spec-card glass-panel-sm">
                        <div class="spec-label">Noise Level</div>
                        <div class="spec-value">${fanData.noise} dB(A)</div>
                    </div>
                    <div class="spec-card glass-panel-sm">
                        <div class="spec-label">Material</div>
                        <div class="spec-value">${fanData.material}</div>
                    </div>
                `;
            }
        },

        // =========================================================
        // 7. FORM VALIDATION
        // =========================================================

        setupInputValidation: function () {
            const inputs = document.querySelectorAll('.form-control[type="number"]');
            inputs.forEach(input => {
                input.addEventListener('blur', (e) => {
                    const value = parseFloat(e.target.value);
                    const min = parseFloat(e.target.min);
                    const max = parseFloat(e.target.max);

                    if (value < min || value > max) {
                        e.target.classList.add('is-invalid');
                        this.showNotification(`⚠ Value must be between ${min} and ${max}`, 'warning');
                    } else {
                        e.target.classList.remove('is-invalid');
                    }
                });
            });
        },

        // =========================================================
        // 8. ACTION HANDLERS
        // =========================================================

        generateReport: function () {
            const fanData = this.fanDatabase[this.state.selectedFanModel];
            
            const reportData = {
                fanModel: this.state.selectedFanModel,
                fanName: fanData.name,
                operatingParams: this.state.operatingParams,
                applicationDetails: this.state.applicationDetails,
                specs: fanData,
                timestamp: new Date().toLocaleString('en-IN')
            };

            console.log('📄 Report Generated:', reportData);
            this.showNotification('✓ Report generated successfully!');
            
            // Optional: Trigger download or API call
            // this.downloadReport(reportData);
        },

        addToProject: function () {
            const fanData = this.fanDatabase[this.state.selectedFanModel];
            
            const projectData = {
                fanModel: this.state.selectedFanModel,
                fanName: fanData.name,
                airflow: this.state.operatingParams.airflow,
                pressure: this.state.operatingParams.pressure,
                addedAt: new Date().toISOString()
            };

            console.log('✓ Added to Project:', projectData);
            this.showNotification('✓ Fan added to project successfully!', 'success');

            // Optional: Send to server
            // this.sendToServer('/api/project/add-fan', projectData);
        },

        // =========================================================
        // 9. UTILITIES
        // =========================================================

        pulseElement: function (element) {
            element.style.animation = 'none';
            setTimeout(() => {
                element.style.animation = 'pulse 0.6s ease-out';
            }, 10);
        },

        showNotification: function (message, type = 'info') {
            const notificationContainer = document.getElementById('notification-container') 
                || this.createNotificationContainer();

            const notification = document.createElement('div');
            notification.className = `alert alert-${type} alert-notification`;
            notification.textContent = message;
            notification.style.animation = 'slideIn 0.3s ease-out';

            notificationContainer.appendChild(notification);

            setTimeout(() => {
                notification.style.animation = 'slideOut 0.3s ease-out';
                setTimeout(() => notification.remove(), 300);
            }, 3000);
        },

        createNotificationContainer: function () {
            const container = document.createElement('div');
            container.id = 'notification-container';
            container.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                width: 300px;
            `;
            document.body.appendChild(container);
            return container;
        }
    };

    // =========================================================
    // 10. ANIMATIONS (CSS-in-JS fallback)
    // =========================================================

    const style = document.createElement('style');
    style.textContent = `
        @keyframes pulse {
            0% { transform: scale(1); opacity: 1; }
            50% { transform: scale(1.05); }
            100% { transform: scale(1); opacity: 1; }
        }

        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(100%);
                opacity: 0;
            }
        }

        .alert-notification {
            padding: 12px 16px;
            border-radius: 8px;
            margin-bottom: 10px;
            font-weight: 600;
            border-left: 4px solid;
        }

        .alert-info {
            background: rgba(0, 114, 255, 0.1);
            border-left-color: #0072ff;
            color: #0072ff;
        }

        .alert-success {
            background: rgba(0, 200, 100, 0.1);
            border-left-color: #00c864;
            color: #00c864;
        }

        .alert-warning {
            background: rgba(255, 107, 0, 0.1);
            border-left-color: #ff6b00;
            color: #ff6b00;
        }

        .fan-model-card {
            transition: all 300ms ease;
        }

        .fan-model-card.active {
            border-color: #00f0ff !important;
            box-shadow: 0 0 25px rgba(0, 240, 255, 0.4) !important;
        }
    `;
    document.head.appendChild(style);

    // =========================================================
    // 11. EXPORT & INITIALIZE
    // =========================================================

    window.CentriSelect = CentriSelect;

    // Auto-initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => CentriSelect.init());
    } else {
        CentriSelect.init();
    }

})();
