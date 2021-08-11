function DeleteBusinessUnit() {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    var target = $('#datatable');
    Dlv.web.SelectingRow(target, false, true);
    var id = Dlv.web.GetTableSelected(target);
    Dlv.web.Del(id, '/org/deletebusinessunit', false, getFunction(target.attr('data-refresh')));
}