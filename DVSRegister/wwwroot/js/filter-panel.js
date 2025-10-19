// Filter Panel Component JavaScript
(function () {
  "use strict";

  function FilterPanel(container) {
    this.container = container;
    this.filterButton = container.querySelector(".app-c-filter-panel__button");
    this.filterContent = container.querySelector(
      ".app-c-filter-panel__content"
    );
    this.isOpen = false;

    this.init();
  }

  FilterPanel.prototype.init = function () {
    if (!this.filterButton || !this.filterContent) {
      return;
    }

    // Set initial state
    this.filterContent.setAttribute("hidden", "");
    this.filterButton.setAttribute("aria-expanded", "false");

    // Bind events
    this.filterButton.addEventListener("click", this.togglePanel.bind(this));

    // Handle Escape key to close panel
    document.addEventListener(
      "keydown",
      function (event) {
        if (event.key === "Escape" && this.isOpen) {
          this.closePanel();
          this.filterButton.focus();
        }
      }.bind(this)
    );

    // Close panel when clicking outside
    document.addEventListener(
      "click",
      function (event) {
        if (this.isOpen && !this.container.contains(event.target)) {
          this.closePanel();
        }
      }.bind(this)
    );
  };

  FilterPanel.prototype.togglePanel = function (event) {
    event.preventDefault();

    if (this.isOpen) {
      this.closePanel();
    } else {
      this.openPanel();
    }
  };

  FilterPanel.prototype.openPanel = function () {
    this.isOpen = true;
    this.filterContent.removeAttribute("hidden");
    this.filterButton.setAttribute("aria-expanded", "true");

    // Focus on first interactive element in the panel
    var firstInput = this.filterContent.querySelector("input, select, button");
    if (firstInput) {
      setTimeout(function () {
        firstInput.focus();
      }, 100);
    }
  };

  FilterPanel.prototype.closePanel = function () {
    this.isOpen = false;
    this.filterContent.setAttribute("hidden", "");
    this.filterButton.setAttribute("aria-expanded", "false");
  };

  // Initialize all filter panels on page load
  function initFilterPanels() {
    var filterPanels = document.querySelectorAll(
      '[data-module="filter-panel"]'
    );

    Array.prototype.forEach.call(filterPanels, function (panel) {
      new FilterPanel(panel);
    });
  }

  // Auto-initialize when DOM is ready
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initFilterPanels);
  } else {
    initFilterPanels();
  }

  // Make it available globally for manual initialization
  window.FilterPanel = FilterPanel;
})();
