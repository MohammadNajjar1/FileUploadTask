Tooltip = {
    init() {
        for (const element of document.querySelectorAll('[data-bs-toggle="tooltip"]')) {
            new bootstrap.Tooltip(element);
        }
    }
};
