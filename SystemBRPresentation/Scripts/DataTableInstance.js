var DataTableEx;
var lineCount = 25;

$(function () {
    $.ajax({
        url: '../Configuracao/GetConfiguracao'
        , type: 'POST'
        , async: false
        , success: function (r) {
            if (r.CONF_IN_LINHAS_GRID != null) {
                lineCount = r.CONF_IN_LINHAS_GRID;
            }
        }
    });
});

$(document).ready(function () {
    DataTableEx = $('.dataTables-example').DataTable({
        pageLength: lineCount,
        dom: '<"html5buttons"B>lTfgitp',
        buttons: [
            { extend: 'copy' },
            { extend: 'csv' },
            { extend: 'excel', title: 'ExampleFile' },
            { extend: 'pdf', title: 'ExampleFile' },
            {
                extend: 'print',
                customize: function (win) {
                    $(win.document.body).addClass('white-bg');
                    $(win.document.body).css('font-size', '10px');

                    $(win.document.body).find('table')
                        .addClass('compact')
                        .css('font-size', 'inherit');
                }
            }
        ]
    });
});