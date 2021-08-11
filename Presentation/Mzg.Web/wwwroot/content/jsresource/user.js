﻿//@ sourceURL=user.js
//分派安全角色
function assignRolesToUser() {
    var id = [];
    if (Dlv.Page && Dlv.Page.PageContext && Dlv.Page.PageContext.RecordId) {
        id.push(Dlv.Page.PageContext.RecordId);
    }
    else {
        var target = $('#datatable');
        id = Dlv.web.GetTableSelected(target);
    }
    if (!id || id.length == 0) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    Dlv.web.OpenDialog('/security/RolesDialog?singlemode=false', 'assignRolesToUserCallback', { userid: id })
}
function assignRolesToUserCallback(data, model) {
    console.log(data, model);
    var ids = [];
    $(data).each(function (i, n) {
        ids.push(n.id);
    });
    Dlv.web.Post('/security/AssignRolesToUser', { userid: model.userid, roleid: ids }, false, function (response) {
        if (response.IsSuccess) {
            Dlv.web.Toast(response.Content, response.IsSuccess, 2500);
        }
        else {
            Dlv.web.Alert(false, response.Content);
        }
    }, null, false, false);
}
//修改密码
function setUserPassword() {
    debugger;
    var id = Dlv.Page.PageContext.RecordId;
    Dlv.web.OpenWindow(ORG_SERVERURL + '/user/EditUserPassword?id=' + id)
}
//重新分派记录
function assignUserAllRecords() {
    var id = Dlv.Page.PageContext.RecordId;
    Dlv.web.OpenDialog('/entity/AssignUserAllRecords', null, { userid: id });
}
//设置用户状态
function SetUserState(state) {
    var id = [], cb;
    if (Dlv.Page && Dlv.Page.PageContext && Dlv.Page.PageContext.RecordId) {
        id.push(Dlv.Page.PageContext.RecordId);
    }
    else {
        var target = $('#datatable');
        id = Dlv.web.GetTableSelected(target);
        cb = getFunction(target.attr('data-refresh'));
    }
    if (!id || id.length == 0) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    Dlv.web.Post('/user/SetUserActive?isactive=' + (state), { recordid: id }, false, function (response) {
        if (response.IsSuccess) {
            Dlv.web.Toast(response.Content, true);
            if (cb) cb.call(this, response);
            else location.reload(true);
            return;
        }
        Dlv.web.Alert(false, response.Content);
    }, null, null, false);
}
//更换经理
function ChangeParentUser() {
    var target = $('<div>'
        + '<div class="form-group container-fluid"><label class="col-sm-2">' + LOC_USER + '</label><div class="input-group input-group-sm">'
        + '<input type="text" id="dialog_systemuserid_text" data-type="lookup" name="dialog_systemuserid_text" class="form-control colinput lookup input-sm" />'
        + '<input type="hidden" id="dialog_systemuserid" data-type="lookup" name="dialog_systemuserid" class="form-control colinput" />'
        + '</div></div>'
        + '</div>');
    target.dialog({
        title: '<span class="glyphicon glyphicon-info-sign"></span> ' + LOC_USER_CHANGEPARENT
        , onClose: function () {
            $(this).dialog("destroy");
            $(document.body).css("padding-right", 0);
        }
        , buttons: [
            {
                text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                classed: "btn-default",
                click: function () {
                    $(this).dialog("destroy");
                    $(document.body).css("padding-right", 0);
                }
            }
            , {
                text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                classed: "btn-info",
                click: function () {
                    $(this).dialog("destroy");
                    $(document.body).css("padding-right", 0);
                }
            }
        ]
    });
    var filter = null;//{ "Conditions": [{ "AttributeName": "customerid", "Operator": 0, "Values": [customerid] }] }
    target.find('#dialog_systemuserid_text').lookup({
        disabled: true,
        dialog: function () {
            Dlv.web.OpenDialog('/entity/RecordsDialog?singlemode=true&entityname=systemuser&inputid=dialog_systemuserid', 'ChangeParentUserCallback', { filter: filter });
        }
        , clear: function () {
            $('#dialog_systemuserid').val('');
            $('#dialog_systemuserid_text').val('');
        }
    });
}
function ChangeParentUserCallback(result, inputid) {
    if (!result || result.length == 0) return;
    $('#' + inputid).val(result[0].id);
    $('#' + inputid + '_text').val(result[0].name);
}
//设置默认仪表板
function setDefaultDashboard(userid, id) {
    console.log(userid, id);
    if (!userid || !id) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    var obj = { entityname: 'systemusersettings', data: {} };
    obj.data.id = userid;
    obj.data.defaultdashboardid = id;
    obj.data = JSON.stringify(obj.data);
    Dlv.web.Post('/api/data/update', obj, false, function (response) {
        if (response.StatusName != 'success')
            Dlv.web.Toptip(response.content);
    });
}