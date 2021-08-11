var Customer = {
    Locked: function () {
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
        var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
        obj.data.id = Dlv.Page.PageContext.RecordId;
        obj.data.statuscode = 3;
        obj.data = JSON.stringify(obj.data);
        Dlv.web.Post('/api/data/update', obj, false, function (response) {
            Dlv.web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , UnLocked: function () {
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
        var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
        obj.data.id = Dlv.Page.PageContext.RecordId;
        obj.data.statuscode = 1;
        obj.data = JSON.stringify(obj.data);
        Dlv.web.Post('/api/data/update', obj, false, function (response) {
            Dlv.web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
};
var Lead = {
    TransToCustomer: function () {
        var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
        obj.data.id = Dlv.Page.PageContext.RecordId;
        obj.data.statuscode = 2;
        obj.data = JSON.stringify(obj.data);
        Dlv.web.Post('/api/data/update', obj, false, function (response) {
            //mzg.Web.Toptip(response.content);
            if (response.IsSuccess) {
                //生成客户及联系人信息
                Dlv.web.Post('/api/data/create/map', { sourceentityname: 'lead', targetentityname: 'customer', sourcerecordid: Dlv.Page.PageContext.RecordId }, false, function (response) {
                    Dlv.web.Toptip(response.content);
                    if (response.IsSuccess) {
                        location.href = ORG_SERVERURL + '/entity/create?entityid=' + response.Extra.entityid + '&recordid=' + response.Extra.id;
                    }
                });
            }
            else {
                Dlv.web.Toptip(response.content);
            }
        });
    }
    , Cancel: function () {
        var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
        obj.data.id = Dlv.Page.PageContext.RecordId;
        obj.data.statuscode = 3;
        obj.data = JSON.stringify(obj.data);
        Dlv.web.Post('/api/data/update', obj, false, function (response) {
            Dlv.web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , ReActive: function () {
        var obj = { entityid: Dlv.Page.PageContext.EntityId, data: {} };
        obj.data.id = Dlv.Page.PageContext.RecordId;
        obj.data.statuscode = 1;
        obj.data = JSON.stringify(obj.data);
        Dlv.web.Post('/api/data/update', obj, false, function (response) {
            Dlv.web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
};