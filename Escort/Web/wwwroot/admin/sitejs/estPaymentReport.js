$(function ()
{
    $.when(payment.ResetStateSaveDataTable()).then(function ()
    {
        payment.List();
    });
});

let paymenttable = $('#data-table_estPaymentReport');


let payment = {
    ResetStateSaveDataTable: function ()
    {
        if (common.isReferrer())
        {
            paymenttable.DataTable().state.clear();
        }
    },
    List: function ()
    {
        let columns = [];
        $(paymenttable).find('thead tr th').each(function (i, v)
        {

            let propName = $(v).data('name');
            let column;

            switch (propName.toLowerCase())
            {
                case "id":
                    column = {
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return meta.row + meta.settings._iDisplayStart + 1;
                        }
                    };
                    break;
                case "amount":
                    column = {
                        width: "23px",
                        maxWidth: "23px",
                        data: null, name: propName, render: function (data, type, row, meta)
                        {
                            return "$" + data.amount.toFixed(2).toLocaleString('en');
                        }
                    };
                    break;
                
                default:
                    column = { data: propName.charAt(0).toLowerCase() + propName.slice(1), name: propName, width: '15%' };
                    break;
            }
            columns.push(column);
        });

        let orderColumns = [0];
        

        let request = {
            EscortId: 15,
            IsPaid: 0
        };

        tbl.BindTable(paymenttable, `/UserTokenTransaction/EscortPaymentReport`, request, columns, orderColumns);
                

    },
}