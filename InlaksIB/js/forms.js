$(document).ready(function(){
    // 
   


    ///$("#npick").select2();

    // Select
   // $('.selectpicker').selectpicker();

    $('.select2').select2();

    // Tags
    $("#tags").tags({
        suggestions: ["alpha", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel", "india"],
        tagData: ["juliett", "kilo"]
    });

    // Editable
   // $('.editable').editable();

    // Wizard
    $('#rootwizard').bootstrapWizard();

    // Mask
    if ($('[data-mask]')
        .length) {
        $('[data-mask]')
            .each(function () {

                $this = $(this);
                var mask = $this.attr('data-mask') || 'error...',
                    mask_placeholder = $this.attr('data-mask-placeholder') || 'X';

                $this.mask(mask, {
                    placeholder: mask_placeholder
                });
            })
    }

   
  //  google.load("visualization", "1", { packages: ["corechart", "charteditor"] });


});