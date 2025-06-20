document.addEventListener('DOMContentLoaded', function () {
    const container = document.querySelector('#autocomplete-container');

    if (!container) return;

    const items = JSON.parse(container.dataset.autocompleteItems || '[]');

    accessibleAutocomplete({
        element: container,
        id: 'autocomplete-input',
        source: items
    });
});
