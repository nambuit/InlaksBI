


function processrole(object){

    var roleid = $(object).val();

    if (roleid == "null") {
        $("#rselect").empty('');
    }

    var myrand = Math.floor(Math.random() * 1000000);

    var head = "<div id='rselect' class='form-group'><div class='col-md-10'><h4>Select Resources</h4>" +
                    "<p>" +
                     "   <select id='rresources' name='resources' class='selectpicker' multiple>";


    var tail = "    </select> </p></div></div>";

    jQuery.ajax({
        url: "RoleResources/" + roleid + "?rand=" + myrand,
        type: "GET",
        success: function (result) {


            var data = $.parseJSON(result);
            var body = "";
            body = body.concat(head);
            for (var i = 0; i < data.length; i++) {
                body = body.concat("<option value='" + data[i].ResourceID + "' " + data[i].selected + ">" + data[i].ResourceName + "</option>");
            }
            body = body.concat(tail);


            $("#rselect").replaceWith(body);

            $('.selectpicker').selectpicker();

            
        }
    });



}


function hide(object) {
    if (object.value == "DATABASE") {
        jQuery("#ldap").hide();
      //  jQuery("#server").val("");

    }
    else {

        jQuery("#ldap").show();
    }
}