var dataTable;

var tbl = {
    BindTable: function (table, url, data, columns, orderColumns) {
       
        if ($.fn.dataTable.isDataTable(table)) table.DataTable().destroy();

        dataTable = table.DataTable(           
            {                
                ajax: { url: url, type: "POST", data: data },
                processing: true,
                serverSide: true,
                stateSave: true,
                pageLength: 10,
                lengthMenu: [5, 10, 20, 30, 50, 100],
                responsive: true,
                scrollX: true,
                stateSaveParams: function (settings, data) {
                    data.search.search = "";

                },
                language: {
                    search: '',
                    infoFiltered: "",
                    zeroRecords: "No records found",
                    oPaginate: {
                        sNext: 'Next',
                        sPrevious: 'Prev'
                    }
                },
                order: [],
                columnDefs: [{ targets: orderColumns, orderable: false }],
                columns: columns,
                dom: `<"top">ct<"top"ipl><"clear">`,
                drawCallback: function (settings) {                 
                    let wrapper = $(this).closest('.dataTables_wrapper');
                    wrapper.find('.dataTables_paginate').toggle(this.api().page.info().pages > 1);
                    wrapper.find('.dataTables_length').toggle(settings.fnRecordsTotal() > 0);
                    wrapper.find('.dataTables_info').toggle(settings.fnRecordsTotal() > 0);  
                    
                    // Call SumOfPoints after the table is redrawn for payment report only
                    if (typeof payment != 'undefined' && typeof payment.SumOfPoints === 'function')
                    {
                        payment.SumOfPoints();
                    }

                    // Call SumOfPoints after the table is redrawn for client spending report only
                    if (typeof clientSpendingReport != 'undefined' && typeof clientSpendingReport.SumOfPoints === 'function')
                    {
                        clientSpendingReport.SumOfPoints($('#filterBy').val());
                    }
                }
            }
        );

        $('#seachinput').keyup(function () { 

            if (isSessionActive()) {
                dataTable.search(this.value).draw();
            } else {
                swal({
                    title: 'Session expired!',
                    text: "Please log in again.",
                    icon: 'warning', // Use 'icon' for alert type
                    buttons: {
                        confirm: {
                            text: 'OK',
                            value: true,
                            visible: true,
                            className: 'btn-ok', // Optional: add your own class for styling
                        }
                    }
                }).then(() => {
                    // Optionally, redirect the user to the login page
                    window.location.href = '/Account/Login';  // Redirect to login page
                });
            }
           
            
        });
        $('#seachinput').bind('paste', function () {

            if (isSessionActive()) {
                dataTable.search(this.value).draw();
            } else {

                swal({
                    title: 'Session expired!',
                    text: "Please log in again.",
                    icon: 'warning', // Use 'icon' for alert type
                    buttons: {
                        confirm: {
                            text: 'OK',
                            value: true,
                            visible: true,
                            className: 'btn-ok', // Optional: add your own class for styling
                        }
                    }
                }).then(() => {
                    // Optionally, redirect the user to the login page
                    window.location.href = '/Account/Login';  // Redirect to login page
                });


              //  toastr.error('Session expired! Please log in again.');
                
            }
           
        });

        
        //$('#seachinput').val($('#seachinput').val()).trigger($.Event("keyup", { keyCode: 13 }));


        table.on('click', 'tbody tr', function () {
            table.find('tbody tr').removeClass('table-active');
            $(this).toggleClass('table-active');
        });
    },
    BindTableSimple: function (table, url, data, columns, orderColumns) {

        if ($.fn.dataTable.isDataTable(table)) table.DataTable().destroy();

        var dataTable = table.DataTable(
            {
                ajax: { url: url, type: "POST", data: data },
                processing: true,
                serverSide: true,
              /*  stateSave: true,*/
                pageLength: 10,
                lengthMenu: [5, 10, 20, 30, 50, 100],
                stateSaveParams: function (settings, data) {
                    data.search.search = "";
                },
                language: {
                    search: '',
                    infoFiltered: "",
                    zeroRecords: "No records found",
                    oPaginate: {
                        sNext: 'Next',
                        sPrevious: 'Prev'
                    }
                },
                "ordering": false,
               /* order: [],*/
                /* columnDefs: [{ targets: orderColumns, orderable: false }],*/
                columns: columns,
                dom: `<"top">ct<"top"><"clear">`,
                drawCallback: function (settings) {
                    let wrapper = $(this).closest('.dataTables_wrapper');
                    wrapper.find('.dataTables_paginate').hide();
                    wrapper.find('.dataTables_length').hide();
                    wrapper.find('.dataTables_info').hide();
                }
            }
        );



        table.on('click', 'tbody tr', function () {
            table.find('tbody tr').removeClass('table-active');
            $(this).toggleClass('table-active');
        });
    }

}


function isSessionActive() {
    return $.ajax({
        url: '/Home/IsSessionActive',  // Replace with your controller action
        method: 'GET',
        async: false  // Synchronous check (returns true or false)
    }).responseText === "true";
}
