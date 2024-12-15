Alerts = {
    init() {
        this.element = document.querySelector(".alerts");

        if (this.element) {
            [].forEach.call(this.element.children, this.bind);
        }
    },

    show(alerts) {
        for (const alert of [].concat(alerts)) {
            const element = document.getElementById(alert.id) || emptyAlert();

            element.className = `alert ${getType(alert.type)} alert-dismissible fade show`;
            element.firstChild.textContent = alert.message || "";
            element.dataset.timeout = alert.timeout || 0;
            element.id = alert.id || "";

            this.element.appendChild(element);
            this.bind(element);
        }

        function emptyAlert() {
            const close = document.createElement("button");
            const alert = document.createElement("div");
            const message = document.createTextNode("");

            close.dataset.bsDismiss = "alert";
            close.className = "btn-close";
            close.type = "button";

            alert.append(message);
            alert.append(close);

            return alert;
        }

        function getType(id) {
            switch (id) {
                case 0:
                    return "alert-danger";
                case 1:
                    return "alert-warning";
                case 2:
                    return "alert-info";
                case 3:
                    return "alert-success";
                default:
                    return `alert-${id}`;
            }
        }
    },
    bind(alert) {
        if (alert.dataset.timeout > 0) {
            setTimeout(() => {
                alert.querySelector(".btn-close").click();
            }, alert.dataset.timeout);
        }
    },
    close(id) {
        const close = document.querySelector(`#${id} .btn-close`);

        if (close) {
            close.click();
        }
    },
    closeAll() {
        for (const close of this.element.querySelectorAll(".btn-close")) {
            close.click();
        }
    },

    clear() {
        this.element.innerHTML = "";
    }
};
