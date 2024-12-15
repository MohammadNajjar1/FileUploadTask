Grid = {
    init() {
        MvcGridNumberFilter.prototype.isValid = function (value) {
            return !value || !isNaN(NumberConverter.parse(value));
        };

        for (const element of document.querySelectorAll(".mvc-grid")) {
            new MvcGrid(element);
        }
    }
};
