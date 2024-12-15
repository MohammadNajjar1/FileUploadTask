// Widgets init
(function () {
    NumberConverter.init();
    Datepicker.init();
    Navigation.init();
    Validator.init();
    Tooltip.init();
    Alerts.init();
    Lookup.init();
    Grid.init();
    Tree.init();
})();

// Read only binding
(function () {
    for (const widget of document.querySelectorAll(".widget-box.readonly")) {
        for (const element of widget.querySelectorAll("textarea,select,input")) {
            element.readOnly = true;
            element.tabIndex = -1;
        }

        for (const element of widget.querySelectorAll(".mvc-lookup")) {
            new MvcLookup(element, { readonly: true });
        }

        for (const element of widget.querySelectorAll(".mvc-tree")) {
            new MvcTree(element, { readonly: true });
        }
    }

    window.addEventListener("change", e => {
        if (e.target && e.target.tagName === "SELECT" && e.target.hasAttribute("readonly")) {
            e.target.value = e.target.dataset.originalValue;
        }
    });

    for (const element of document.querySelectorAll("select")) {
        element.dataset.originalValue = element.value;
    }

    window.addEventListener("click", e => {
        if (e.target && e.target.readOnly) {
            e.preventDefault();
        }
    });
})();

// Input focus binding
(function () {
    const invalid = document.querySelector(".input-validation-error[type=text]:not([readonly]):not(.date-picker):not(.date-time-picker)");

    if (invalid) {
        invalid.setSelectionRange(0, invalid.value.length);
        invalid.focus();
    } else {
        const input = document.querySelector("input[type=text]:not([readonly]):not(.date-picker):not(.date-time-picker)");

        if (input) {
            input.setSelectionRange(0, input.value.length);
            input.focus();
        }
    }
})();
