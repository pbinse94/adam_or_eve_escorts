var tbl = {
    BindTable: function (table, url, data, columns, orderColumns) {
       
        if ($.fn.dataTable.isDataTable(table)) table.DataTable().destroy();

        var dataTable = table.DataTable(           
            {                
                ajax: { url: url, type: "POST", data: data },
                processing: true,
                serverSide: true,
                stateSave: true,
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
                order: [],
                columnDefs: [{ targets: orderColumns, orderable: false }],
                columns: columns,
                dom: `<"top">ct<"top"ipl><"clear">`,
                drawCallback: function (settings) {                 
                    let wrapper = $(this).closest('.dataTables_wrapper');
                    wrapper.find('.dataTables_paginate').toggle(this.api().page.info().pages > 1);
                    wrapper.find('.dataTables_length').toggle(settings.fnRecordsTotal() > 0);
                    wrapper.find('.dataTables_info').toggle(settings.fnRecordsTotal() > 0);
                }
            }
        );

        $('#seachinput').keyup(function () {           
            dataTable.search(this.value).draw();
        });
        $('#seachinput').bind('paste',function () {
            dataTable.search(this.value).draw();
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