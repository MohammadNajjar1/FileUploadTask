Tree = {
    init() {
        for (const element of document.querySelectorAll(".mvc-tree")) {
            new MvcTree(element);
        }
    }
};
