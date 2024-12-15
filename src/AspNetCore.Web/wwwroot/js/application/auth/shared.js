for (const tooltip of document.querySelectorAll("input + .input-error")) {
    tooltip.previousElementSibling.addEventListener("wellidate-error", e => {
        tooltip.dataset.bsOriginalTitle = e.detail.message;
    });
}
