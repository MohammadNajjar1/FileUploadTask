Datepicker = {
    init() {
        const dateFormat = moment().locale(document.documentElement.lang)._locale._longDateFormat.L;

        for (const date of document.querySelectorAll(".date-picker")) {
            rome(date, {
                styles: {
                    container: "rd-container date-container"
                },
                monthFormat: "YYYY MMMM",
                inputFormat: dateFormat,
                dayFormat: "D",
                time: false
            });
        }

        for (const date of document.querySelectorAll(".date-time-picker")) {
            rome(date, {
                styles: {
                    container: "rd-container date-time-container"
                },
                inputFormat: `${dateFormat} HH:mm`,
                monthFormat: "YYYY MMMM",
                timeInterval: 900,
                autoClose: false,
                dayFormat: "D"
            });
        }

        document.addEventListener("click", e => {
            if (e.target.classList.contains("date-picker-browser")) {
                setTimeout(() => {
                    rome(e.target.previousElementSibling).show();
                }, 10);
            }
        });
    }
};
