$(document).ready(function () {

    // AJAX call to the IngredientController.cs:  preventing duplicate IngredientName, used in CreateIngredient.cshtml
    $("#IngredientName").on("blur", function () {
        var name = $(this).val();
        $.get("/Ingredient/CheckDuplicate?name=" + name, function (data) {
            if (data.exists) {
                $("#ingredient-error").text("This ingredient already exists.");
            } else {
                $("#ingredient-error").text("");
            }
        });
    });

    // Initialize Bootstrap ToolTips for entire application
        /* to utilize this code, the element must include the attributes: data-bs-toggle="tooltip" title="What you want to say" */
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
})