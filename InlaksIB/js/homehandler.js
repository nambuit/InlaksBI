


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






function processtable(object) {

    var tablename = $(object).val();

    var link = $(object).closest('.form-group').find('.link');
    var plink = $(object).closest('.form-group').find('.plink');
    var cselect = $(object).closest('.form-group').find('.cselect');
    var cpick = $(object).closest('.form-group').find('.cpick');

    $(link).empty();
    $(plink).empty();
    $(cselect).empty('');
    if (tablename == "none") {
        $(cselect).empty('');
        return;
    }

    var myrand = Math.floor(Math.random() * 1000000);

    var head = "  <div class='col-md-4 cselect'> <label for='" + $(cpick).attr('id') + "'>Columns</label>" +
                    
                     "   <select  id='" + $(cpick).attr('id')+  "' class='form-control  cpick' multiple>";


    var tail = "    </select></div>";

    jQuery.ajax({
        url: "DataSetColumns/" + tablename + "?rand=" + myrand,
        type: "GET",
        success: function (result) {


            var data = $.parseJSON(result);
            var body = "";
            body = body.concat(head);
            
            for (var i = 0; i < data.length; i++) {
                body = body.concat("<option value='" + data[i].Value + "'>" + data[i].Value + "</option>");
                $(link).append("<option value='" + data[i].Value + "'>" + data[i].Value + "</option>");
                $(plink).append("<option value='" + data[i].Value + "'>" + data[i].Value + "</option>");
            }
            body = body.concat(tail);


            $(cselect).replaceWith(body);

            $('.cpick').selectpicker();


        }
    });


}


function removeTable(object) {
    var group = $(object).closest('.tablegroup');

    var groups = $('.tablegroup').toArray();

    var lastid = $(groups[(groups.length-1)]).attr('id');

    var currid = $(group).attr('id');

    if ( lastid==currid ) {


        $(group).replaceWith('');
    }
    else {
       
        alertmsg("You cannot remove Tables with dependants. Consider removing dependant table first ", "Invalid Operation",false)

    }

}

function removeSet(object) {
    var currow = $(object).closest('.dsetrow');
    var datasetname = $(currow).find('.dsetname').text().trim();

    $.confirm({
        title: 'Confirm dataset delete',
        content: 'Dataset will be deleted?',
        buttons: {
            Ok: function () {

                var myrand = Math.floor(Math.random() * 1000000);

                $.ajax({
                    url: "DeleteDataSet/" + datasetname + "?rand=" + myrand,
                    type: "GET",
                    success: function (result) {

                        var msg = "";

                        switch (result.trim()) {
                            case "0":
                                msg = "Dataset deleted successfully.";
                                $(currow).replaceWith('');
                                break;
                            case "1":
                                msg = "This dataset cannot be deleted because of dependant resources";
                        }

                        $.alert({
                            title: 'Response Message',
                            content: msg,
                            buttons: {
                                Okay: function () {
                                  

                                }
                            }
                        });

                    }
                });
            },
            cancel: function () {

            }
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




function addTable() {

    var tbgrps = $('.tablegroup').toArray();

    var tablecount = tbgrps.length;

    var link = $("#group" + tablecount).find('.lselect');

    $(link).show();

    //var selectedtables = "";

    //for (var i = 0; i < tbgrps.length; i++) {
    //    var tbgrp = $(tbgrps[i]).find('.tpick');

    //}

    var curtable = tablecount + 1;
    var myrand = Math.floor(Math.random() * 1000000);

    var body = "<div class='form-group tablegroup' id='group" + curtable + "'><hr/>" +
       "<div class='col-md-1 delete'><button id='deltable' onclick='removeTable(this)' class='btn-danger'>Remove</button></div>" +
                "<div class='col-md-3 tselect'>"+
               "<label for='tpick"+curtable+"'>Table"+curtable+"</label>"+
            "<select onchange='processtable(this)' id='tpick"+curtable+"' class='form-control tpick select2'>";


    jQuery.ajax({
        url: "TableList?rand=" + myrand,
        type: "GET",
        success: function (result) {

            var data = $.parseJSON(result);
          
            $("#tpick" + curtable).append("<option value='none'>Select Table</option>")
            for (var i = 0; i < data.length; i++) {
               $("#tpick"+curtable).append("<option value='" + data[i].ID + "'>" + data[i].Value + "</option>");
               
            }
           


           

            $('.cpick').selectpicker();
            $('.select2').select2();

        }
    });






  
    var tail = "</select></div>"+
               " <div  class='cselect col-md-3'>"+
               " <label for='cpick"+curtable+"'>Columns</label>"+
               "  <select  id='cpick"+curtable+"' class='form-control cpick'>"+
               "  </select></div>"+
               "  <div class='col-md-3 plselect'>"+
               "    <label for='plink"+curtable+"'>Table"+(curtable-1)+" Link</label>"+
              " <select id='plink"+curtable+"' class='form-control select2 plink'></select></div>"+
                "  <div class='col-md-3 lselect' style='display:none'>" +
               "    <label for='link" + curtable + "'>Table" + (curtable + 1) + " Link</label>" +
              " <select id='link" + curtable + "' class='form-control select2 link'></select></div>"+
              "" +
              "</div>";


    body = body.concat(tail);


    $("#group" + (curtable - 1)).after(body);



}



function processDataSet(){


    var datasetname = $("#dsetname").val();

    if (datasetname.trim() == '') {
        alertmsg("DataSetName cannot be empty!", "Invalid Operation",false);
        return;
}
    var moduleid = $("#module").val();

    if (moduleid == undefined || moduleid == "null") {
        alertmsg("Please select target module", "Invalid Operation",false);
        return;
}

    var groups = $('.tablegroup').toArray();

    var datasetobject = [];

    for(var i=0;i<groups.length;i++){

        var tname = $(groups[i]).find('.tpick').val();

        if (tname == "none") {
            alertmsg("Please select Table" + (i + 1) + "", "Invalid Operation",false);
            return;
}

        var cols = $(groups[i]).find('.cpick').val();
        
        var prelink = $(groups[i]).find('.plink').val();


        var nextlink = $(groups[i]).find('.link').val();

        var Table = { "DataSetName":datasetname,"TableName": tname, "Columns": cols, "PreTable": prelink, "NxtTable": nextlink };

        datasetobject[i] = Table;

    }

    var data = JSON.stringify(datasetobject);

    jQuery.ajax({
        url: "CreateDataSet",
        type: "POST",
        data: $.param({ datasetdata:data,module:moduleid}),
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        success: function (result) {

            switch (result) {
                case "0":
                    
                    alertmsg("DataSet Created Successfully", "Response Message",true);
                    
                    break;
                case "1":
                    alertmsg("Duplicate table detected", "Invalid Operation",false);
                    break;
                default:
                    alertmsg(result, "Response Message",false);
                    break;

}

        }
    });


}



function alertmsg(msg,title,refresh) {

    $.alert({
        title: title,
        content: msg,
        buttons: {
            Okay: function () {
                if (refresh) {
                    window.location = window.location;
                }
            }
        }
    });

}