//@ sourceURL=pages/entity.create.js
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
    //page Url = /pages/entity.create.js
    //deps
    var asyncindex = -1;
    var singnal = true;
    var isSyncLoadSubGrid = false;//单据体是否使用异步加载方式

    var setlabelsToTarget = createlabelsToTargetFactory();
    var syncCount = 0;//页面需要统计的异步方法个数
    var SetFormStateStep = 0;

    function getPageType() {
        if (location.href.indexOf('recordid') > -1) {
            return "edit";
        }
        return "new";
    }
    //page init
    var pageWrap_Create = {
        loadFormInfo: function () {
            //mzg.Web.Post()
        },
        init: function () {
            asyncindex = Dlv.Form.getTabIsAsync(_form.Panels);
            if (~asyncindex) {
                isSyncLoadSubGrid = true;
            }
            $(function () {
                
                if ($('.breadcrumb > li:not(.pull-right)').length <= 1) {
                    $('.breadcrumb').append('<li>' + page_Common_Info.localizeName + '</li>');
                }
                $('.breadcrumb > li.pull-right').remove();
                if (top.location != location) {
                    $('.breadcrumb').parent().hide();
                }
                $('#main').css('margin-bottom', 0)
                $('title').prepend(page_Common_Info.title + ' - ');
                $(".modal").draggable({
                    handle: ".modal-header",
                    cursor: 'move',
                    refreshPositions: false
                });
                var $formWrap = $('<div class="xms-formWrap container-fluid"></div>').html('<div class="row"></div>');

                //生成导航链接
                var isRenderNav = _form.IsShowNav;

                var $formContent = $('<div class="xmsformContent col-sm-12"></div>');
                var $formIframe = $('<iframe id="formIframeContent" src="" name="formIframeContent" class="ifrmae-list" width="100%" frameborder="0" onload="Javascript:Dlv.web.iFrameHeight(this,-150)"  scrolling="no" style=" width=100%; visibility: visible; display:none; position:abslute; top:0; bottom:0; left:0; right;0;" ></iframe>');

                //console.log(_form)
                //生成表单
                Dlv.Form.CreateFormDiv(_form, _record, $('#editForm'));

                if (_form.CustomCss) {
                    var formcss = JSON.parse(_form.CustomCss);
                    var labels = formcss.labels;
                    var inputs = formcss.inputs;
                    $.addClassToElement(null, null, $.changeobjtoStyle(labels, 'form-cell-label'));
                    $.addClassToElement(null, null, $.changeobjtoStyle({ 'font-size': labels['font-size'] }, 'form-cell-label>label'));
                    $.addClassToElement(null, null, $.changeobjtoStyle(inputs, '.form-cell-ctrl input,.form-cell-ctrl select', ' '));
                }

                //mzg.Page.getControl('users').seteditable(false);
                //mzg.Page.getControl('users').setfilter({"FilterOperator":0,"Conditions":[{"AttributeName":"lk_tms_systemuserid.teamid","Operator":0,"Values":[Dlv.Page.PageContext.RecordId]}],"Filters":[]});
                $formContent.append($(".body")).append($formIframe).css('position', 'relative');
                if (isRenderNav) {
                    //显示导航链接
                    var $formNav = renderFormNav(_form, $('#editForm'));
                    if ($formNav && $formNav.length) {
                        $formWrap.children().append($formNav);
                        $formContent.removeClass('col-sm-12').addClass('col-sm-10')
                        //绑定导航链接事件
                        $formNav.find("a[data-href]").on("click", function (e) {
                            e = e || window.event;
                            var $this = $(this);
                            var type = $this.attr("data-type");
                            //  if(type==1){
                            changeFormIframe($this);
                            //  }else{
                            //  }
                            if (e.preventDefault) {
                                e.preventDefault();
                            } else if (e.returnValue) {
                                e.returnValue = false;
                            }
                        });
                    }
                }
                $formWrap.children().append($formContent);
                $('#editForm').append($formWrap);
                $formWrap.append($("#xmsFormFooter"));

                //加载业务流程
                if (Dlv.Page.PageContext.RecordId != null && Dlv.Page.PageContext.BusinessFlowEnabled == true) {
                    Dlv.web.LoadPage('/flow/businessprocess'
                        , { entityid: Dlv.Page.PageContext.EntityId, recordid: Dlv.Page.PageContext.RecordId, businessflowinstanceid: Dlv.Page.PageContext.BusinessFlowInstanceId }, function (data) {
                            if (data.indexOf('{') == 0) {
                                var d = JSON.parse(data);
                                console.log(d);
                                if (d.IsSuccess == false) {
                                    Dlv.web.Toast('业务流程：' + d.Content, false)
                                    return;
                                }
                            }
                            var $buss = $('<div id="businessprocess-section" class="businessprocess-section" />');
                            $buss.html(data);
                            $('.header').after($buss);
                            //Dlv.Page.SetFormState(Dlv.Page.Form.State);
                        });
                }
                //如果是系统自动编号则设置为不可编辑
                if (_serialNumberField) {
                    var attr = Dlv.Page.getAttribute(_serialNumberField);
                    if (attr) attr.setDisabled(true);
                }
                if (_nonePermissions) {
                    if (_nonePermissions.noneread && _nonePermissions.noneread.length > 0) {
                        for (var i = 0; i < _nonePermissions.noneread.length; i++) {
                            var field = $('[data-attributeid=' + _nonePermissions.noneread[i] + ']');
                            field.val('').prop('disabled', true).prop('title', '没有查看权限').prop('type', 'password');
                            //field.before('<span class="glyphicon glyphicon-lock"></span>');
                        }
                    }
                    if (_nonePermissions.noneedit && _nonePermissions.noneedit.length > 0) {
                        for (var i = 0; i < _nonePermissions.noneedit.length; i++) {
                            $('[data-attributeid=' + _nonePermissions.noneedit[i] + ']').prop('disabled', true).prop('title', '没有更新权限');
                        }
                    }
                }

                

                $('.subtools').click(function () {
                    var type = $(this).attr('data-type');
                    if (type == 'attach') {
                        Dlv.web.OpenDialog('/file/AttachmentsDialog?entityid=' + Dlv.Page.PageContext.EntityId + '&objectid=' + Dlv.Page.PageContext.RecordId);
                    }
                });

				//lyb 获得焦点 取消千位符
				$('input[data-controltype="money"]').focus(function () {
								var $a = $(this).val().replace(/,|\s/g, '');
								$(this).val($a);
							});
				
				//lyb 失去焦点 加上千位符
				$('input[data-controltype="money"]').blur(function () {
					var $b = ConvertMtoT($(this).val());
					$(this).val($b);
				});

                //引用类型扩展属性显示标签
                $(".lookupLabel").each(function (key, obj) {
                    var self = $(obj);
                    var valueid = self.prop('id');
                    var name = self.attr('name').toLowerCase();
                    if (_record && typeof _record[name + 'name'] !== 'undefined') {
                        self.text('');
                        self.text(_record[name + 'name']);
                    } else if (name == 'owningbusinessunit' && CURRENT_USER['businessunitidname']) {
                        self.text(CURRENT_USER.businessunitidname);
                    } else {
                        self.text(CURRENT_USER.username);
                    }
                    // });
                });

                var hasValueList = [];//保存lookup有值的
                //查找相关引用类型的输入控件
                $('.form-group .lookup ').each(function (i, n) {
                    var self = $(n);
                    var name = self.prop('name').replace(/_text/, '');
                    var inputid = self.prop('id');
                    var valueid = inputid.replace(/_text/, '');
                    var lookupid = self.attr('data-lookup');
                    var defaultLookupUrl = '';
                    var value = self.attr('data-value');
                    var attrid = self.attr('data-attributeid');
                    var disabled = self.prop("disabled");
                    var readonly = self.prop('readonly');

                    $(".attributesLabel").each(function () {
                        var name = $(this).attr("data-sourceattributename");
                        var that = this;
                        if (typeof name === "string") { name = name.toLowerCase(); }
                        var target = Dlv.Page.getAttribute(name);
                    });
                    //设置引入字段的打开地址，同时传入当前实体id和要引入的实体id及字段id
                    
                    if ($("#" + inputid).attr('data-controltype') == 'partylist')//如果是多选
                        singnal = false;
                    else
                        singnal = true;

                    var lookupurl = '/entity/RecordsDialog?singlemode=' + singnal + '&inputid=' + inputid + '&attrid=' + attrid + '&baseentityid=' + Dlv.Page.PageContext.EntityId;

                    //tempobj["FilterOperator"] = 1;
                    if (self.attr('data-defaultviewid') && self.attr('data-defaultviewid') != '') {
                        lookupurl += '&queryid=' + self.attr('data-defaultviewid');
                    }
                    else {
                        lookupurl += '&entityid=' + self.attr('data-lookup');
                    }
                    if (self.attr('data-dependentattributename') && self.attr('data-dependentattributename') != '') {
                        // console.log(self.attr("data-dependentattributename"))
                        //设置对应字段的筛选条件;
                        var filterTarStr = "" + self.attr("data-dependentattributename") + "_text";
                        var filterTarget = $("#" + filterTarStr);
                        if (filterTarget.length > 0) {
                            filterTarget.attr("data-sourceName", self.attr("id"));//如果有对应的字段需要添加相关过滤条件
                        }
                    }
                    defaultLookupUrl = lookupurl;
                    
                    
                    //getvalue
                    if (value && value != '') {
                        //syncCount++;
                        hasValueList.push(inputid);
                        getRecordInfo(valueid, function (response) {
                           
                            if (getPageType() == "edit") {
                                var sourceAttrDom = $('div.attributesLabel[data-sourceattributename="' + valueid + '"]');
                                var extAttrDom = $('.extParamEnti[data-extsourceattributename="' + valueid + '"]');
                                if (sourceAttrDom.length > 0) {
                                    sourceAttrDom.each(function () {
                                        //var tarEntityid = $(this).attr("data-lookup");
                                        var tarType = $(this).attr("data-attributename");
                                        var controlType = $(this).attr("data-sourceattributetype");
                                        var that = this;
                                        setlabelsToTarget(lookupid, value, function (data) {
                                            toSetLabels(that, data, tarType, controlType);
                                        });
                                    });
                                }
                            }
                            if (self.attr('data-onlylabel') && self.attr('data-onlylabel') == 'true') {
                                self.next().remove();
                                self.replaceWith('<a href="' + ORG_SERVERURL + '/entity/edit?entityid=' + lookupid + '&recordid=' + value + '" target="_blank"><span name="' + self.prop('name') + '" id="' + self.prop('id') + '">' + response.content.name + '</span></a>');
                            }
                            else {
                                var attr = Dlv.Page.getAttribute(self.prop('name').replace(/_text/, ''));
                                var btnDis;//如果是多选，输入框自动只读按钮不是只读（没有设置只读时）
                                if (!singnal)//如果是多选
                                {
                                    $("#" + inputid).val(response[valueid.toLowerCase()]);
                                    btnDis = false || readonly;
                                }
                                else {
                                    $("#" + inputid).val(response[valueid.toLowerCase() + "name"]);
                                    $("#" + valueid).val(response[valueid.toLowerCase()]);
                                    btnDis = disabled || readonly;
                                }
                                self.lookup({// 引入字段点击放大镜按钮后
                                    disabled: disabled || readonly,
                                    btnDisabled: btnDis,
                                    dialog: function (obj, callback) {
                                        var f = $('#' + obj.prop('id').replace(/_text/, '')).attr("data-filter");
                                        if (f) f = JSON.parse(decodeURIComponent(f));
                                        else f = null;
                                        var inputDom = $("#" + inputid);
                                        var valueDom = $("#" + valueid);
                                        var dependName = inputDom.attr('data-dependentattributename');
                                        if (typeof dependName === "string" && dependName != "") {
                                            lookupurl = defaultLookupUrl;
                                            var TargetDom = $("#" + dependName);
                                            var targetName = TargetDom.attr("data-sourcename");//打开搜索框前 增加过滤条件
                                            lookupurl += '&dependentattributename=' + inputDom.attr('data-dependentattributename') + '&RelationshipName=' + inputDom.attr("data-filterrelationshipname") + '&ReferencedRecordId=' + (TargetDom.val() || "");
                                        }
                                        if (self.attr('data-defaultviewid') && self.attr('data-defaultviewid') != '') {
                                            lookupurl = $.setUrlParam(lookupurl, 'queryid', self.attr('data-defaultviewid'));
                                        }
                                        else {
                                            lookupurl = $.setUrlParam(lookupurl, 'entityid', self.attr('data-lookup'));
                                        }
                                        if (f) {
                                            Dlv.web.OpenDialog(lookupurl, 'selectRecordCallback', { filter: f }, function () { callback && callback(); });
                                        } else {
                                            Dlv.web.OpenDialog(lookupurl, 'selectRecordCallback', null, function () { callback && callback(); });
                                        }
                                    }
                                    , clear: function () {// 引入字段点击删除按钮后
                                        $('#' + inputid).val('').trigger('change');
                                        $('#' + valueid).val('').trigger('change');
                                        var idsname=inputid.substring(0,inputid.length-5);
                                        $('#' + idsname + 'ids').val('');
                                        //更新只读列表
                                        if($('#' + inputid).attr('readonly')=='readonly'){
                                            read_old_value[$('#' + inputid).attr('id')]=$('#' + inputid).val();
                                        }
                                        if($('#' + idsname + 'ids').attr('readonly')=='readonly'){
                                            read_old_value[$('#' + idsname + 'ids').attr('id')]=$('#' + idsname + 'ids').val();
                                        }

                                        if ($('#' + inputid).siblings(".xms-dropdownLink").length > 0) {
                                            $('#' + inputid).siblings(".xms-dropdownLink").remove();
                                        }
                                        $('#' + inputid).css('color', '#555');
                                        var sourceAttrDom = $('div.attributesLabel[data-sourceattributename="' + valueid + '"]');
                                        var extAttrDom = $('.extParamEnti[data-extsourceattributename="' + valueid + '"]');
                                        //清除关联的数据
                                        if (typeof extAttrDom != "undefined" && extAttrDom.length > 0) {
                                            extAttrDom.each(function () {
                                                var tarEntityid = $(this).attr("data-lookup");
                                                var tarType = $(this).attr("data-extattributename");
                                                var controlType = $(this).attr("data-controltype");
                                                removeExtParam($(this), tarType, controlType);
                                            });
                                        }
                                        if (typeof sourceAttrDom != "undefined" && sourceAttrDom.length > 0) {
                                            sourceAttrDom.each(function () {
                                                var tarEntityid = $(this).attr("data-lookup");
                                                var tarType = $(this).attr("data-extattributename");
                                                var controlType = $(this).attr("data-controltype");
                                                removeLabelParam($(this), tarType, controlType);
                                            });
                                        }
                                    },
                                    isDefaultSearch: true
                                    , isShowSearch: true,
                                    searchOpts: {
                                        id: lookupid
                                        , addHandler: function (tar, obj, par) {
                                            dirtyChecker.setValue(tar.attr("id"), tar.val());
                                            dirtyChecker.checkWatchs(function () {
                                                bindBeforeUnload({});
                                            });
                                            //console.log(tar)
                                            $(par.obj).trigger("lookup.triggerChange");
                                            $(tar).trigger("change");
                                            $(tar).trigger("label.changeLabel");
                                            $(tar).trigger("extend.changetext");
                                        }
                                        , delHandler: function (input) {
                                            var tarid = input.attr("id").replace("_text", "");
                                            var tarDom = $("#" + tarid);
                                            var tagContext = $('div[data-sourceattributename="' + tarid + '"]');
                                            tagContext.html('');
                                        }
                                    }
                                });
                            }
                            setTimeout(function () {
                                $(document).scrollTop(0);
                            }, 0);
                        });
                        //  });
                    }
                    else {
                        if (self.attr('data-onlylabel') && self.attr('data-onlylabel') == 'true') {
                            self.next().remove().end().remove();
                        }
                        else {
                           // var attr = Dlv.Page.getAttribute(name);
                           // 新建时设置所有者、创建者、修改者及所有者部门的默认值
                           // mzg
                           // 202009130918
                            if (self.attr('data-controltype') == 'owner' || name == 'createdby' || name == 'modifiedby' || name=='owningbusinessunit') {
                                // if(getPageType()=="new"){
                                // attr.setValue({id:CURRENT_USER.systemuserid, name: CURRENT_USER.username});
                                var inputDom = $("#" + inputid);
                                var valueid = inputid.replace(/_text/, '');
                                var valueDom = $('#' + valueid);
                            if (name == 'owningbusinessunit') {
                                inputDom.val(CURRENT_USER.businessunitname);
                                valueDom.val(CURRENT_USER.businessunitid);
                                        } else {
                                            inputDom.val(CURRENT_USER.username);
                                            valueDom.val(CURRENT_USER.systemuserid);
                                        }
                                //  }else{}
                            }
                            //系统默认字段设置为灰色只读去掉了所有者部门|| name == 'owningbusinessunit'
                            if (name == 'createdby' || name == 'modifiedby' || name == 'organizationid'  || name == 'owneridtype' || name == 'versionnumber') {
                                self.prop('disabled', true);
                                self.siblings('input').prop('disabled', true);
                            }
                            else {
                                var btnDis ;//如果是多选，输入框自动只读按钮不是只读（没有设置只读时）
                                if (singnal)
                                    btnDis = disabled || readonly;
                                else
                                    btnDis = false || readonly;

                                self.lookup({//设置引用、多选类型的字段值及显示放大镜
                                    disabled: disabled || readonly,
                                    btnDisabled: btnDis,
                                    dialog: function (obj, callback) {
                                        var f = $('#' + obj.prop('id').replace(/_text/, '')).attr("data-filter");
                                        if (f) f = JSON.parse(decodeURIComponent(f));
                                        else f = null;

                                        var inputDom = $("#" + inputid);
                                        var dependName = inputDom.attr('data-dependentattributename');
                                        if (typeof dependName === "string" && dependName != "") {
                                            lookupurl = defaultLookupUrl;
                                            var TargetDom = $("#" + dependName);
                                            var targetName = TargetDom.attr("data-sourcename");//打开搜索框前 增加过滤条件

                                            lookupurl += '&dependentattributename=' + inputDom.attr('data-dependentattributename') + '&RelationshipName=' + inputDom.attr("data-filterrelationshipname") + '&ReferencedRecordId=' + (TargetDom.val() || "");
                                        }
                                        if (self.attr('data-defaultviewid') && self.attr('data-defaultviewid') != '') {
                                            lookupurl = $.setUrlParam(lookupurl, 'queryid', self.attr('data-defaultviewid'));
                                        }
                                        else {
                                            lookupurl = $.setUrlParam(lookupurl, 'entityid', self.attr('data-lookup'));
                                        }
                                        Dlv.web.OpenDialog(lookupurl, 'selectRecordCallback', { filter: f }, function () { callback && callback(); });
                                    }
                                    , clear: function () {
                                        $('#' + inputid).val('').trigger('change');
                                        var idsname=inputid.substring(0,inputid.length-5);
                                        $('#' + idsname + 'ids').val('');
                                        if ($('#' + inputid).siblings(".xms-dropdownLink").length > 0) {
                                            $('#' + inputid).siblings(".xms-dropdownLink").remove();
                                        }
                                        //更新只读列表
                                        if($('#' + inputid).attr('readonly')=='readonly'){
                                            read_old_value[$('#' + inputid).attr('id')]=$('#' + inputid).val();
                                        }
                                        if($('#' + idsname + 'ids').attr('readonly')=='readonly'){
                                            read_old_value[$('#' + idsname + 'ids').attr('id')]=$('#' + idsname + 'ids').val();
                                        }
                                        var valueid = inputid.replace(/_text/, '');
                                        $('#' + valueid).val('').trigger('change');
                                        $('#' + inputid).css('color', '#555');
                                        //清除关联的数据
                                        var sourceAttrDom = $('div.attributesLabel[data-sourceattributename="' + valueid + '"]');
                                        var extAttrDom = $('.extParamEnti[data-extsourceattributename="' + valueid + '"]');
                                        if (typeof extAttrDom != "undefined" && extAttrDom.length > 0) {
                                            extAttrDom.each(function () {
                                                var tarEntityid = $(this).attr("data-lookup");
                                                var tarType = $(this).attr("data-extattributename");
                                                var controlType = $(this).attr("data-controltype");
                                                removeExtParam($(this), tarType, controlType);
                                            });
                                        }
                                        if (typeof sourceAttrDom != "undefined" && sourceAttrDom.length > 0) {
                                            sourceAttrDom.each(function () {
                                                var tarEntityid = $(this).attr("data-lookup");
                                                var tarType = $(this).attr("data-extattributename");
                                                var controlType = $(this).attr("data-controltype");
                                                removeLabelParam($(this), tarType, controlType);
                                            });
                                        }
                                    },
                                    isDefaultSearch: true
                                    , isShowSearch: true
                                    , searchOpts: {
                                        id: lookupid,
                                        addHandler: function (tar, obj, par) {
                                            dirtyChecker.setValue(tar.attr("id"), tar.val());
                                            dirtyChecker.checkWatchs(function () {
                                                bindBeforeUnload({});
                                            });
                                            $(par.obj).trigger("lookup.triggerChange");
                                            $(tar).trigger("change");
                                            //console.log(tar)
                                            $(tar).trigger("label.changeLabel");
                                            $(tar).trigger("extend.changetext");
                                        }, delHandler: function (input) {
                                            var tarid = input.attr("id").replace("_text", "");
                                            var tarDom = $("#" + tarid);
                                            var tagContext = $('div[data-sourceattributename="' + tarid + '"]');
                                            tagContext.html('');
                                        }
                                    }
                                });
                            }
                        }
                    }
                    // if(disabled || readonly){
                    //      Dlv.Page.setDisabled(name,true);
                    // }
                });

                //查找相关nvarchar、money类型的输入控件并设置对应的默认值
                //mzg
                //202012041119
                $('.form-group .nvarchar,.money,.ntext,.float,.int').each(function (i, n) {
                    if (Dlv.Page.Form.State == Dlv.Page.FormState.Create) {//如果是修改状态
                        var self = $(n);
                        var name = self.prop('name').replace(/_text/, '');
                        var inputid = self.prop('id');
                        var valueid = inputid.replace(/_text/, '');
                        var value = self.attr('data-value');
                        var attrid = self.attr('data-attributeid');
                        var disabled = self.prop("disabled");
                        var readonly = self.prop('readonly');
                        var inputDom = $("#" + inputid);
                        var valueid = inputid.replace(/_text/, '');
                        var valueDom = $('#' + valueid);
                        //如果默认值是以#JS--开头并以--JS#结束系统会自动执行其间的代码并赋值，也可以是一个常量
                        for (var j = 0; j < _duplicate.length; j++) {
                            if (_duplicate[j].attributeid == attrid) {//如果是此字段设置的默认值
                                if (_duplicate[j].defaultvalue !== undefined) {
                                    if (_duplicate[j].defaultvalue.indexOf("#JS--") != -1 && _duplicate[j].defaultvalue.indexOf("--JS#") != -1) {
                                        var valueStr = eval(_duplicate[j].defaultvalue.replace("#JS--", "").replace("--JS#", ""));
                                        inputDom.val(valueStr);
                                    }
                                    else
                                        inputDom.val(_duplicate[j].defaultvalue);
                                } else
                                    inputDom.val(_duplicate[j].defaultvalue);
                            }
                            
                        };
                    }
                });

                $(".attributesLabel").each(function () {
                    var name = $(this).attr("data-sourceattributename");
                    var that = this;
                    if (typeof name === "string") { name = name.toLowerCase(); }
                    var target = Dlv.Page.getAttribute(name);
                    var tarEntityid = target.Target.attr("data-lookup");
                    //console.log(target.Target);
                    var value = target.getValue(name);
                    target.Target.bind("label.changeLabel", function () {
                        var changeEntityid = $(this).attr("data-lookup");
                        var tarType = $(that).attr("data-attributename").toLowerCase();
                        //console.log(tarType)
                        var controlType = $(that).attr("data-sourceattributetype");
                        var changeValue = $(this).val();
                        setlabelsToTarget(changeEntityid, changeValue, function (data) {
                            toSetLabels(that, data, tarType, controlType);
                            target.Target.trigger('extend.label.changeEnd');
                        });
                    });
                });
                //console.log($(".extParamEnti"))
                $(".extParamEnti").each(function () {
                    var name = $(this).attr("data-extsourceattributename");

                    if (name == "") return true;
                    var that = this;
                    if (typeof name === "string") { name = name.toLowerCase(); }
                    var target = Dlv.Page.getAttribute(name);
                    var tarEntityid = target.Target.attr("data-lookup");
                    target.Target.bind("extend.changetext", function () {
                        var changeEntityid = $(this).attr("data-lookup");
                        //console.log("extend:",changeEntityid)
                        var tarType = $(that).attr("data-extattributename").toLowerCase();
                        var controltype = $(that).attr("data-controltype");
                        var changeValue = $(this).val();
                        setlabelsToTarget(changeEntityid, changeValue, function (data) {
                            //console.log("setE",data.content);
                            toSetExts(that, data.content, tarType, controltype);
                            target.Target.trigger('extend.changeEnd');
                        });
                    });
                });

                if (getPageType() == 'edit') {
                    $('#attachSection').show();
                }

                function loadDataGrid() {
                    renderDataGrid();
                }

                $('.createForm-openWin').each(function () {
                    var $this = $(this), href = $this.attr('data-href');
                    href = $.setUrlParam(href, 'grid', '');
					$this.removeAttr('target');
                    $this.attr('href', '#');
					$this.bind('click', function () {
						window.parent.$('.entityCreateSection-close').trigger('click');
						window.open(href);
					});
                });

                //选项输入框设置是否
                $('.picklist,.bit').each(function (i, n) {
                    var self = $(n);
                    var items = JSON.parse(decodeURIComponent(self.attr('data-items')));
                    var isselect =  self.val() == "1" ? false : true;
                    var style = self.attr('data-displaystyle') || 'select';
                    if (self.hasClass('bit')) {
                        style = 'radio';
                    }
                    //console.log(self.val())
                    self.picklist({
                        required: self.is('.required'),
                        items: items
                        , displaytype: style
                        , isDefault: isselect
                        , isReadonly: self.prop('readonly')
                    });
                });

                //通过引用实体添加时，则填充引用字段值
                if (Dlv.Page.PageContext.RelationShipName != '') {
                    Dlv.web.GetJson('/api/schema/relationship/' + Dlv.Page.PageContext.RelationShipName, null, function (result) {
                        if (result) {
                            var attr = Dlv.Page.getAttribute(result.content.referencingattributename.toLowerCase());
                            if (Dlv.Page.Form.State != Dlv.Page.FormState.Create) {
                                attr.setDisabled(true);
                                return;
                            }
                            var lookupid = result.content.referencedentityid;
                            var referencedid = Dlv.Page.PageContext.ReferencedRecordId;
                            Dlv.web.GetJson('/api/data/retrieve/ReferencedRecord/' + lookupid + '/' + referencedid, null, function (response) {
                                if (attr.Target.attr('data-onlylabel') && self.attr('data-onlylabel') == 'true') {
                                    attr.Target.next().remove();
                                    attr.Target.replaceWith('<a href="' + ORG_SERVERURL + '/entity/edit?entityid=' + lookupid + '&recordid=' + value + '" target="_blank"><span name="' + self.prop('name') + '" id="' + self.prop('id') + '">' + response.content.name + '</span></a>');
                                }
                                else {
                                    var obj = { id: response.content.id, name: response.content.name };
                                    attr.setValue(obj).setDisabled(true);
                                }
                                attr.Target.trigger('extend.changetext');
                                attr.Target.trigger("label.changeLabel");
                            });
                        }
                        
                    });
                }
                //mzg.Page.getAttribute('serviceid').Target.attr('data-customfilter', '{"FilterOperator":0,"Conditions":[{"AttributeName":"createdon","Operator":2,"Values":["2015-10-24"]}],"Filters":[]}');
                var formIsSave = true;
                /*
                 * 表单验证页面初始化时验证，提交表单后二次验证（在此期间刷新数据）,保存后
                 * mzg
                 * 202010051203
                 */
                Dlv.web.Form($("#editForm"), function (response) {
                    //  console.log(response);
                    if (response.IsSuccess) {//更新成功
                        var data = response;//JSON.parse(response.Content);
                        var gridid = $.getUrlParam('grid');
                        if (window.opener && gridid) {
                            window.opener.Dlv.Page.getControl(gridid).refresh();
                        }
                        else if (window.opener) {
							//判断如果是主表，则刷新局部主表页面
                            if ($.getUrlParam('primaryid') == null) {
                                //lyb改,判断父页面的引用框是否被按如果是则刷新父页面的引用框中数据
                                if (window.opener.parent.$('#entityRecordsModal').length > 0) {
                                    var lookupurl = window.opener.parent.$('table', '#entityRecordsModal').attr('data-pageurl');
                                    var inputid = $.getUrlParam("inputid", lookupurl);
                                    window.opener.parent.Dlv.web.CloseDialog('#entityRecordsModal');
                                    window.opener.parent.$('#' + inputid).parent().nextAll().find('button[name=lookupBtn]').trigger('click');
                                }
                                else {
                                    //是主表，先局部刷新查询页面
                                    window.opener.parent.$("button[title='刷新']").eq(1).trigger('click');
                                }
                            }
                            else {
                                //lyb改,判断父页面的引用框是否被按如果是则刷新父页面的引用框中数据
                                if (window.opener.parent.$('#entityRecordsModal').length > 0) {
                                    var lookupurl = window.opener.parent.$('table', '#entityRecordsModal').attr('data-pageurl');
                                    var inputid = $.getUrlParam("inputid", lookupurl);
                                    window.opener.parent.Dlv.web.CloseDialog('#entityRecordsModal');
                                    window.opener.parent.$('#' + inputid).parent().nextAll().find('button[name=lookupBtn]').trigger('click');
                                }
                                else {
                                    //是子表，则因为总金额需要先更新金额，然后刷新全部页面
                                    gengxinzbzd();
                                    window.opener.location.reload();
                                }
							}
                        }
                        else if (window.parent && gridid) {
                            window.parent.Dlv.Page.getControl(gridid).refresh();
                        }
                        else {
                            //刷新视图列表数据，调用create.cshtml中的js
                            //mzg
                            //202010042217
                            WindowPostMessage('refresh');
                        }
                        aftersaveid = data.Extra.id;//保存时获取主表ID
                        //表单提后检测方法，可通过Dlv.FormSubmitAfter.add(fun)方法添加，保存后
                        if (Dlv && Dlv.FormSubmitAfter) {
                            if (!Dlv.FormSubmitAfter.check(data)) {
                                return false;
                            }
                        }
                        Dlv.web.Alert(true, data.Content, function () {
                            var url = location.href;
                            url = $.setUrlParam(url, 'copyid', null);
                            if (_formSaveAction == Dlv.FormSaveAction.save) {
                               
                                url = $.setUrlParam(url, 'recordid', data.Extra.id);
                                if (Dlv.Page.PageContext.EntityName == 'Roles') {
                                    url = ORG_SERVERURL + '/role/editrole?id=' + data.Extra.id
                                }
                                location.href = url;
                            }
                            else if (_formSaveAction == Dlv.FormSaveAction.saveAndNew) {
                                url = $.setUrlParam(url, 'recordid', null);
                                if (Dlv.Page.PageContext.EntityName == 'Roles') {
                                    url = ORG_SERVERURL + '/role/editrole?id=' + data.Extra.id
                                }
                                location.href = url;
                            }
                            else if (_formSaveAction == Dlv.FormSaveAction.saveAndClose) {
                                Dlv.web.CloseWindow();
                            }
                        }, function () {
                            var url = location.href;
                            url = $.setUrlParam(url, 'recordid', data.Extra.id);
                            location.href = url;
                        });
                    }
                    else {
                        formIsSave = true;
                        Dlv.web.Alert(false, response.Content);
                    }
                }, null, function () {
                    if (Dlv && Dlv.FormPrevSubmit.list.length>0) {//表单前检测方法，可通过Dlv.FormPrevSubmit.add(fun)方法添加
                            var func = getFunction(Dlv.FormPrevSubmit.list[0].name);
                            if (func) {
                                if (!Dlv.FormPrevSubmit.check(func)) {
                                    return false;
                                }
                            }
                    }
                    if (!dirtyChecker.isDirty) {
                        return false;
                    }
                    if (!formIsSave) {
                        return false;
                    }
                    //保存单据体数据
                    if (entityDatagirdList && entityDatagirdList.list.length > 0) {
                        var flag = true;
                        var gridDatas = [];
                        $.each(entityDatagirdList.list, function (i, n) {
                            var _grid = n;
                            var info = n.getAndValidDatas();
                            if (info && info.isvalid == false) {
                                flag = false;
                                console.log(info.requiredDatas);
                                var msg = [];
                                if (info.requiredDatas && info.requiredDatas.length > 0) {
                                    $.each(info.requiredDatas, function (i, n) {
                                        msg.push('<p>' + n.label + '：不能为空</p>')
                                    });
                                    Dlv.web.Toast(msg.join(''), false)
                                }
                                return false;
                            }
                            var postdata = [];

                            var griddatas = n.getEditedDatas();
                            if (griddatas && griddatas.length > 0) {
                                $.each(griddatas, function (ii, nn) {
                                    var temp = { data: nn, name: n.datas.entityName, entityid: n.datas.entityId, relationshipname: n.datas.relationshipname, rownumber: i };
                                    if (nn.cdatagrid_editer === 'new') {
                                        temp.data[n.datas.entityName + 'id'] = Dlv.Utility.Guid.EmptyGuid.ToString()
                                    }
                                    gridDatas.push(temp);
                                });
                                // gridDatas.push(griddatas);
                            }
                            var deldatas = n.deleteList;
                            if (deldatas && deldatas.length > 0) {
                                $.each(deldatas, function (ii, nn) {
                                    var data = {
                                        data: {}, name: n.datas.entityName, entityid: n.datas.entityId, relationshipname: n.datas.relationshipname, rownumber: i, entitystatus: 3
                                    };
                                    data.data[n.datas.entityName + 'id'] = nn[n.datas.entityName + 'id']

                                    gridDatas.push(data);
                                });
                            }
                        });

                        //如果有字段是富文本的话
                       

                        if (flag == false) {
                            return false;
                        }
                        console.log(gridDatas);
                        console.log(JSON.stringify(gridDatas));
                        $('#child').val(encodeURIComponent(JSON.stringify(gridDatas)));
                    } else {
                        // $('#child').val('');
                        }
                        $('form:first').find('textarea.ntext').each(function () {
                            var html = $(this).val();
                            html = decodeURIComponent(html);
                            var $html = $(html);
                            var $wrap = $('<div></div>');
                          //  $wrap.appendTo($('body'));
                            $wrap.html(html);
                            if (editor_files) {
                                $.each(editor_files, function (i,n) {
                                    var $file = $wrap.find('img[title="' + n._id + '"]');
                                    if ($file.length > 0) {
                                        var url_pre = '/upload/attachment/'+CURRENT_USER.organizationid+'/'
                                        $file.attr('src', url_pre+ n._id + '.' + n.filetype.replace('image\/',''));
                                    }
                                });
                            }
                            $(this).val($wrap.html());
                        });
                    console.log('dirtyChecker.isDirty', dirtyChecker.isDirty);
                    formIsSave = false;//防止重复提交
                }, { //setting
                    ignore: ".readonly"
                });

                $('.collapse').collapse({
                    toggle: false
                });

                //加载富文本插件
                var ntextPrefix = 'ntext_';
                $('.ntext').each(function () {
                    var _id = this.id;
                    var $this = $(this);
                    var width = $this.parent().width();
                    var ue = UE.getEditor(ntextPrefix + _id, {
                        initialFrameHeight: 300,
                        initialFrameWidth: width,
                        autoHeightEnabled: false
                    });
                    $this.data().ue = ue;
                    ue.addListener("contentChange", function () {
                        console.log(ue.getContent())
                        var value = encodeURIComponent(ue.getContent());
                        console.log($this)
                        $this.val(value);
                        $this.trigger('change');
                    });
                });

                $('.freetext').each(function () {
                    var val = $(this).attr('value');
                    if (val) {
                        val = decodeURIComponent(val);
                        $(this).html(val);
                    }
                });

                //return false;
                $('a[onClick^=Save]').bind("click", function () {
                    unBindBeforeUnload();
                });

                $("#toolbar").hideBySize({ type: "count", count: 5, itemClass: '.hideBySizeItem:not(.hide)' });

                //加载单据体
                pageWrap_Create.loadSubGrid();

                //加载附件相关
                pageWrap_Create.loadFileUploader();

                //日期选择框
                $.datetimepicker.setLocale('zh');
                $('.datepicker:not(:disabled)').each(function () {
                    var format = $(this).attr('data-fmdata') || 'yyyy/MM/dd hh:mm:ss';
                    // var isdisabled = $(this).prop('readonly');
                    // if(isdisabled)return false;
                    // console.log('datepicker.isdisabled',isdisabled);
                    if ($(this).prop('readonly')) {
                        $(this).parents('.form-items-row:first').find('label').attr('for', '');
                    }
                    if (format.indexOf("hh:mm") > -1) {
                        format = format.replace("yyyy", "Y").replace("dd", "d").replace("hh", "h").replace("mm", "i").replace('MM', "m").replace('ss', "s").replace('HH', "H").replace('h', "H");
                        $(this).datetimepicker({
                            language: "en"
                            , step: 15
                            , scrollInput: !1
                            , format: format
                        });
                    } else {
                        format = format.replace("yyyy", "Y").replace("dd", "d").replace('MM', "m");
                        $(this).datetimepicker({
                            timepicker: false
                            , step: 15
                            , scrollInput: !1
                            , format: format
                        });
                    }
                });

                //将只读的字段值存入数组方便在提交时进行比对
                var read =$("#editForm [readonly]");
                read_old_value={};
                read.each(function(i,n) {
					//lyb改 判断是金额类型去除千位符
					if($(this).attr('data-controltype')=='money'){
						read_old_value[$(this).attr("id")]=$(this).val().replace(/,|\s/g, '');
					}
					else{
						read_old_value[$(this).attr("id")]=$(this).val();
					}
                });
                //加载子表时将主表id赋值
                zibiaogxzbid();

                dirtyChecker.bindCheckerEvent(dirtyChecker.watchs);

                $(document.body).on('xmsChecker.dirty', function () {
                    dirtyChecker.setFormDirtyValue();
                });

                if (pageWrap_Create.cellsStyles && pageWrap_Create.cellsStyles.length > 0) {
                    $.addClassToElement(null, null, pageWrap_Create.cellsStyles.join(''))
                }
            });
        },
        loadFileUploader: function () {
            var fileLimitSize = 100;
            if ($(".uploadify-field").length > 0) {
                $(".uploadify-field").uploadTo64({
                    uploadEnd: function (txshow, value) {
                        var id = $(value).attr("id");
                        dirtyChecker.setValue(id, $(value).val());
                        dirtyChecker.checkWatchs(function () {
                            bindBeforeUnload();
                        });
                        $(value).trigger('change');
                    }
                });
                $(".upload-file-del").bind("click", function () {
                    var $this = $(this);
                    var fileUp = $this.siblings(".upload-file").children(".uploadify-field");
                    var fileValue = $this.siblings(".uploadinput");
                    var fileText = $this.siblings(".upload-file-input");
                    $this.parents('.upload-file-box:first').find('.uploadinput').trigger('change');
                    dirtyChecker.isDirty = true;
                    fileUp.val('');
                    fileValue.val('');
                    fileText.text('');
                    //dirtyChecker.setValue(fileValue.attr("id"), '');
                    //dirtyChecker.checkWatchs(function () {
                    //    bindBeforeUnload();
                    //});
                });
            }
            $(".upload-file-input").imgShow();
        }
        , loadSubGrid: function () {
            
            //如果页面所有单据体都不需要异步加载
            if (isSyncLoadSubGrid == false) {
                var subGridDom = $('.subgrid');
                syncCount = subGridDom.length;
                if (syncCount > 0) {
                    subGridDom.each(function (i, n) {
                        var $this = $(n);
                        //单据体视图id，单据体实体名
                        Dlv.Page.PageContext.SubQueryViewId = $this.context.dataset.viewid;
                        Dlv.Page.PageContext.SubEntityName = $this.context.id.substring(0, $this.context.id.length - 2);
						//求单据体id(lyb新增功能)
						if(Dlv.Page.PageContext.SubEntityName){
							var filter = { "Conditions": [{ "AttributeName": "name", "Operator": 0, "Values": [Dlv.Page.PageContext.SubEntityName]}] };
							var queryObj = { EntityName: 'entity', Criteria: filter, ColumnSet: { allcolumns: true } };
							var data = JSON.stringify({ "query": queryObj, "isAll": true });
							Dlv.web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
								if (response.content && response.content.length > 0) {
									Dlv.Page.PageContext.SubEntityId=response.content[0].entityid;
								}
							}, null, null, 'post',true);
						}
                        var $parent = $this.parent();
                        $this.appendTo($('body'));
                        renderGridView($this, function (obj) {
                            SetFormStateStep++;
                            var par = obj.parents('.subgrid');
                            $parent.append($this);
                            if (SetFormStateStep == syncCount) {
                                //Dlv.Page.SetFormState(Dlv.Page.Form.State);
                                if (typeof services !== "undefined") {
                                    getEvents(services.getEvents);
                                }
                                obj.trigger('subpage.resetWidth');
                                $('form').trigger('onload', {});//方便在所有数据加载完后触发其他的方法
                                console.log($parent)
                            }
                        });
                    });
                } else {
                    subGridDom.each(function (i, n) {
                        var $this = $(n);
                        var $parent = $this.parent();
                        $this.appendTo($('body'));
                        renderGridView($this, function (obj) {
                            var par = obj.parents('.subgrid');
                            obj.trigger('subpage.resetWidth');
                            console.log($parent)
                            $parent.append($this);
                        });
                    });
                   // Dlv.Page.SetFormState(Dlv.Page.Form.State);
                    if (typeof services !== "undefined") {
                        getEvents(services.getEvents);
                    }
                }
            }

            //如果不异步加载单据体信息
            if (isSyncLoadSubGrid == false) {
                //防止文字被隐藏，放到最后
                $(".body").formTab({
                    selecter: ".formTab",
                    type: "after"
                });
            } else {
                var subGridDom = $('.noasncSubgrid');//页面加载时直接加载单据体
                // console.log('not-tabconet', subGridDom);
                syncCount = subGridDom.length;//需要异步加载单据体的数量，为了在表单所有单据体加载完后加载脚本
                if (syncCount > 0) {
                    subGridDom.each(function (i, n) {
                        var $this = $(n);
                        var $parent = $this.parent();
                        $this.appendTo($('body'));
                        renderGridView($this, function (obj) {
                            SetFormStateStep++;
                            var par = obj.parents('.subgrid');
                            $parent.append($this);
                            if (SetFormStateStep == syncCount) {//当所有需要异步加载的单据体全部加载完时加载脚本
                                //Dlv.Page.SetFormState(Dlv.Page.Form.State);
                                if (typeof services !== "undefined") {
                                    getEvents(services.getEvents);
                                }
                                obj.trigger('subpage.resetWidth');
                                $('form').trigger('onload', {});//方便在所有数据加载完后触发其他的方法
                            }
                        });
                    });
                } else {
                    subGridDom.each(function (i, n) {
                        var $this = $(n);
                        var $parent = $this.parent();
                        $this.appendTo($('body'));
                        renderGridView($this, function (obj) {
                            var par = obj.parents('.subgrid');
                            obj.trigger('subpage.resetWidth');
                            $('form').trigger('onload', {});//方便在所有数据加载完后触发其他的方法
                            $parent.append($this);
                        });
                    });
                    //Dlv.Page.SetFormState(Dlv.Page.Form.State);
                    if (typeof services !== "undefined") {
                        getEvents(services.getEvents);
                    }
                }
                //初始化标签页
                $(".body").formTab({
                    selecter: ".formTab",
                    type: "after",
                    clickHandler: function ($obj, $id) {//异步加载对应的单据体
                        var _subgrid = $id.find('.subgrid');
                        if (_subgrid.hasClass('noasncSubgrid')) {//不加载不是异步加载的单据体
                            return false;
                        }
                        var isLoaded = $obj.attr('data-isLoaded');
                        if (isLoaded && isLoaded == "true") {
                            return false;
                        } else {
                            $obj.attr('data-isLoaded', true);
                        }
                        if (_subgrid.length > 0) {
                            renderGridView(_subgrid, function (obj) {
                                var par = obj.parents('.subgrid');
                                //当前表单状态为禁用或者只读时
                                if (Dlv.Page.GetFormState() == Dlv.Page.FormState.Disabled || Dlv.Page.GetFormState() == Dlv.Page.FormState.ReadOnly) {
                                    Dlv.Page.SetFormState(Dlv.Page.GetFormState(), obj);
                                }
                                obj.trigger('subpage.resetWidth');
                            });
                        }
                    }
                });
            }
        }
    }
    function createlabelsToTargetFactory() {
        var cacheFactory = [];//缓存对应的数据
        return function (entityid, value, callback) {
            //console.log(controltype)

            if (cacheFactory[entityid + value] != undefined) {
                var timer = setInterval(function () {
                    if (cacheFactory[entityid + value].content) {
                        callback(cacheFactory[entityid + value]);
                        clearInterval(timer);
                    }
                }, 50);
                //return false;
            } else {
                cacheFactory[entityid + value] = [];
                // var url = '/api/data/retrieve/' + entityname + '/' + v;
                Dlv.web.GetJson('/api/data/Retrieve/ReferencedRecord/' + entityid + '/' + value + '/true', null, function (data) {
                    cacheFactory[entityid + value] = data;
                    callback(data);
                });
            }
        }
    }
    //生成导航链接html
    function renderFormNav(_form, container) {
        var navDatas = _form.NavGroups;
        console.log(navDatas);
        if (!navDatas || navDatas.length == 0) { return false; }
        var _html = [];
        _html.push('<dl class="list-group col-sm-2" id="formNav">');

        for (var i = 0, len = navDatas.length; i < len; i++) {
            var item = navDatas[i];
            var timestrap = (new Date() * 1).toString(16) + i;
            _html.push('<dt class="list-group-item" data-target="#tab_' + timestrap + '" aria-expanded="true" data-toggle="collapse"><span class="glyphicon glyphicon-chevron-down collapse-title" data-target="#tab_' + timestrap + '" aria-expanded="true" data-toggle="collapse"></span>' + item.Label + '</dt>');
            _html.push('<dd id="tab_' + timestrap + '" class="panel-collapse collapse in" aria-expanded="true">');
            for (var j = 0, jlen = item.NavItems.length; j < jlen; j++) {
                var jitem = item.NavItems[j];
                //如果是实体、链接、函数分别处理
                if (jitem.RelationshipName.length > 0) {
                    _html.push('<div class="list-group-item"><span class="' + jitem.Icon + '"></span><a data-type="1" class="form-navlink" target="_blank" data-href="' + ORG_SERVERURL + '/entity/list?entityid=' + jitem.Id + '&relationshipname=' + jitem.RelationshipName + '">' + jitem.Label + '</a></div>');
                } else if (jitem.Url.length > 0) {
                    _html.push('<div class="list-group-item"><span class="' + jitem.Icon + '"></span><a data-type="0" target="_blank" href="' + jitem.Url + '" class="form-navlink">' + jitem.Label + '</a></div>');
                }
                else {
                    _html.push('<div class="list-group-item"><span class="' + jitem.Icon + '"></span><a class="form-navlink" onclick="' + jitem.Fun + '"><font style="cursor:pointer">' + jitem.Label + '</font></a></div>');
                }
            }
            _html.push('</dd>')
        }
        _html.push('</dl>');
        var formNav = $(_html.join(""));
        container.append(formNav);
        return formNav;
    }

    function removeExtParam(context, type, controltype) {
        var $context = $(context);
        $context.val('');
        type = type || 'name';
        controltype = controltype || "nvarchar";
        $context.trigger('extend.ClearBefore');
        if ($context.is(":disabled")) { return false; }
        if (controltype == "lookup" || controltype == "owner" || controltype == "customer") {
            $context.val('');
            $context.attr("data-id", '');
            $context.siblings(".xms-dropdownLink").remove();
        } else if (controltype == "state") {
            // $context.parent().find("input[type='radio']").prop("checked",false);
            //$context.parent().find("input[type='radio'][value='"+list[type]+"']").prop("checked",true);
            //$context.val(list[type])
        } else if (controltype == "picklist") {
            $context.siblings("select>option:first").prop("selected", true);
        } else {
            $context.val('');
        }
        $context.trigger('extend.clearEnd')
    }

    function removeLabelParam(context, type, controltype) {
        var $context = $(context);
        $context.html('');
        $context.siblings('input').val('');
    }

    function toSetLabels(context, data, type, controltype) {
        var $context = $(context);
        $context.empty();
        var list = data.content || data;
        //console.log("list-------:",list);
        type = type || 'name';

        var html = [];
        if (list[type] == null || typeof list[type] == "undefined") return false;
        console.log('toSetLabels', controltype);
        var _value = list[type];
        if (controltype == "datetime") {
            if (~list[type].indexOf('00:00:00')) {//防止短时间格式会显示长时间
                _value = list[type].replace(/\s00:00:00/, '');
            }
        }
        if (controltype && controltype == "lookup" || controltype == "owner" || controltype == "uniqueidentifier" || controltype == "customer" || controltype == "state" || controltype == "bit" || controltype == "status" || controltype == "picklist") {
            html.push('<span class="label-tag" data-id="' + list.id + '">' + (list[type + 'name'] || '') + '</span>');
        } else {
            html.push('<span class="label-tag" data-id="' + list.id + '">' + (_value || '') + '</span>');
        }

        $context.html(html.join(""));
        $context.siblings('input').val(_value);
    }

    function toSetExts(context, data, type, controltype, isdefault) {
        var $context = $(context);
        //$context.val('');
        var list = data.content || data;

        type = type.toLowerCase() || 'name';
        controltype = controltype || "nvarchar";

        if (controltype == "lookup" || controltype == "owner" || controltype == "customer") {
            if (!list[type]) {
                list[type] = '';
            }
            // console.log("lookup:",list);
            var $hidden = $('#' + $context.attr('id').replace('_text', ""));
            $context.val(list[type + "name"]);
            $hidden.val(list[type]);
            if($context.attr('readonly')=="readonly")
            {
                read_old_value[$context.attr('id')]=list[type + "name"];//更新只读字段数组的值
            }
            $context.attr("data-id", list[type]);
            if (!isdefault) {
                $.fn.xmsSelecteDown.setLookUpState($context);
            }
            dirtyChecker.setValue($context.attr('id').replace('_text', ""), list[type]);
            $hidden.trigger('extend.changetext')
        } else if (controltype == "state") {
            if (!list[type]) {
                list[type] = '';
            }

            $context.parent().find("input[type='radio']").prop("checked", false);
            $context.parent().find("input[type='radio'][value='" + list[type] + "']").prop("checked", true);
            $context.val(list[type]);

            if($context.attr('readonly')=="readonly")
            {
                read_old_value[$context.attr('id')]=list[type];//更新只读字段数组的值
            }
            dirtyChecker.setValue($context.attr('id'), list[type]);
        } else if (controltype == "picklist" || controltype == "status") {
            if (list[type]) {
                $context.siblings("select").find(">option[value='" + list[type] + "']").prop("selected", true);
                $context.val(list[type]);
                if($context.attr('readonly')=="readonly")
                {
                    read_old_value[$context.attr('id')]=list[type];//更新只读字段数组的值
                }
            } else {
                $context.siblings("select").find(">option:first").prop("selected", true);
                $context.val($context.siblings("select").find(">option:first").val());
                if($context.attr('readonly')=="readonly")
                {
                    read_old_value[$context.attr('id')]=$context.siblings("select").find(">option:first").val();//更新只读字段数组的值
                } 
            }
            dirtyChecker.setValue($context.attr('id'), list[type]);
        } else {
            if (!list[type]) {
                list[type] = '';
            }
            $context.val(list[type]);
            if($context.attr('readonly')=="readonly")
            {
                read_old_value[$context.attr('id').replace('_text', "")]=list[type].toString();//更新只读字段数组的值
            }
            dirtyChecker.setValue($context.attr('id').replace('_text', ""), list[type]);
        }

        dirtyChecker.checkWatchs(function () {
            bindBeforeUnload({});
        });
    }
    // 选中引入字段点击确定后，选择引用数据确认后
    function selectRecordCallback(result, inputid) {
        if ($("#" + inputid).attr('data-controltype') == 'partylist') {//如果是多选
            var nameStr = "", valueidStr = "";
            var valueid = inputid.replace(/_text/, '');//显示页面的显示值
            var idsvalue = $('#' + valueid + 'ids').val();
            for (j = 0, len = result.length; j < len; j++) {//将多选引入的值和id去重复循环保存到变量

                if (typeof (idsvalue) == "undefined" || idsvalue == "")//判断ids内容是否为空
                {
                    nameStr += result[j].name + "||";
                    valueidStr += result[j].id + "||";
                } else {
                    //去重复 
                    if (idsvalue.indexOf(result[j].id) == -1 && $('#' + inputid).val() != result[j].id && j != 0) {
                        nameStr += result[j].name + "||";
                        valueidStr += result[j].id + "||";
                    }
                }
            }
            //判断这些字段之前是否有值，如果有值就在原来的值上新增         
            if($('#' + inputid).val()!=''){//更新显示的引入字段中的值
                if(nameStr!='')
                {
                    $('#' + inputid).val($('#' + inputid).val()+'||'+nameStr.substring(0, nameStr.length-2));
                    $('#' + inputid + '').next().next().children().eq(1).html($('#' + valueid + '').val()+'||'+nameStr.substring(0, nameStr.length - 2));
                }else{
                    $('#' + inputid).val($('#' + inputid).val());
                    $('#' + inputid + '').next().next().children().eq(1).html($('#' + valueid + '').val());
                }
            }else{
                $('#' + inputid).val(nameStr.substring(0, nameStr.length-2));
                $('#' + inputid + '').next().next().children().eq(1).html(nameStr.substring(0, nameStr.length - 2));
            }
                 
            if($('#' + valueid + '').val()!='')//更新隐藏的引入字段中的值
            {  
                if(nameStr!='')
                {
                    $('#' + valueid + '').val($('#' + valueid + '').val()+'||'+nameStr.substring(0, nameStr.length - 2));
                }else{
                    $('#' + valueid + '').val($('#' + valueid + '').val());
                }

            }else{
                $('#' + valueid + '').val(nameStr.substring(0, nameStr.length - 2));
            }
            if($('#' + valueid + 'ids').val()!='')//更新ids字段的值
            {
                if(valueidStr!='')
                {
                   $('#' + valueid + 'ids').val($('#' + valueid + 'ids').val()+'||'+valueidStr.substring(0, valueidStr.length - 2));
                }else{
                   $('#' + valueid + 'ids').val($('#' + valueid + 'ids').val());
                }

            }else{
                $('#' + valueid + 'ids').val(valueidStr.substring(0, valueidStr.length - 2));
            }

            //更新到只读列表
            if($('#' + inputid).attr('readonly')=='readonly')
            {
                read_old_value[$('#' + inputid).attr('id')]=$('#' + inputid).val();
            }
            //更新到只读列表
            if($('#' + valueid + 'ids').attr('readonly')=='readonly')
            {
                read_old_value[$('#' + valueid + 'ids').attr('id')]=$('#' + valueid + 'ids').val();
            }

            $('#' + valueid).trigger('change');//可以激活保存按钮，解决只修改引入字段后不能保存的问题
        }
        else {
            $('#' + inputid).val(result[0].name);
            var valueid = inputid.replace(/_text/, '');
            $('#' + valueid).val(result[0].id);
            // setLookUpState($('#'+inputid),"insert");
            $('#' + inputid).trigger('change');
            $('#' + valueid).trigger('change');
            dirtyChecker.setValue(valueid, result[0].id);

            dirtyChecker.checkWatchs(function () {
                bindBeforeUnload({});
            });
            $('#' + valueid).trigger("label.changeLabel");
            $('#' + valueid).trigger("extend.changetext");
            $('#' + inputid).trigger("searchDialog", {});
        }
    }
    /**
     * 新建、编辑时视图、按钮查询的生成
     * author mzg
     * 调用api后传值到m.datagrid.js中
     * @param {any} $this
     * @param {any} callback
     */
    function renderGridView($this, callback) {
        var data = {};
        data = $.extend({}, data);
        data.theme = 'jqgrid';
        if ($this.length > 0) {
            if ($this.attr('data-relationshipname')) {
                data.relationshipname = $this.attr('data-relationshipname');
            }
            //判断当前页面是新增还是修改编辑
            if (Dlv.Page.PageContext.RecordId != null) {
                data.referencedrecordid = Dlv.Page.PageContext.RecordId;
            }
            if ($this.attr('data-pagesize')) {
                data.pagesize = $this.attr('data-pagesize');
                var minheight = (data.pagesize * 1) * 40 + 140 - 60;
                if ((data.pagesize * 1) * 40 > 6.5 * 40) {
                    minheight = 6.5 * 40;
                }
                if (data.theme != 'jqgrid') {
                    $this.css('min-height', minheight);
                    $this.css('max-height', (data.pagesize * 1 + 1) * 40 + 140);
                }
            }
            //根据单据属性设置相关值
            data.height = minheight;
            data.formEntityName = Dlv.Page.PageContext.EntityName.toLowerCase();;
            data.queryviewid = $this.attr('data-viewid');
            data.iseditable = $this.attr('data-editable');//是否可编辑
            data.gridid = $this.attr('name');
            data.PagingEnabled = $this.attr('data-pagingenabled');
            data.DefaultEmptyRows = $this.attr('data-defaultemptyrows');
            data.pageIsEdit = getPageType() == "edit";
            data.formState = Dlv.Page.Form.State;
            data.filter = {}
            if ($this.attr('data-filter')) {
                var _filter = JSON.parse(decodeURIComponent($this.attr('data-filter')));
                if (_filter) {
                    $.extend(data.filter, _filter);
                }
            }
        } else if ($this.queryviewid) {
            $.extend(data, $this);
        }
        if (data.theme == 'jqgrid') {
            if (data.relationshipname && data.relationshipname != '') {
                var rela = data.relationshipname.split('_');
                //当主表名有下划线时进行截取取到对应的主键字段名
                //如：gc_fp为主表名子表中的主键字段应为gc_fpid
                //如果主表没有下划线则取第2位,默认取此数
                var relaentity = rela[2];
                //明细表出错
                if (rela.length >3)
                    relaentity = rela[rela.length - 2] + '_' + rela[rela.length-1];
                
                var relaattribute = relaentity;
                if (!Dlv.Page.PageContext.RecordId) {
                    data.pageIsEdit = false;
                }
                var filter = { operator: 0, Conditions: [{ AttributeName: relaattribute, Operator: 8, values: [Dlv.Page.PageContext.RecordId] }], Filters: [] }
                // data.filter = filter;
                $.extend(data.filter, filter);
            }
            //单据视图信息、单据字段信息
            var url = ORG_SERVERURL + '/api/schema/queryview/GetViewInfo?';
            url += 'id=' + data.queryviewid;
            //调用API方法
            Dlv.web.Get(url, function (res) {
                console.log('getbyentityid', JSON.parse(res.Content));
                //单据体的按钮信息及字段信息
                var jsonres = JSON.parse(res.Content);
                data.queryviews = jsonres.views;
                var _id = $this.attr('id');
                var $grid = $('<div class="entity-datagrid-wrap" id="datagrid_wrap_' + _id + '" data-id="' + _id + '"></div>');//防止datagrid出现显示问题
                $this.append($grid);
                //两项合并，也可深度嵌套或添加静态方法
                var datas = $.extend({}, data, jsonres);
                datas.gridviewLoaded = function () {
                    callback && callback($this);
                }
                var grid = new entityDatagrid(_id, $grid, datas);
                $this.data().__entityDatagrid = grid;
                // grid.setDatas(datas);
                // grid.loadDatagird($grid);
                buttonhidden_zb();//提交后隐藏子表按钮
                console.log(res);
            });
        } else {
            var url = $this.attr('data-url');
            var timestrap = '__r=' + new Date().getTime();
            if ($.getUrlParam && $.getUrlParam('debug') == 'true') {
                timestrap = '';
            }
            url = url + (url.indexOf('?') == -1 ? '?' : '&') + timestrap;
            Dlv.web.LoadPage(url, data, function (response) {
                $this.html(response);
                $this.attr('data-refresh', 'renderGridView($(this))');
                page_common_formular.setSubGridFormular($this, data.theme);//添加值计算
                callback && callback($this);
            });
        }
    }
    function callChildMethod(childid, method) {
        var child = document.getElementById(childid).contentWindow
        if (child && child[method]) {
            child[method](document.body);
        }
    }
    function callParentMethod(method) {
        if (parent && parent[method]) {
            parent[method](document.body);
        }
    }
    //表单提交后隐藏子表按钮
    function buttonhidden_zb() {
        if (Dlv.Page.Form.State == 'readonly') {
            var tablename = Dlv.Page.PageContext.EntityName;//获取主表表名
            var id = Dlv.Page.PageContext.RecordId;//获取当前数据ID
            var filter = { "Conditions": [{ "AttributeName": tablename + 'id', "Operator": 0, "Values": [id] }] };
            var queryObj = { EntityName: tablename, Criteria: filter, ColumnSet: { allcolumns: true } };
            var data = JSON.stringify({ "query": queryObj, "isAll": true });
            Dlv.web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                console.log(Dlv.Page.formHelper);
                var spzt = response.content[0].processstate;//查询审批状态
                if (spzt == '1' || spzt == '2') {
                    $('div[class="toolbar-bottons pull-left"]').hide();//隐藏按钮标签
                }
            }, null, null, 'post', true);
        }
    }

    //加载子表时把主表id赋值到字段
    function zibiaogxzbid() {
        var filter = { "Conditions": [{ "AttributeName": "zibiaoentityid", "Operator": 0, "Values": [Dlv.Page.PageContext.EntityId] }] };
        var queryObj = { EntityName: 'Association', Criteria: filter, ColumnSet: { allcolumns: true } };
        var data = JSON.stringify({ "query": queryObj, "isAll": true });
        Dlv.web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
         //   if (typeof (response) != "undefined") {
                if (response.content && response.content.length > 0) {

                if (response.content.length > 0 && response.content[0].zhujianid) {
                    var zjid = response.content[0].zhujianid.toLowerCase();
                    var primaryid = $.getUrlParam('primaryid')
                    var obj = {};
                    obj.id = primaryid;
                    $('#' + zjid + '_text').attr('data-value', primaryid);
                    Dlv.Page.getAttribute(zjid).setValue(obj);
                }
            }

        }, null, null, 'post');
    }
    window.editor_files = [];
    window.setlabelsToTarget = setlabelsToTarget;
    window.selectRecordCallback = selectRecordCallback;
    window.pageWrap_Create = pageWrap_Create;
    window.callChildMethod = callChildMethod;
    window.callParentMethod = callParentMethod;
    window.renderGridView = renderGridView;
    return pageWrap_Create;
});