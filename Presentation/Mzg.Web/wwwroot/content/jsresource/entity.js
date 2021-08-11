//@ sourceURL=entity.js
//记录操作相关1
function CreateRecord(newWindow, parameters) {
    var url = location.href;
    if (url.indexOf('entity/create') > 0) {
        url = $.setUrlParam(url, 'entityid', Dlv.Page.PageContext.EntityId);
        url = $.setUrlParam(url, 'formid', Dlv.Page.PageContext.TargetFormId);
        url = $.setUrlParam(url, 'recordid', null);
        newWindow = false;
    } else {
        url = ORG_SERVERURL + '/entity/create?entityid=' + Dlv.Page.PageContext.EntityId + (Dlv.Page.PageContext.TargetFormId ? '&formid=' + Dlv.Page.PageContext.TargetFormId : '');
    }
    if (parameters) url += (url.indexOf('?') > 0 ? '&' + parameters : '?' + parameters);
    if (newWindow) {
        //mzg.Web.OpenWindow(url);
        entityIframe('show', url);
    }
    else {
        location.href = url;
    }
}
function EditRecord(newWindow) {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    var target = $('#datatable');
    Dlv.web.SelectingRow(event.target, false, true);
    var id = Dlv.web.GetTableSelected(target);
    var url = '/entity/edit?entityid=' + Dlv.Page.PageContext.EntityId + '&recordid=' + id;
    if (newWindow) {
        Dlv.web.OpenWindow(ORG_SERVERURL + url);
    }
    else {
        location.href = ORG_SERVERURL + url;
    }
}
function CopyRecord() {
    var url = ORG_SERVERURL + '/entity/create?entityid=' + Dlv.Page.PageContext.EntityId + '&copyid=' + Dlv.Page.PageContext.RecordId;
    location.href = url;
}
function DeleteRecord() {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    var target = $('#datatable');
    Dlv.web.SelectingRow(event.target, false, true);
    var id = Dlv.web.GetTableSelected(target);
    Dlv.web.Del(id, '/api/data/delete?entityid=' + Dlv.Page.PageContext.EntityId, false, function (response) {
        if (response.IsSuccess) {
            getFunction(target.attr('data-refresh'))();
            return;
        }
    });
}
function DeleteOneRecord() {
    var id = Dlv.Page.PageContext.RecordId;
    if (!id) {
        Dlv.web.Toast(LOC_SAVERECORD_FIRST, false);
        return;
    }
    Dlv.web.Del([id], '/api/data/delete?entityid=' + Dlv.Page.PageContext.EntityId, false, function (response) {
        console.log(response);
        if (response.IsSuccess) {
            Dlv.web.Event.publish('refresh');
            Dlv.web.CloseWindow();
        }
    });
}
function Save(callback) {

    var event = event || window.event || arguments.callee.caller.arguments[0];
    callback && callback();
    if (typeof formSaveSubGrid == 'function') {
        if (formSaveSubGrid()) {
            if (event && event.target) {
                $(event.target).parents('form:first').trigger('submit');
            } else {
                $('form:first').trigger('submit');
            }
        }
    } else {
        $(event.target).parents('form:first').trigger('submit');
    }
}
function SaveAndNew() {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    if (typeof formSaveSubGrid == 'function') {
        if (formSaveSubGrid()) {
            _formSaveAction = Dlv.FormSaveAction.saveAndNew;
            if (event && event.target) {
                $(event.target).parents('form:first').trigger('submit');
            } else {
                $('form:first').trigger('submit');
            }
        }
    } else {
        _formSaveAction = Dlv.FormSaveAction.saveAndNew;
        $(event.target).parents('form:first').trigger('submit');
    }
}
//状态更改
function SetRecordState(state) {
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
    var data = {};
    data.entityid = Dlv.Page.PageContext.EntityId;
    data.recordid = id;
    data.state = state;
    Dlv.web.OpenDialog('/entity/setrecordstate', null, data);
}
function SetRecordStatus(status) {
    var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
    obj.data.id = Dlv.Page.PageContext.RecordId;
    obj.name = Dlv.Page.PageContext.EntityName;
    obj.data.statuscode = status;
    obj.data = JSON.stringify(obj.data);
    Dlv.web.Post('/api/data/update', obj, false, function (response) {
        Dlv.web.Toptip(response.content);
        if (response.IsSuccess) {
            Dlv.web.Event.publish('refresh');
            location.reload(true);
        }
    });
}
//共享
function Sharing(entityid, objectid) {
    if (!objectid) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    if (!entityid) {
        Dlv.web.Alert(false, LOC_NOTSPECIFIED_OBJECTTYPE);
        return;
    }
    var data = { entityid: entityid, objectid: objectid };
    Dlv.web.OpenDialog('/entity/sharing', null, data);
}
function Shared(objectid, target) {
    if (!objectid) {
        Dlv.web.Alert(false, LOC_NOTSPECIFIED_RECORD);
        return;
    }
    if (!target || target.length == 0) {
        Dlv.web.Alert(false, LOC_NOTSPECIFIED_OBJECT);
        return;
    }
    var data = { objectid: objectid, target: target };
    Dlv.web.Post('/api/data/share', data, false, function (response) {
        if (response.IsSuccess) {
            Dlv.web.Toptip(response.Content);
            return;
        }
        Dlv.web.Alert(false, response.Content);
    });
}
//分派
function Assigning(entityid, objectid) {
    if (!objectid) {
        objectid = Dlv.web.GetTableSelected($('#datatable'));
    }
    if (!objectid || objectid.length == 0) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    if (!entityid) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_OBJECTTYPE, false);
        return;
    }
    if (Object.prototype.toString.call(objectid) != '[object Array]') {
        objectid = [objectid];
    }
    var data = { entityid: entityid, objectid: objectid };
    Dlv.web.OpenDialog('/entity/assigning', null, data);
}
//合并
function MergeRecords() {
    var target = $('#datatable');
    var id = Dlv.web.GetTableSelected(target);
    if (!id || id.length == 0) {
        Dlv.web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    if (id.length != 2) {
        Dlv.web.Toast(LOC_SPECIFIED_TWORECORDS, false);
        return;
    }
    Dlv.web.OpenDialog('/entity/merge?entityid=' + Dlv.Page.PageContext.EntityId + '&recordid1=' + id[0] + '&recordid2=' + id[1]);
}
//下推单据
function AppendRecord(entityid, recordid) {
    if (!recordid) {
        Dlv.web.Alert(false, LOC_NOTSPECIFIED_RECORD);
        return;
    }
    if (!entityid) {
        Dlv.web.Alert(false, LOC_NOTSPECIFIED_OBJECTTYPE);
        return;
    }
    Dlv.web.OpenDialog('/entity/appendrecord?entityid=' + entityid + '&recordid=' + recordid);
}
//列表相关
function GetRowData($row) {
    var target = $row.parents('table:first');
    var data = new Array();
    target.find('thead>tr>th[data-name]').each(function (i, n) {
        var self = $(n);
        var dataCell = $row.find('td:eq(' + self.index() + ')');
        data[i] = dataCell.text().trim();
    });
    return data;
}
//工作流相关
// 启动审批
// mzg
// 202006171111
function StartWorkFlow(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Dlv.web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Dlv.web.OpenDialog('/flow/startworkflow?entityid=' + entityid + '&recordid=' + recordid, callback);
}
// 审批处理
// mzg
// 202006171422
function WorkFlowProcessing(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Dlv.web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Dlv.web.OpenDialog('/flow/workflowprocessing?entityid=' + entityid + '&recordid=' + recordid, null, null, callback);
}
// 审批详情
function WorkFlowProcessDetail(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Dlv.web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Dlv.web.OpenDialog('/flow/WorkFlowInstanceDetail?entityid=' + entityid + '&recordid=' + recordid, callback);
}
// 撤消审批
function WorkFlowCancel(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Dlv.web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    var func = true;
    if (typeof WorkFlowCancelBeforSubmit === "function") func = WorkFlowCancelBeforSubmit();
    if (func) {//如果返回true继续
        Dlv.web.Confirm(LOC_CONFIRM_OPERATION_TITLE, '确定要撤消吗？',
            function () {
                Dlv.web.Post('/api/workflow/cancel?entityid=' + entityid + '&recordid=' + recordid, null, false, function (response) {
                    console.log('callback', callback);
                    if (response.IsSuccess) {
                        Dlv.web.Toptip(response.Content, true);
                        Dlv.web.Event.publish('refresh');
                        if (typeof (callback) == "function") callback(response);
                        else location.reload(true);
                        return;
                    }
                    Dlv.web.Alert(false, response.Content);
                })
            }
        );
    }
}
function changeNoticeReaded() {
    var target = $('#datatable');
    var id = Dlv.web.GetTableSelected(target);
    $(id).each(function (i) {
        var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
        obj.data.id = this;
        obj.data.isread = 1;
        obj.data = JSON.stringify(obj.data);
        Dlv.web.Post('/api/data/update', obj, false, function (response) {
            if (i == id.length - 1) {
                Dlv.web.Event.publish("noticeChange");
                rebind();
            }
        }, false, false, true);
    });
}