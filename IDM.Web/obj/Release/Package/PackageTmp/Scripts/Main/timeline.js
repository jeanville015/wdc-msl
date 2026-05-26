// Timeline Module
var TimelineModule = (function () {
    'use strict';

    function init() {
        bindEvents();
        initializeAnimations();
        loadManualVersions();
    }

    function loadManualVersions() {
        // Manually add versions here
        const manualVersions = [
            {
                version: "2.1.0.0",
                title: "&nbsp; Applying Tool Entries",
                date: "TBD",
                description: "Tool Entry can be uploaded and approved.",
                status: "planned", // planned, in-progress, completed, delayed
                features: [
                    { name: "Tool Type Maintenance", status: "completed" },
                    { name: "Tool Maintenance", status: "completed" }, 
                    { name: "Analysis Maintenance", status: "completed" },
                    { name: "Tool Setting Maintenance", status: "pending" },
                    { name: "Table for items that need to approve/disapprove", status: "pending" },
                    { name: "Page for items", status: "pending" },
                    { name: "Staging table", status: "pending" },
                    { name: "View for approved items", status: "pending" }
                ],
                badgeClass: "bg-warning",
                dotClass: "bg-warning"
            },
            {
                version: "2.0.0.0",
                title: "&nbsp; Structure update",
                date: "Fri, Nov. 21, 2025",
                description: "Applying Clean Architecture for easily maintaining the code",
                status: "completed", // planned, in-progress, completed, delayed
                features: [
                    { name: "Separation of Services and business logic on web application", status: "completed" },
                    { name: "Separation of Repositories", status: "completed" },
                    { name: "Applying DTO and Domain Entities", status: "completed" },
                    { name: "Dividing the projects to from 1 to Presentation, Domain, Application, Infrastructure", status: "completed" }
                ],
                badgeClass: "bg-success",
                dotClass: "bg-success"
            },
            {
                version: "1.0.0.0",
                title: "&nbsp; Initial Release",
                date: "Thu, Apr 30, 2025",
                description: "In version 1.0.0.0, the initial release includes the ability to maintain various elements such as parameters, materials, UOMs, suppliers, manufacturers, area, testing site, user role and users. It also supports entering parameter data per material, which can be sent to the MFG, EDCSPC, and IDM databases. Users log in to the application using their Active Directory accounts and must also be registered in the IDM system.",
                status: "completed", // planned, in-progress, completed, delayed
                features: [
                    { name: "Material Data Entry", status: "completed" },
                    { name: "Master Data Maintenance", status: "completed" },
                    { name: "Material Management", status: "completed" },
                    { name: "MQ Integration (MFG, EDCSPC, IDM)", status: "completed" },
                    { name: "Active Directory Authentication", status: "completed" },
                    { name: "User Role Management", status: "completed" }
                ],
                badgeClass: "bg-success",
                dotClass: "bg-success"
            }
        ];

        // Add all manual versions to timeline
        manualVersions.forEach(function(version) {
            addTimelineItem(version);
        });
    }

    function bindEvents() {
        // Add hover effects to timeline cards
        $('.timeline-card').on('mouseenter', function () {
            $(this).find('.timeline-dot').addClass('pulse');
        }).on('mouseleave', function () {
            $(this).find('.timeline-dot').removeClass('pulse');
        });

        // Add click to expand functionality (optional)
        $('.timeline-card').on('click', function () {
            // Toggle expanded state or navigate to details
            console.log('Timeline card clicked');
        });
    }

    function initializeAnimations() {
        // Trigger animations when timeline comes into view
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.animationPlayState = 'running';
                }
            });
        }, {
            threshold: 0.1
        });

        // Observe all timeline items
        $('.timeline-item').each(function () {
            observer.observe(this);
        });
    }

    function addTimelineItem(versionData) {
        // Get status icon and color based on feature status
        function getStatusIcon(status) {
            switch(status) {
                case 'completed':
                    return '<i class="fas fa-check-circle text-success me-2"></i>';
                case 'in-progress':
                    return '<i class="fas fa-spinner fa-spin text-primary me-2"></i>';
                case 'pending':
                    return '<i class="far fa-circle text-muted me-2"></i>';
                case 'delayed':
                    return '<i class="fas fa-exclamation-circle text-warning me-2"></i>';
                case 'cancelled':
                    return '<i class="fas fa-times-circle text-danger me-2"></i>';
                default:
                    return '<i class="far fa-circle text-muted me-2"></i>';
            }
        }

        // Dynamic function to add new timeline items
        const timelineItem = `
            <div class="timeline-item">
                <div class="timeline-dot ${versionData.dotClass || 'bg-primary'}"></div>
                <div class="timeline-content">
                    <div class="card shadow-sm timeline-card">
                        <div class="card-header bg-light border-0">
                            <div class="row align-items-center">
                                <div class="col-md-6">
                                    <div class="d-flex align-items-center">
                                        <span class="version-badge badge ${versionData.badgeClass || 'bg-primary'} me-2">${versionData.version}</span>
                                        <h5 class="card-title mb-0">${versionData.title}</h5>
                                    </div>
                                </div>
                                <div class="col-md-6 text-md-end">
                                    <small class="text-muted">
                                        <i class="far fa-calendar me-1"></i>
                                        ${versionData.date}
                                    </small>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            <p class="card-text text-muted mb-3">${versionData.description}</p>
                            ${versionData.features ? `
                                <div class="release-features">
                                    <h6 class="text-muted mb-2">Key Features:</h6>
                                    <ul class="feature-list">
                                        ${versionData.features.map(feature => `
                                            <li>${getStatusIcon(feature.status)}${feature.name}</li>
                                        `).join('')}
                                    </ul>
                                </div>
                            ` : ''}
                        </div>
                    </div>
                </div>
            </div>
        `;

        $('.timeline-items').append(timelineItem);
        
        // Re-bind events for new items
        bindEvents();
    }

    function filterTimeline(filterType) {
        // Filter timeline items by version type or date
        // Implementation for future filtering functionality
        console.log('Filter timeline by:', filterType);
    }

    return {
        init: init,
        addTimelineItem: addTimelineItem,
        filterTimeline: filterTimeline
    };
})();

$(document).ready(function () {
    TimelineModule.init();
});
