function setMembershipFilter() {
    var tempobj = { "FilterOperator": 0, "Conditions": [{ "AttributeName": "lk_tms_systemuserid.teamid", "Operator": 0, "Values": [Dlv.Page.PageContext.RecordId] }], "Filters": [] }
    Dlv.Page.getControl('users').setfilter(tempobj).refresh();
}
//添加团队成员
function AddTeamMembership() {
    Dlv.web.OpenDialog('/entity/RecordsDialog?entityname=systemuser&singlemode=false&inputid=team', 'AddTeamMembership_Callback');
}
function AddTeamMembership_Callback(result, inputid) {
    console.log(result, inputid);
    var ids = [];
    $(result).each(function (i, n) {
        ids.push(n.id);
    });
    var obj = { teamid: Dlv.Page.PageContext.RecordId, userid: ids };
    Dlv.web.Post('/api/team/addmembers', obj, false, function (response) {
        Dlv.web.Toptip(response.content);
        if (response.IsSuccess) location.reload();
    });
}
//移除团队成员
function RemoveTeamMembership() {
    $('.subgrid[id=users] .datatable')
    Dlv.web.Post('/api/team/removemembers', obj, false, function (response) {
        Dlv.web.Toptip(response.content);
        if (response.IsSuccess) location.reload();
    });
}

//分派安全角色
function AssignRolesToTeam() {
    var id = [];
    if (Dlv.Page && Dlv.Page.PageContext && Dlv.Page.PageContext.RecordId) {
        id.push(Dlv.Page.PageContext.RecordId);
    }
    else {
        var target = $('#datatable');
        id = Dlv.web.GetTableSelected(target);
    }
    if (!id || id.length == 0) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, 'error');
        return;
    }
    Dlv.web.OpenDialog('/security/RolesDialog?singlemode=false', 'AssignRolesToTeamCallback', { teamid: id })
}
function AssignRolesToTeamCallback(data, model) {
    console.log(data, model);
    var ids = [];
    $(data).each(function (i, n) {
        ids.push(n.id);
    });
    Dlv.web.Post('/security/AssignRolesToTeam', { teamid: model.teamid, roleid: ids }, false, function (response) {
        Dlv.web.Toast(response.IsSuccess, response.Content);
    });
}