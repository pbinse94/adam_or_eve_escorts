$(function ()
{
    giftTokensChart();
    getCountryWiseEscorts();
    if (isRevenueReportAvailable == "True")
    {
        getRevenueReportChart();
    }
    getSubscriptionReportChart();

    Highcharts.setOptions({
        lang: {
            thousandsSep: ','
        }
    });
})

function getCountryWiseEscorts()
{
    $.ajax({

        type: "GET",
        url: '/Dashboard/GetCountryWiseEscorts',
        dataType: 'html',
        success: function (responseHtml)
        {
            $('#country-wise-escorts').html(responseHtml);
        },
        error: function (data)
        {
        }
    });
}

function giftTokensChart()
{

    $.ajax({

        type: "GET",
        url: '/Dashboard/GetGiftsChart',
        async: true,
        success: function (data)
        {
            const months = [];
            const tokens = [];
            $.each(data, function (i, v)
            {

                months.push(
                    v.month,
                );
                tokens.push(
                    v.tokens,
                );
            })


            Highcharts.chart('gift-chart', {
                title: false,

                subtitle: {
                    //text: 'Gift Summary'
                },
                credits: false,
                chart: {
                    type: 'areaspline'
                },
                xAxis: {
                    categories: months,
                    title: {
                        text: 'Months',

                    }
                },

                yAxis: {
                    title: {
                        text: 'Gift Spent',

                    }
                },
                plotOptions: {
                    series: {
                        pointStart: 0
                    },
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    },
                    allowPointSelect: false,

                },
                series: [{
                    name: 'Gift Spent',
                    data: tokens,
                    color: '#ffb96e',
                    lineColor: '#EC881D',
                    fillColor: '#ffb96e',
                    marker: {
                        fillColor: '#EC881D',
                    },
                    showInLegend: false

                }]
            });

        },
        error: function (data)
        {
        }

    });
}

function getRevenueReportChart()
{
    $.ajax({

        type: "GET",
        url: '/Dashboard/GetRevenueChart',
        async: true,
        success: function (data)
        {
            const months = [];
            const amounts = [];
            $.each(data, function (i, v)
            {

                months.push(
                    v.month,
                );
                amounts.push(
                    v.amount,
                );
            })


            Highcharts.chart('weekly-revenue-report', {
                chart: {
                    type: 'column'
                },
                title: false,

                subtitle: {
                    //text: 'Gift Summary'
                },
                credits: false,
                xAxis: {
                    categories: months,
                    accessibility: {
                        description: 'Months'
                    },
                    title: {
                        text: 'Months',

                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Revenue in Dollers($)'
                    }
                },
                tooltip: {
                    valuePrefix: '$'
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },
                series: [
                    {
                        name: 'Doller',
                        data: amounts,
                        color: '#EC881D',
                        showInLegend: false
                    }
                ]
            });

        },
        error: function (data)
        {
        }

    });


}

function getSubscriptionReportChart()
{
    $.ajax({

        type: "GET",
        url: '/Dashboard/GetSubscriptionChart',
        async: true,
        success: function (data)
        {
            const months = [];
            const amounts = [];
            $.each(data, function (i, v)
            {

                months.push(
                    v.month,
                );
                amounts.push(
                    v.amount,
                );
            })


            Highcharts.chart('subscription-report', {
                chart: {
                    type: 'areaspline'
                },
                title: false,

                subtitle: {
                    //text: 'Gift Summary'
                },
                credits: false,
                xAxis: {
                    categories: months,
                    accessibility: {
                        description: 'Months'
                    },
                    title: {
                        text: 'Months',

                    }
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Revenue in Dollers($)'
                    }
                },
                tooltip: {
                    valuePrefix: '$'
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },
                series: [
                    {
                        name: 'Doller',
                        data: amounts,
                        color: '#ffb96e',
                        lineColor: '#EC881D',
                        fillColor: '#ffb96e',
                        marker: {
                            fillColor: '#EC881D',
                        },
                        showInLegend: false
                    }
                ]
            });

        },
        error: function (data)
        {
        }

    });

}