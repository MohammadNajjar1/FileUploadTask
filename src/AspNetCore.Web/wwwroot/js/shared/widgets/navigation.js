Navigation = {
    init() {
        const maxWidth = 100;
        const navigation = this;

        navigation.element = document.querySelector(".navigation");

        if (navigation.element) {
            navigation.nodes = navigation.element.querySelectorAll("li");
            navigation.search = navigation.element.querySelector("input");
            navigation.activeSubmenus = navigation.element.querySelectorAll(".active.submenu");

            navigation.search.addEventListener("input", function () {
                navigation.filter(this.value);
            });

            [].map.call(navigation.nodes, node => node)
                .filter(node => node.classList.contains("submenu"))
                .forEach(submenu => {
                    submenu.firstElementChild.addEventListener("click", e => {
                        e.preventDefault();

                        submenu.classList.toggle("open");

                        if (navigation.element.clientWidth < maxWidth) {
                            [].forEach.call(submenu.parentElement.children, sibling => {
                                if (sibling !== submenu) {
                                    sibling.classList.remove("open");
                                }
                            });
                        }
                    });
                });

            window.addEventListener("resize", () => {
                if (navigation.element.clientWidth < maxWidth) {
                    navigation.closeAll();
                }
            });

            window.addEventListener("click", e => {
                if (navigation.element.clientWidth < maxWidth) {
                    let target = e && e.target;

                    while (target && !/navigation/.test(target.className)) {
                        target = target.parentElement;
                    }

                    if (!target && target !== window) {
                        navigation.closeAll();
                    }
                }
            });

            if (navigation.element.clientWidth < maxWidth) {
                navigation.closeAll();
            }
        }
    },

    filter(term) {
        this.search.value = term;

        for (const node of this.nodes) {
            node.classList.remove("open");
            node.style.display = "";
        }

        if (term) {
            [].forEach.call(this.element.lastElementChild.children, node => {
                filterNode(node, term.toLowerCase());
            });
        } else {
            for (const node of this.activeSubmenus) {
                node.classList.add("open");
            }
        }

        function filterNode(element, search) {
            const text = element.firstElementChild.querySelector(".text").textContent.toLowerCase();
            const match = text.includes(search);

            if (!match && element.classList.contains("submenu")) {
                const { children } = element.lastElementChild;

                for (let i = 0; i < children.length; i++) {
                    if (filterNode(children[i], search)) {
                        element.classList.add("open");
                    }
                }
            }

            if (!match && !element.classList.contains("open")) {
                element.style.display = "none";
            }

            return match;
        }
    },

    closeAll() {
        for (const node of this.nodes) {
            node.classList.remove("open");
        }
    }
};
