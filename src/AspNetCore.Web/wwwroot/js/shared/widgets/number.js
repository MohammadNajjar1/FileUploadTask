NumberConverter = {
    init() {
        const parts = new Intl.NumberFormat(document.documentElement.lang).formatToParts(12345.6);

        this.decimal = parts.find(part => part.type === "decimal").value;
        this.group = parts.find(part => part.type === "group").value;
        this.decimalRegex = new RegExp(`[${this.decimal}]`, "g");
        this.groupRegex = new RegExp(`[${this.group}]`, "g");
    },
    parse(value) {
        return parseFloat(value.replace(this.groupRegex, "").replace(this.decimalRegex, "."));
    },
    format(number, format) {
        const options = Object.assign({
            maximumFractionDigits: 20
        }, format);

        return new Intl.NumberFormat(document.documentElement.lang, options).format(number);
    }
};
