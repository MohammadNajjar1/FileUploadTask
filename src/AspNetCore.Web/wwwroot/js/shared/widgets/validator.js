Validator = {
    init() {
        Wellidate.default.rules.number.groupSeparator = NumberConverter.group;
        Wellidate.default.rules.number.decimalSeparator = NumberConverter.decimal;

        Wellidate.default.rules.date.isValid = function () {
            const value = this.normalizeValue();
            const dateFormat = moment().locale(document.documentElement.lang)._locale._longDateFormat.L;
            const dateTimeFormat = moment().locale(document.documentElement.lang)._locale._longDateFormat.LLL;

            return !value || (moment(value, dateFormat).isValid() || moment(value, dateTimeFormat).isValid());
        };

        Wellidate.default.rules.min.isValid = function () {
            const value = this.normalizeValue();

            return !value || parseFloat(this.value) <= NumberConverter.parse(value);
        };

        Wellidate.default.rules.max.isValid = function () {
            const value = this.normalizeValue();

            return !value || NumberConverter.parse(value) <= parseFloat(this.value);
        };

        Wellidate.default.rules.range.isValid = function () {
            const range = this;
            const value = this.normalizeValue();
            const number = NumberConverter.parse(value);

            return !value || (range.min == null || parseFloat(range.min) <= number) && (range.max == null || number <= parseFloat(range.max));
        };

        Wellidate.default.rules.lower.isValid = function () {
            const value = this.normalizeValue();
            const number = NumberConverter.parse(value);

            return !value || number < parseFloat(this.than);
        };

        Wellidate.default.rules.greater.isValid = function () {
            const value = this.normalizeValue();
            const number = NumberConverter.parse(value);

            return !value || parseFloat(this.than) < number;
        };

        document.addEventListener("wellidate-error", e => {
            if (e.target.classList.contains("mvc-lookup-value")) {
                const { wellidate } = e.detail.validatable;
                const { group, search } = new MvcLookup(e.target);

                group.classList.add("mvc-lookup-invalid");
                search.classList.add(wellidate.inputErrorClass);
                search.classList.remove(wellidate.inputValidClass);
                search.classList.remove(wellidate.inputPendingClass);
            }
        });

        document.addEventListener("wellidate-success", e => {
            if (e.target.classList.contains("mvc-lookup-value")) {
                const { wellidate } = e.detail.validatable;
                const { group, search } = new MvcLookup(e.target);

                group.classList.remove("mvc-lookup-invalid");
                search.classList.add(wellidate.inputValidClass);
                search.classList.remove(wellidate.inputErrorClass);
                search.classList.remove(wellidate.inputPendingClass);
            }
        });

        for (const value of document.querySelectorAll(".mvc-lookup-value.input-validation-error")) {
            new MvcLookup(value).group.classList.add("mvc-lookup-invalid");
        }

        document.querySelectorAll("form").forEach(form => new Wellidate(form, {
            wasValidatedClass: "validated"
        }));
    }
};
