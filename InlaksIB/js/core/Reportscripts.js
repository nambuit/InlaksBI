var paramsvalue;    

var operatorslist = '<select class="form-control operator" >'+
         '<option value="=">EqualTo</option>'+
         '<option value="!=">NotEqualTo</option>'+
         '<option value="like">Contains</option>'+
         '<option value="not like">Not Contain</option>'+
         '<option value=">">GreaterThan</option>'+
         '<option value="<">LessThan</option>'+
         '<option value=">=">GreaterThanorEqualTo</option>'+
         '<option value="<=">LessThanorEqualTo</option>'+
     '</select>';

$('#example').dataTable();
$('#migtable').dataTable();
$('#input').dataTable();


$(document).ready(function () {
    $("#state").hide();
    $("#export").hide();
    $("#print").hide();
    $("#resourcelist").hide();
    $("#datasetlist").hide();
    $("#columnlist").hide();
    $("#reportview").hide();
   
    $("#finalcontrols").hide();
    localStorage.baseurl = $("#baseurl").val();
    localStorage.title = $("#title").val();
     
    paramsvalue = $("#params").val();


    if (paramsvalue != undefined) {

        if (paramsvalue == "No") {
            $.alert({
                title: 'Missing Report Configuration',
                content: "A Report is yet to be configured for this resource. Contact Admin",
                buttons: {
                    Okay: function () {
                        window.location = localStorage.baseurl;
                    }
                }
            });
        }
        else {

            $("#rptpanel").LoadingOverlay("show", {
                image: localStorage.baseurl+"img/gears.gif",
                color: "rgba(0, 0, 0, 0)"
            });


            var params = JSON.parse(paramsvalue); 

            var currentconfig = JSON.parse(params.pivotConfig);

            switch (params.UserConfig) {
                
                case "None":

                    break;

                case "BaseChanged":
                    $.alert({
                        title: 'Response Message',
                        content: "Your previously saved state for this Report is no longer applicable as the base configuration has changed since your last saved state.",
                        buttons: {
                            Okay: function () {
                             
                            }
                        }
                    });

                    break;

                default:
                    currentconfig = JSON.parse(params.UserConfig);
                    break;
            }

            var dataset = params.dataSet;

            var filters = params.Filters;

            var InstanceID = params.InstanceID;

            var formdata = $.param({ dataset: dataset, filters: JSON.stringify(filters) });

            $.ajax({
                url: localStorage.baseurl + "Report/getFilteredData",
                type: "POST",
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                data: formdata,
                success: function (result) {

                    if (result.trim() == "Failed") {

                        $.alert({
                            title: 'Response Message',
                            content: " Failed to Load DataSet. Contact Admin",
                            buttons: {
                                Okay: function () {
                                    $("#rptpanel").LoadingOverlay("hide");
                                }
                            }
                        });
                       
                    }
                    else{
                        var data = $.parseJSON(result);
                   

                        try {
                            function saveState(config) {
                                $("#state").show();
                                $("#print").show();
                                $('.pvtRendererArea').attr('id', 'pvt');
                                $('.pvtRenderer').attr('onchange', 'enableExport(this)');
                                $('.pvtTable').attr("id", "pvtTable");
                            
                           
                                //   $('.rowexpanded').removeClass('rowexpanded').addClass('rowcollapsed');
                                var config_copy = JSON.parse(JSON.stringify(config));
                                // alert(localStorage.currentconfig);
                                //delete some values which are functions
                                delete config_copy["aggregators"];
                                delete config_copy["renderers"];
                                //delete some bulky default values
                                delete config_copy["rendererOptions"];
                                delete config_copy["localeStrings"];
                                localStorage.setItem('pivotdatakey', JSON.stringify(config_copy));
                                // alert(localStorage.pivotdatakey);
                            }
                  
                            //      google.load("visualization", "1", { packages: ["corechart", "charteditor"] });
                           
                            var dataClass = $.pivotUtilities.SubtotalPivotData;
                           
                            var renderers = $.extend($.pivotUtilities.renderers,
                                                         $.pivotUtilities.subtotal_renderers,
                                                         $.pivotUtilities.gchart_renderers,
                                                          $.pivotUtilities.export_renderers

                                                          );
                           
                            currentconfig.onRefresh = saveState;
                            currentconfig.renderers = renderers;
                            currentconfig.dataClass = dataClass;
                            // currentconfig.rendererName = "Table With Subtotal";

                            currentconfig.rendererOptions = {
                                collapseRowsAt: 0,             
                          
                                collapseColsAt: 1
                                //arrowCollapsed: "[+] ",
                                //arrowExpanded: "[-] ",
                            };
                         
                            $("#output").pivotUI(data, currentconfig, true);
                   

                            //    var renderer = $.pivotUtilities.subtotal_renderers[currentconfig.rendererName];
                            //    currentconfig.renderer = renderer;
                            //    $("#output").pivot(data, currentconfig, true);
                            //}
                            $("#rptpanel").LoadingOverlay("hide");
                       
                          
                       
                            //var dataClass = $.pivotUtilities.SubtotalPivotData;
                           

                            //currentconfig.onRefresh = saveState;
                            //currentconfig.renderer = renderer;
             
                            //currentconfig.dataClass = dataClass;

                            //currentconfig.rendererOptions = {
                            //    c3: {
                            //        size: { height: 450, width: 450 }
                            //    },
                            //    collapseRowsAt: 0,
                            //    collapseColsAt: 0
                            //    //arrowCollapsed: "[+] ",
                            //    //arrowExpanded: "[-] ",
                            //};


                          

                       

                       

                      

                   


                        }
                        catch (Exception) {

                        }
                    }
          

                }


            });




        }
    }

       

 


    



});

function showresources(object) {
    var module = object.value;

    var myrand = Math.floor(Math.random() * 1000000);
    $("#resourcelist").show();
    $("#datasetlist").show();

    jQuery.ajax({
        url: "../getValuePair/resources?param=" + module + "&rand=" + myrand,
        type: "GET",
        success: function (result) {
            var data = $.parseJSON(result);

            $("#resource").empty();

            for(var i=0;i<data.length;i++){

                $("#resource").append("<option value='" + data[i].ID + "'>" + data[i].Value + "</option>");
            }
        }
    });


    jQuery.ajax({
        url: "../getValuePair/dataset?param=" + module + "&rand=" + myrand,
        type: "GET",
        success: function (result) {
          
            var data = $.parseJSON(result);

            $("#dataset").empty();

            for (var i = 0; i < data.length; i++) {

                $("#dataset").append("<option value='" + data[i].ID + "'>" + data[i].Value + "</option>");
            }
            $("#dataset").trigger('change');
        }
    });

   

}

function showColumns(object) {
    var dataset = object.value;
    var myrand = Math.floor(Math.random() * 1000000);
    $("#columnlist").show();

    jQuery.ajax({
        url: "../getFilterColumns/" + dataset + "?rand=" + myrand,
        type: "GET",
        success: function (result) {
            var data = $.parseJSON(result);

            $("#columns").empty();

            for (var i = 0; i < data.length; i++) {

                $("#columns").append("<tr class='filters'><td><input class='IsIncluded' Value='" + JSON.stringify(data[i]) + "'  type='checkbox' /></td><td  ><input class='ColumnName form-control'   Value='" + data[i].ColumnName + "' type='text' disabled='disabled' /></td><td>" + operatorslist +
                "</td><td><input  class='ColumnValue form-control' value='' type='text' /></td><td><input  class='DisplayName form-control' value='' type='text' /></td></tr>");
            }
        }
    });

}


function processSubmit(object){
    //var value = object.value;

    //if (value.trim() == "LoadReport") {
        $("#reportview").show();

        $("#reportview").LoadingOverlay("show", {
            image: localStorage.baseurl+"img/gears.gif",
            color: "rgba(0, 0, 0, 0)"
        });

       
           

        var filters = getFilters();



        var formdata = $.param({ dataset: $("#dataset").val(), filters: JSON.stringify(filters) });

        $.ajax({
            url: localStorage.baseurl+"Report/getFilteredData",
            type: "POST",
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            data: formdata,
            success: function (result) {
                try{
                function saveState(config) {
                    var config_copy = JSON.parse(JSON.stringify(config));
                   // alert(localStorage.currentconfig);
                    //delete some values which are functions
                    delete config_copy["aggregators"];
                    delete config_copy["renderers"];
                    //delete some bulky default values
                    delete config_copy["rendererOptions"];
                    delete config_copy["localeStrings"];
                    localStorage.setItem('pivotdatakey', JSON.stringify(config_copy));
                   // alert(localStorage.pivotdatakey);
                }

                var renderers = $.extend($.pivotUtilities.renderers,
                                                              $.pivotUtilities.subtotal_renderers,
                                                              $.pivotUtilities.gchart_renderers,
                                                               $.pivotUtilities.export_renderers

                                                               );

                var dataClass = $.pivotUtilities.SubtotalPivotData;
                //var renderer = $.pivotUtilities.subtotal_renderers["Table With Subtotal"];

                var config = {
                    onRefresh: saveState,
                    renderers: renderers,
                    dataClass: dataClass,
                    rows: [],
                    cols: [],
                    rendererName: "Table With Subtotal",
                    rendererOptions: {
                        c3: {
                            size: { height: 450, width: 450 }
                        },
                        collapseRowsAt: 0,
                        collapseColsAt:0
                        //arrowCollapsed: "[+] ",
                        //arrowExpanded: "[-] ",
                    }
                };

                        var data = $.parseJSON(result);
      
                        $("#buildoutput").pivotUI(data, config, true);

                        $("#reportview").LoadingOverlay("hide");

                        $("#finalcontrols").show();
                    
   
            }
    catch(Exception){

    }

            }


        });


 



}

function processReport() {

    $.confirm({
        title: 'Confirm Report Publish',
        content: 'Are you sure you want to publish this report as configured? This will overwrite existing setup for this resource',
        buttons: {
            confirm: function () {
                var filters = getFilters();
                var pivotconfig = localStorage.pivotdatakey;

                var formdata = $.param({ dataset: $("#dataset").val(), filters: JSON.stringify(filters), pivotconfig: pivotconfig, resourceid: $("#resource").val()});

                $.ajax({
                    url: localStorage.baseurl + "Report/ProcessReport",
                    type: "POST",
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    data: formdata,
                    success: function (result) {

                        $.alert({
                            title: 'Response Message',
                            content: result,
                            buttons: {
                                Okay: function () {
                                    window.location = window.location;
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


function getFilters(){

    var filters = [];

    var includes = $('.IsIncluded').toArray();

    for (var i = 0; i < includes.length; i++) {

        var icheck = includes[i];

        var filter = JSON.parse(icheck.value);

        filter.IsIncluded =  icheck.checked;

        var valueinput = $(icheck).closest('.filters').find('.ColumnValue');

        filter.ColumnValue = $(valueinput).val();

        var operatorobject = $(icheck).closest('.filters').find('.operator');

        filter.Operator = $(operatorobject).val();

        var displayobject = $(icheck).closest('.filters').find('.DisplayName');

        filter.DisplayName = $(displayobject).val();

        filters[i] = filter;
    }
    return filters;
}

function saveState() {
  
    $.confirm({
        title: 'Confirm State Save',
        content: 'This will save the current state to your User Profile?',
        buttons: {
            Ok: function () {

                var pivotconfig = localStorage.pivotdatakey;

                var params = JSON.parse(paramsvalue);

                var InstanceID = params.InstanceID;

                var reportid = params.ReportID;

                var formdata = $.param({ pivotconfig: pivotconfig, reportid: reportid, InstanceID:InstanceID });

                $.ajax({
                    url: localStorage.baseurl + "Report/SaveUserReportState",
                    type: "POST",
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    data: formdata,
                    success: function (result) {

                        $.alert({
                            title: 'Response Message',
                            content: result,
                            buttons: {
                                Okay: function () {
                                    $("#state").hide();
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


function PrintElem(elem) {
    var mywindow = window.open('', 'popup', 'height=900,width=1200');

    mywindow.document.write('<html><head><title>' + document.title + '</title>');
    mywindow.document.write('</head><body >');
    mywindow.document.write('<h1>' + localStorage.title + '</h1>');
    mywindow.document.write(document.getElementById(elem).innerHTML);
   
    mywindow.document.write('</body></html>');

    mywindow.document.close(); // necessary for IE >= 10
    mywindow.focus(); // necessary for IE >= 10*/

    +mywindow.print();
    mywindow.close();

    return true;
}

function enableExport(object){
    var option = object.value;

    if (option.trim() == "Table") {
        $("#export").show();
       
    }
    else {
        $("#export").hide();
    }
}

function Export() {

}