class MvcTree {
    constructor(element, options = {}) {
        const tree = this;
        const treeElement = tree.findTree(element);

        if (treeElement.dataset.id) {
            return MvcTree.instances[parseInt(treeElement.dataset.id)].set(options);
        }

        tree.values = {};
        tree.element = treeElement;
        tree.ids = tree.element.querySelector(".mvc-tree-ids");
        tree.view = tree.element.querySelector(".mvc-tree-view");
        tree.element.dataset.id = MvcTree.instances.length.toString();
        tree.readonly = treeElement.classList.contains("mvc-tree-readonly");

        [].forEach.call(tree.ids.children, input => {
            tree.values[input.value] = input;
        });

        [].forEach.call(tree.view.children, branch => {
            tree.update(branch, true);
        });

        MvcTree.instances.push(tree);
        tree.set(options);
        tree.bind();
    }

    set({ readonly }) {
        const tree = this;

        tree.readonly = readonly == null ? tree.readonly : readonly;

        if (tree.readonly) {
            tree.element.classList.add("mvc-tree-readonly");
        } else {
            tree.element.classList.remove("mvc-tree-readonly");
        }

        return tree;
    }

    uncheck(branch) {
        let parent = branch.parentElement.parentElement;

        this.uncheckNode(branch);

        for (const node of branch.querySelectorAll("li")) {
            this.uncheckNode(node);
        }

        while (parent.tagName == "LI") {
            this.update(parent);

            parent = parent.parentElement.parentElement;
        }
    }
    check(branch) {
        let parent = branch.parentElement.parentElement;

        this.checkNode(branch);

        for (const node of branch.querySelectorAll("li")) {
            this.checkNode(node);
        }

        while (parent.tagName == "LI") {
            this.update(parent);

            parent = parent.parentElement.parentElement;
        }
    }

    update(branch, recursive) {
        if (branch.lastElementChild.tagName == "UL") {
            let checked = 0;
            let unchecked = 0;
            const { children } = branch.lastElementChild;

            [].forEach.call(children, node => {
                const states = recursive ? this.update(node, recursive) : node.classList;

                if (!states.contains("mvc-tree-undetermined")) {
                    if (states.contains("mvc-tree-checked")) {
                        checked++;
                    } else {
                        unchecked++;
                    }
                }
            });

            if (children.length == unchecked) {
                branch.classList.remove("mvc-tree-checked");
                branch.classList.remove("mvc-tree-undetermined");
            } else if (children.length == checked) {
                branch.classList.add("mvc-tree-checked");
                branch.classList.remove("mvc-tree-undetermined");
            } else {
                branch.classList.add("mvc-tree-undetermined");
            }
        }

        return branch.classList;
    }
    uncheckNode(node) {
        node.classList.remove("mvc-tree-checked");
        node.classList.remove("mvc-tree-undetermined");

        if (node.dataset.id && this.values[node.dataset.id]) {
            this.ids.removeChild(this.values[node.dataset.id]);

            delete this.values[node.dataset.id];
        }
    }
    checkNode(node) {
        node.classList.add("mvc-tree-checked");
        node.classList.remove("mvc-tree-undetermined");

        if (node.dataset.id && !this.values[node.dataset.id]) {
            const input = document.createElement("input");

            input.name = this.element.dataset.for;
            input.value = node.dataset.id;
            input.type = "hidden";

            this.values[node.dataset.id] = input;
            this.ids.appendChild(input);
        }
    }

    collapse(branch) {
        branch.classList.add("mvc-tree-collapsed");
    }
    expand(branch) {
        branch.classList.remove("mvc-tree-collapsed");
    }

    findTree(element) {
        const tree = element.closest(".mvc-tree");

        if (!tree) {
            throw new Error("Tree can only be created from within mvc-tree structure.");
        }

        return tree;
    }
    bind() {
        const tree = this;

        for (const node of tree.element.querySelectorAll("a")) {
            const branch = node.parentElement;

            node.addEventListener("click", e => {
                e.preventDefault();

                if (!tree.readonly) {
                    if (branch.classList.contains("mvc-tree-checked")) {
                        tree.uncheck(branch);
                    } else {
                        tree.check(branch);
                    }
                }
            });

            node.addEventListener("keyup", e => {
                if (e.key == "ArrowLeft") {
                    this.collapse(branch);
                } else if (e.key == "ArrowRight") {
                    this.expand(branch);
                }
            });
        }

        for (const branch of tree.element.querySelectorAll(".mvc-tree-branch > i")) {
            branch.addEventListener("click", () => {
                const parent = branch.parentElement;

                if (parent.classList.contains("mvc-tree-collapsed")) {
                    tree.expand(parent);
                } else {
                    tree.collapse(parent);
                }
            });
        }
    }
}

MvcTree.instances = [];
