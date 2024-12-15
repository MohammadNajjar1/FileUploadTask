Lookup = {
    init() {
        for (const element of document.querySelectorAll(".mvc-lookup")) {
            new MvcLookup(element);
        }
    }
};
