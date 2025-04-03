$(function () {
    $.when(users.ResetStateSaveDataTable()).then(function () {
        users.List();
    });
});

//let MemberEmail = [];
/*let isValidate = true;*/
let userstable = $('#data-table_userTokenTransaction');

let users = {

    ResetStateSaveDataTable: function () {
        if (common.isReferrer()) {
            userstable.DataTable().state.clear();
        }
    },

    List: function () {
        let columns = [];
        $(userstable).find('thead tr th').each(function (i, v) {

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
                case "name":
                    column = {
                        data: null, name: propName, class: 'fw-semibold', mRender: function (data, type, full) {
                            return `<a href="/UserTokenTransaction/GetListByName?userId=` + data.id + `"  style="text-decoration: underline;">${data.name}</a>`;
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
        tbl.BindTable(userstable, `/UserTokenTransaction/Index`, {}, columns, orderColumns);
    }
}

function openUserTokenTransaction(UserId) {
    $.ajax({
        url: '/UserTokenTransaction/GetListByName',
        data:
        {
            currentId: UserId,
        },
        type: "Get",
        success: function (response) {

        },
        error: function (response) {

        }
    });
}