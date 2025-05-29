// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// AJAX call to the IngredientController.cs:  preventing duplicate IngredientName, used in CreateIngredient.cshtml & EditIngredient.cshtml
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