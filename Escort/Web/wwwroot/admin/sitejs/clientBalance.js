$(function () {
    $.when(clientbalance.ResetStateSaveDataTable()).then(function () {
        clientbalance.List();
    });
});

//let MemberEmail = [];
/*let isValidate = true;*/
let clientbalancetable = $('#data-table_userTransaction');

let clientbalance = {

    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            userstable.DataTable().state.clear();
        }
    },

    List: function () {
        let columns = [];
        $(clientbalancetable).find('thead tr th').each(function (i, v) {

            let propName = $(v).data('name');
            let column;

            switch (propName.toLowerCase()) {
                case "id":
                    column = {
                        data: null, name: propName, render: function (data, type, row, meta) {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    };
                    break; 
                case "date":
                    column = {
                        data: null, // Assuming 'AddedOnUTC' is the key in your data source that holds the date string
                        name: propName,
                        width: '15%',
                        maxWidth: '15%',
                        render: function (data, type, row, meta) {
                            if (type === 'display' || type === 'filter') {
                                const date = new Date(data.date);
                                const day = ("0" + date.getDate()).slice(-2);
                                const month = ("0" + (date.getMonth() + 1)).slice(-2);
                                const year = date.getFullYear();
                                return day + '/' + month + '/' + year;
                            }
                            return data; // Return the original data for other types
                        }
                    };
                    break;
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), class: 'dt-right rpad-26', name: propName, width: '15%' };
                    break;
            }

            columns.push(column);
        });

        let orderColumns = [0];

      
        data = {
            ClientId: clientId, 
        };
        tbl.BindTable(clientbalancetable, `/User/ClientBalanceList`, data, columns, orderColumns);
    }
}