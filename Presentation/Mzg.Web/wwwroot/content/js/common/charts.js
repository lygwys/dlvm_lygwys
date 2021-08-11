//闭包执行一个立即定义的匿名函数
!function (factory) {
    //factory是一个函数，下面的koExports就是他的参数

    // Support three module loading scenarios
    if (typeof require === 'function' && typeof exports === 'object' && typeof module === 'object') {
        // [1] CommonJS/Node.js
        // [1] 支持在module.exports.abc,或者直接exports.abc
        var target = module['exports'] || exports; // module.exports is for Node.js
        factory(target);
    } else if (typeof define === 'function' && define['amd']) {
        // [2] AMD anonymous module
        // [2] AMD 规范
        //define(['exports'],function(exports){
        //    exports.abc = function(){}
        //});
        define(['jquery'], factory);
    } else {
        // [3] No module loader (plain <script> tag) - put directly in global namespace
        factory(window['jQuery']);
    }
}(function ($) {
/****************************************************************
 * 以上为固定写法
* 插件兼容CommonJS, AMD, CMD 和 原生 JS
* 我们除了提供 AMD 模块接口，CMD 模块接口，还得提供原生的 JS 接口。
* 由于 CMD 和 AMD 都可以使用 return 来定义对外接口，故可以合并成一句代码。
* mzg
* 20200427
* **************************************************************
*/
    var modules_common_charts = {
        insertChart: insertChart,
        GetChartList: GetChartList
    }
    function GetChartList(callback) {
        Dlv.web.GetJson('/api/chart?entityid=' + Dlv.Page.PageContext.EntityId, null, function (data) {
            console.log('data.content', data);
            if (!data || !data.content || data.content.length == 0) return;
            var chartHtml = [];
            chartHtml.push('<option value="" ></option>')
            $(data.content).each(function (i, n) {
                chartHtml.push('<option value="' + n.chartid + '" >' + n.name + '</option>');
            });
            $('#ChartList').html(chartHtml);
            callback && callback();
        });
    }
    function insertChart(chartid, queryid, opts, postData, callback) {
        if (!opts) {
            opts = { 'width': '100%', 'height': '300px' };
        }
        if ($("#listchartWinBtn").attr("active") == "1") {
            opts = { 'width': '100%', 'height': '300px' };
        }
        //mzg.load('/component/RenderChart?queryid=04377908-2bc2-4e6d-be48-82ea5bbc62d7&chartid=8eeaef79-3cf5-4d97-82b1-2c7f1dc8041e', function (data) {
        if (!postData) {
            postData = {};
        }
        postData.QueryId = queryid;
        postData.ChartId = chartid;
        postData.Filter = filters
        console.log(postData);
        $.ajax({
            type: "POST",
            data: JSON.stringify(postData),
            url: ORG_SERVERURL + '/component/RenderChart',
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (data) {
                try { data = data.replace(/chart1/g, 'chart' + j); }
                catch (e) { }
                var dHtml = $('<div style="width:100%"></div>');
                dHtml.html(data);
                dHtml.find('.chartA:last').css(opts);
                dHtml.find('.chartA:last').children().css(opts);
                dHtml.find('.chartA').trigger('getInserComponent', function (obj) {
                    renderChart && renderChart(obj.chartid, obj.queryid, null, { filter: obj.filter, groups: obj.groups });
                });
                callback && callback(dHtml, data, opts, postData);
            }
        });
    }

    window.common_charts = modules_common_charts;
    return modules_common_charts
});