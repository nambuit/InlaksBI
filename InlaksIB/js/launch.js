$(document).ready(function () {



    var input2 = $("#pick").pickadate({
        selectMonths: true,
        selectYears: true
    });

    var  picker = input.pickadate('picker');


    $("#pick").on('change', function () {
        $(this).val(picker.get('select', 'dd-mmm-yyyy'));
    });

});