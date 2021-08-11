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
    function XmsFilter(filter) {
        filter = filter || {};
        this.FilterOperator = filter.operator || 0;
        this.Conditions = filter.Conditions || [];
        this.Filters = filter.Filters || [];
    }
    XmsFilter.prototype.setOperator = function (operator) {
        this.FilterOperator = operator;
    }
    XmsFilter.prototype.addFilter = function (filter, repeat) {
        if (!repeat) {
            this.Filters.push(filter);
        }
    }
    //只能检查第一层，没做递归检测
    XmsFilter.prototype._checkByConditions = function (objdata) {
        var self = this, flag = false;
        if (this.FilterOperator == 0) {//如果是并
            flag = true;
            if (this.Conditions.length > 0) {
                $.each(this.Conditions, function (i, n) {
                    var operator = n.operator;//操作符
                    var operatorname = Dlv.Fetch.getFilterName(operator);//获取操作符名字
                    var operatorFunc = Dlv.ExtFilter.FilterHandler[operatorname];//获取操作符对比函数;

                    if (typeof operatorFunc === 'function') {
                        var res = operatorFunc(n.values, objdata[n.attributename]);
                        if (!res) {
                            flag = false;
                        }
                    }
                });
            }
        } else {//如果是或
            if (this.Conditions.length > 0) {
                $.each(this.Conditions, function (i, n) {
                    var operator = n.operator;//操作符
                    var operatorname = Dlv.Fetch.getFilterName(operator);//获取操作符名字
                    var operatorFunc = Dlv.ExtFilter.FilterHandler[operatorname];//获取操作符对比函数;

                    if (typeof operatorFunc === 'function') {
                        var res = operatorFunc(n.values, objdata[n.attributename]);
                        if (res) {
                            flag = true;
                            return false;
                        }
                    }
                });
            }
        }
        return flag;
    }
    XmsFilter.prototype.filterByData = function (objdata) {
        if (typeof Xms != 'undefined' && Dlv.Fetch) {
            return this._checkByConditions(objdata);
        } else {
            console.error('请加载fetch.js');
        }
        return false;
    }
    XmsFilter.prototype.addCondition = function (condition, cover) {
        if (!cover) {
            this.Conditions.push(condition);
        } else {
            var index = this.indexOfCondition(condition.AttributeName)
            if (~index) {
                this.Conditions[index] = condition;
            } else {
                this.Conditions.push(condition);
            }
        }
    }

    XmsFilter.prototype.indexOfCondition = function (AttributeName) {
        var index = -1;
        $.each(this.Conditions, function (i, n) {
            if (n.AttributeName == AttributeName) {
                index = i;
                return false;
            }
        });
        return index;
    }

    XmsFilter.prototype.findCondition = function (AttributeName) {
        var index = [];
        $.each(this.Conditions, function (i, n) {
            if (n.AttributeName == AttributeName) {
                index.push(i);
                return true;
            }
        });
        return index;
    }
    //只删除当前FILER里的CONDITION
    XmsFilter.prototype.removeCondition = function (AttributeName) {
        var index = this.findCondition(AttributeName)
        var self = this;
        if (index && index.length > 0) {
            self.Conditions = $.grep(self.Conditions, function (i, n) {
                var flag = true;
                $.each(index, function (key, item) {
                    if (item == n) {
                        flag = false;
                        return false;
                    }
                });
                return flag
            });
        }
    }
    //递归删除所有的CONDITION
    XmsFilter.prototype.removeAllCondition = function (AttributeName) {
        var self = this;
        var removeFiltersIndex = [];
        $.each(this.Filters, function (i, n) {
            if (n.Conditions.length > 0) {
                n.removeCondition(AttributeName);
            }
            if (n.Filters.length > 0) {
                var __condition = n.removeAllCondition(AttributeName);
            }
            if (n.Filters.length == 0 && n.Conditions.length == 0) {
                removeFiltersIndex.push(i);
            }
        });
        self.Filters = $.grep(self.Filters, function (i, n) {
            var flag = true;
            $.each(removeFiltersIndex, function (key, item) {
                if (item == n) {
                    flag = false;
                    return false;
                }
            });
            return flag
        });
        console.log(self.Filters);
    }

    //获取当前Filter的对应得condition
    XmsFilter.prototype.getCondition = function (AttributeName) {
        var conditions = [];
        $.each(this.Conditions, function (i, n) {
            if (n.AttributeName == AttributeName) {
                conditions.push(n);
            }
        });
        return conditions;
    }
    //查找出所有的condition
    XmsFilter.prototype.getAllCondition = function (AttributeName) {
        var conditions = [];
        $.each(this.Filters, function (i, n) {
            var _condition = n.getCondition(AttributeName);
            if (_condition.length > 0) {
                conditions = conditions.concat(_condition);
            }
            if (n.Filters.length > 0) {
                var __condition = n.getAllCondition(AttributeName);
                if (__condition.length > 0) {
                    conditions = conditions.concat(__condition);
                }
            }
        });
        return conditions;
    }
    XmsFilter.prototype.getFilterInfo = function () {
        var res = {};
        res.FilterOperator = this.FilterOperator || 0;
        res.Conditions = this.Conditions;
        res.Filters = [];
        $.each(this.Filters, function (i, n) {
            res.Filters.push(n.getFilterInfo());
        });
        return res;
    }
    XmsFilter.prototype.clearAll = function () {
        this.FilterOperator = 0;
        this.Conditions = [];
        this.Filters = [];
    }
    function XmsCondition(AttributeName, Operator, Values) {
        this.AttributeName = AttributeName || '';
        this.Operator = Operator || 0;
        this.Values = Values || [];
    }

    XmsFilter.changeFiltersToXmsFilter = function (filters) {
        var xmsfilter = new XmsFilter();
        if (filters.Conditions && filters.Conditions.length > 0) {
            xmsfilter.Conditions = filters.Conditions;
        }
        xmsfilter.FilterOperator = filters.FilterOperator;
        if (filters.Filters && filters.Filters.length > 0) {
            $.each(filters.Filters, function (i, n) {
                xmsfilter.Filters.push(XmsFilter.changeFiltersToXmsFilter(n));
            });
        }
        return xmsfilter;
    }
    window.XmsFilter = XmsFilter;
    window.XmsCondition = XmsCondition;
    return {
        XmsFilter: XmsFilter,
        XmsCondition: XmsCondition
    }
});

(function () {
    if (typeof (Dlv) == "undefined") { Dlv = { __namespace: true }; }
    Dlv.ExtFilter = function () { };

    Dlv.ExtFilter.FilterHandlerLabel = {}
    //val1是过滤条件里的值，val2是传进来需要对比的值
    Dlv.ExtFilter.FilterHandler = {
        Equal: function (val1, val2) {
            return val1[0] == val2;
        },
        NotEqual: function (val1, val2) {
            return val1[0] != val2;
        },
        GreaterThan: function (val1, val2) {
            return val1[0] < val2;
        },
        LessThan: function (val1, val2) {
            return val1[0] > val2;
        },
        GreaterEqual: function (val1, val2) {
            return val1[0] <= val2;
        },
        LessEqual: function (val1, val2) {
            return val1[0] >= val2;
        },
        Like: function (val1, val2) {
            return val2.indexOf(val1[0]) != -1;
        },
        NotLike: function (val1, val2) {
            return val2.indexOf(val1[0]) == -1;
        },
        In: function (val1, val2) {
            return val2.indexOf(val1[0]) != -1;
        },
        NotIn: function (val1, val2) {
            return val2.indexOf(val1[0]) == -1;
        },
        Between: function (val1, val2) {
            return val2 < val1[1] && val2 > val1[0];
        },
        NotBetween: function (val1, val2) {
            return val2 > val1[1] || val2 < val1[0];
        },
        Null: function (val1, val2) {
            return val2.indexOf(val1[0]) == -1;
        },
        NotNull: function (val1, val2) {
            return val2.indexOf(val1[0]) != -1;
        },
        Yesterday: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', -1)
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        Today: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date()
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        Tomorrow: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 1)
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        Last7Days: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', -7)
                return val2time.DateDiff('d', checkday) < 0;
            }
        },
        Next7Days: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 7)
                return val2time.DateDiff('d', checkday) > 0;
            }
        },
        LastWeek: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', -1)
                return val2time.DateDiff('w', checkday) < 0;
            }
        },
        ThisWeek: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 0)
                return val2time.DateDiff('w', checkday) == 0;
            }
        },
        NextWeek: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 1)
                return val2time.DateDiff('w', checkday) > 0;
            }
        },
        LastMonth: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', -1)
                return val2time.DateDiff('m', checkday) < 0;
            }
        },
        ThisMonth: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 0)
                return val2time.DateDiff('m', checkday) == 0;
            }
        },
        NextMonth: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 1)
                return val2time.DateDiff('m', checkday) > 0;
            }
        },
        On: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        OnOrBefore: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) <= 0;
            }
        },
        OnOrAfter: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) >= 0;
            }
        },
        Before: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) < 0;
            }
        },
        After: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) > 0;
            }
        },
        LastYear: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) < 0;
            }
        },
        ThisYear: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) == 0;
            }
        },
        NextYear: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) > 0;
            }
        },
        LastXHours: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('h', 0)
                return val2time.DateDiff('h', checkday) < 0;
            }
        },
        NextXHours: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('h', 0)
                return val2time.DateDiff('h', checkday) > 0;
            }
        },
        LastXDays: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) < 0;
            }
        },
        NextXDays: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) > 0;
            }
        },
        LastXWeeks: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 0)
                return val2time.DateDiff('w', checkday) < 0;
            }
        },
        NextXWeeks: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 0)
                return val2time.DateDiff('w', checkday) > 0;
            }
        },
        LastXMonths: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 0)
                return val2time.DateDiff('m', checkday) < 0;
            }
        },
        NextXMonths: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 0)
                return val2time.DateDiff('m', checkday) > 0;
            }
        },
        LastXYears: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) < 0;
            }
        },
        NextXYears: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) > 0;
            }
        },
        EqualUserId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.systemuserid == val2;
                }
            }
        },
        NotEqualUserId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.systemuserid != val2;
                }
            }
        },
        EqualBusinessId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.businessunitid == val2;
                }
            }
        },
        NotEqualBusinessId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.businessunitid != val2;
                }
            }
        },
        ChildOf: 47,
        Mask: 48,
        NotMask: 49,
        MasksSelect: 50,
        Contains: 51,
        DoesNotContain: 52,
        EqualUserLanguage: 53,
        NotOn: 54,
        OlderThanXMonths: 55,
        BeginsWith: 56,
        DoesNotBeginWith: 57,
        EndsWith: 58,
        DoesNotEndWith: 59,
        ThisFiscalYear: 60,
        ThisFiscalPeriod: 61,
        NextFiscalYear: 62,
        NextFiscalPeriod: 63,
        LastFiscalYear: 64,
        LastFiscalPeriod: 65,
        LastXFiscalYears: 66,
        LastXFiscalPeriods: 67,
        NextXFiscalYears: 68,
        NextXFiscalPeriods: 69,
        InFiscalYear: 70,
        InFiscalPeriod: 71,
        InFiscalPeriodAndYear: 72,
        InOrBeforeFiscalPeriodAndYear: 73,
        InOrAfterFiscalPeriodAndYear: 74,
        EqualUserTeams: 75,
        EqualOrganizationId: 76,
        NotEqualOrganizationId: 77,
        OnOrBeforeToday: 78,
        OnOrAfterToday: 79,
        BeforeToday: 80,
        AfterToday: 81,
        OlderThanXYears: 82,
        OlderThanXDays: 83,
        AfterXYears: 84,
        AfterXMonths: 85,
        AfterXDays: 86
    };
    //models

    ////类型比较符
    //mzg.ExtFilter.FilterHandler.CommonOperators = [];
    //mzg.ExtFilter.FilterHandler.CommonOperators.push(["Equal", Dlv.ExtFilter.FilterHandler.Equal, Dlv.ExtFilter.FilterHandlerLabel.Equal]);
    //mzg.ExtFilter.FilterHandler.CommonOperators.push(["NotEqual", Dlv.ExtFilter.FilterHandler.NotEqual, Dlv.ExtFilter.FilterHandlerLabel.NotEqual]);
    //mzg.ExtFilter.FilterHandler.CommonOperators.push(["NotNull", Dlv.ExtFilter.FilterHandler.NotNull, Dlv.ExtFilter.FilterHandlerLabel.NotNull]);
    //mzg.ExtFilter.FilterHandler.CommonOperators.push(["Null", Dlv.ExtFilter.FilterHandler.Null, Dlv.ExtFilter.FilterHandlerLabel.Null]);

    //mzg.ExtFilter.FilterHandler.StringOperators = Dlv.ExtFilter.FilterHandler.CommonOperators.concat();
    //mzg.ExtFilter.FilterHandler.StringOperators.push(["Like", Dlv.ExtFilter.FilterHandler.Like, Dlv.ExtFilter.FilterHandlerLabel.Like]);
    //mzg.ExtFilter.FilterHandler.StringOperators.push(["NotLike", Dlv.ExtFilter.FilterHandler.NotLike, Dlv.ExtFilter.FilterHandlerLabel.NotLike]);
    //mzg.ExtFilter.FilterHandler.StringOperators.push(["BeginsWith", Dlv.ExtFilter.FilterHandler.BeginsWith, Dlv.ExtFilter.FilterHandlerLabel.BeginsWith]);
    //mzg.ExtFilter.FilterHandler.StringOperators.push(["DoesNotBeginWith", Dlv.ExtFilter.FilterHandler.DoesNotBeginWith, Dlv.ExtFilter.FilterHandlerLabel.DoesNotBeginWith]);
    //mzg.ExtFilter.FilterHandler.StringOperators.push(["EndsWith", Dlv.ExtFilter.FilterHandler.EndsWith, Dlv.ExtFilter.FilterHandlerLabel.EndsWith]);
    //mzg.ExtFilter.FilterHandler.StringOperators.push(["DoesNotEndWith", Dlv.ExtFilter.FilterHandler.DoesNotEndWith, Dlv.ExtFilter.FilterHandlerLabel.DoesNotEndWith]);

    //mzg.ExtFilter.FilterHandler.NumberOperators = Dlv.ExtFilter.FilterHandler.CommonOperators.concat();
    //mzg.ExtFilter.FilterHandler.NumberOperators.push(["GreaterThan", Dlv.ExtFilter.FilterHandler.GreaterThan, Dlv.ExtFilter.FilterHandlerLabel.GreaterThan]);
    //mzg.ExtFilter.FilterHandler.NumberOperators.push(["LessThan", Dlv.ExtFilter.FilterHandler.LessThan, Dlv.ExtFilter.FilterHandlerLabel.LessThan]);
    //mzg.ExtFilter.FilterHandler.NumberOperators.push(["GreaterEqual", Dlv.ExtFilter.FilterHandler.GreaterEqual, Dlv.ExtFilter.FilterHandlerLabel.GreaterEqual]);
    //mzg.ExtFilter.FilterHandler.NumberOperators.push(["LessEqual", Dlv.ExtFilter.FilterHandler.LessEqual, Dlv.ExtFilter.FilterHandlerLabel.LessEqual]);

    //mzg.ExtFilter.FilterHandler.DateTimeOperators = Dlv.ExtFilter.FilterHandler.CommonOperators.concat();
    ////mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["GreaterThan", Dlv.ExtFilter.FilterHandler.GreaterThan, Dlv.ExtFilter.FilterHandlerLabel.GreaterThan]);
    ////mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LessThan", Dlv.ExtFilter.FilterHandler.LessThan, Dlv.ExtFilter.FilterHandlerLabel.LessThan]);
    ////mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["GreaterEqual", Dlv.ExtFilter.FilterHandler.GreaterEqual, Dlv.ExtFilter.FilterHandlerLabel.GreaterEqual]);
    ////mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LessEqual", Dlv.ExtFilter.FilterHandler.LessEqual, Dlv.ExtFilter.FilterHandlerLabel.LessEqual]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["On", Dlv.ExtFilter.FilterHandler.On, Dlv.ExtFilter.FilterHandlerLabel.On]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["Last7Days", Dlv.ExtFilter.FilterHandler.Last7Days, Dlv.ExtFilter.FilterHandlerLabel.Last7Days]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastWeek", Dlv.ExtFilter.FilterHandler.LastWeek, Dlv.ExtFilter.FilterHandlerLabel.LastWeek]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastMonth", Dlv.ExtFilter.FilterHandler.LastMonth, Dlv.ExtFilter.FilterHandlerLabel.LastMonth]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastYear", Dlv.ExtFilter.FilterHandler.LastYear, Dlv.ExtFilter.FilterHandlerLabel.LastYear]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXHours", Dlv.ExtFilter.FilterHandler.LastXHours, Dlv.ExtFilter.FilterHandlerLabel.LastXHours]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXDays", Dlv.ExtFilter.FilterHandler.LastXDays, Dlv.ExtFilter.FilterHandlerLabel.LastXDays]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXWeeks", Dlv.ExtFilter.FilterHandler.LastXWeeks, Dlv.ExtFilter.FilterHandlerLabel.LastXWeeks]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXMonths", Dlv.ExtFilter.FilterHandler.LastXMonths, Dlv.ExtFilter.FilterHandlerLabel.LastXMonths]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXYears", Dlv.ExtFilter.FilterHandler.LastXYears, Dlv.ExtFilter.FilterHandlerLabel.LastXYears]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["Next7Days", Dlv.ExtFilter.FilterHandler.Next7Days, Dlv.ExtFilter.FilterHandlerLabel.Next7Days]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextWeek", Dlv.ExtFilter.FilterHandler.NextWeek, Dlv.ExtFilter.FilterHandlerLabel.NextWeek]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextMonth", Dlv.ExtFilter.FilterHandler.NextMonth, Dlv.ExtFilter.FilterHandlerLabel.NextMonth]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextYear", Dlv.ExtFilter.FilterHandler.NextYear, Dlv.ExtFilter.FilterHandlerLabel.NextYear]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXHours", Dlv.ExtFilter.FilterHandler.NextXHours, Dlv.ExtFilter.FilterHandlerLabel.NextXHours]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXDays", Dlv.ExtFilter.FilterHandler.NextXDays, Dlv.ExtFilter.FilterHandlerLabel.NextXDays]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXWeeks", Dlv.ExtFilter.FilterHandler.NextXWeeks, Dlv.ExtFilter.FilterHandlerLabel.NextXWeeks]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXMonths", Dlv.ExtFilter.FilterHandler.NextXMonths, Dlv.ExtFilter.FilterHandlerLabel.NextXMonths]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXYears", Dlv.ExtFilter.FilterHandler.NextXYears, Dlv.ExtFilter.FilterHandlerLabel.NextXYears]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OlderThanXYears", Dlv.ExtFilter.FilterHandler.OlderThanXYears, Dlv.ExtFilter.FilterHandlerLabel.OlderThanXYears]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OlderThanXMonths", Dlv.ExtFilter.FilterHandler.OlderThanXMonths, Dlv.ExtFilter.FilterHandlerLabel.OlderThanXMonths]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OlderThanXDays", Dlv.ExtFilter.FilterHandler.OlderThanXDays, Dlv.ExtFilter.FilterHandlerLabel.OlderThanXDays]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterXYears", Dlv.ExtFilter.FilterHandler.AfterXYears, Dlv.ExtFilter.FilterHandlerLabel.AfterXYears]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterXMonths", Dlv.ExtFilter.FilterHandler.AfterXMonths, Dlv.ExtFilter.FilterHandlerLabel.AfterXMonths]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterXDays", Dlv.ExtFilter.FilterHandler.AfterXDays, Dlv.ExtFilter.FilterHandlerLabel.AfterXDays]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrBefore", Dlv.ExtFilter.FilterHandler.OnOrBefore, Dlv.ExtFilter.FilterHandlerLabel.OnOrBefore]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrAfter", Dlv.ExtFilter.FilterHandler.OnOrAfter, Dlv.ExtFilter.FilterHandlerLabel.OnOrAfter]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["Before", Dlv.ExtFilter.FilterHandler.Before, Dlv.ExtFilter.FilterHandlerLabel.Before]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["After", Dlv.ExtFilter.FilterHandler.After, Dlv.ExtFilter.FilterHandlerLabel.After]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["ThisWeek", Dlv.ExtFilter.FilterHandler.ThisWeek, Dlv.ExtFilter.FilterHandlerLabel.ThisWeek]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["ThisMonth", Dlv.ExtFilter.FilterHandler.ThisMonth, Dlv.ExtFilter.FilterHandlerLabel.ThisMonth]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["ThisYear", Dlv.ExtFilter.FilterHandler.ThisYear, Dlv.ExtFilter.FilterHandlerLabel.ThisYear]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["Today", Dlv.ExtFilter.FilterHandler.Today, Dlv.ExtFilter.FilterHandlerLabel.Today]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["Tomorrow", Dlv.ExtFilter.FilterHandler.Tomorrow, Dlv.ExtFilter.FilterHandlerLabel.Tomorrow]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["Yesterday", Dlv.ExtFilter.FilterHandler.Yesterday, Dlv.ExtFilter.FilterHandlerLabel.Yesterday]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrBeforeToday", Dlv.ExtFilter.FilterHandler.OnOrBeforeToday, Dlv.ExtFilter.FilterHandlerLabel.OnOrBeforeToday]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrAfterToday", Dlv.ExtFilter.FilterHandler.OnOrAfterToday, Dlv.ExtFilter.FilterHandlerLabel.OnOrAfterToday]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["BeforeToday", Dlv.ExtFilter.FilterHandler.BeforeToday, Dlv.ExtFilter.FilterHandlerLabel.BeforeToday]);
    //mzg.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterToday", Dlv.ExtFilter.FilterHandler.AfterToday, Dlv.ExtFilter.FilterHandlerLabel.AfterToday]);

    //mzg.ExtFilter.FilterHandler.LookUpOperators = Dlv.ExtFilter.FilterHandler.CommonOperators.concat();
    //mzg.ExtFilter.FilterHandler.LookUpOperators.push(["Like", Dlv.ExtFilter.FilterHandler.Like, Dlv.ExtFilter.FilterHandlerLabel.Like]);
    //mzg.ExtFilter.FilterHandler.LookUpOperators.push(["NotLike", Dlv.ExtFilter.FilterHandler.NotLike, Dlv.ExtFilter.FilterHandlerLabel.NotLike]);
    //mzg.ExtFilter.FilterHandler.LookUpOperators.push(["BeginsWith", Dlv.ExtFilter.FilterHandler.BeginsWith, Dlv.ExtFilter.FilterHandlerLabel.BeginsWith]);
    //mzg.ExtFilter.FilterHandler.LookUpOperators.push(["DoesNotBeginWith", Dlv.ExtFilter.FilterHandler.DoesNotBeginWith, Dlv.ExtFilter.FilterHandlerLabel.DoesNotBeginWith]);
    //mzg.ExtFilter.FilterHandler.LookUpOperators.push(["EndsWith", Dlv.ExtFilter.FilterHandler.EndsWith, Dlv.ExtFilter.FilterHandlerLabel.EndsWith]);
    //mzg.ExtFilter.FilterHandler.LookUpOperators.push(["DoesNotEndWith", Dlv.ExtFilter.FilterHandler.DoesNotEndWith, Dlv.ExtFilter.FilterHandlerLabel.DoesNotEndWith]);

    //mzg.ExtFilter.FilterHandler.OwnerOperators = Dlv.ExtFilter.FilterHandler.CommonOperators.concat();
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["Like", Dlv.ExtFilter.FilterHandler.Like, Dlv.ExtFilter.FilterHandlerLabel.Like]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["NotLike", Dlv.ExtFilter.FilterHandler.NotLike, Dlv.ExtFilter.FilterHandlerLabel.NotLike]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["BeginsWith", Dlv.ExtFilter.FilterHandler.BeginsWith, Dlv.ExtFilter.FilterHandlerLabel.BeginsWith]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["DoesNotBeginWith", Dlv.ExtFilter.FilterHandler.DoesNotBeginWith, Dlv.ExtFilter.FilterHandlerLabel.DoesNotBeginWith]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["EndsWith", Dlv.ExtFilter.FilterHandler.EndsWith, Dlv.ExtFilter.FilterHandlerLabel.EndsWith]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["DoesNotEndWith", Dlv.ExtFilter.FilterHandler.DoesNotEndWith, Dlv.ExtFilter.FilterHandlerLabel.DoesNotEndWith]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["EqualUserId", Dlv.ExtFilter.FilterHandler.EqualUserId, Dlv.ExtFilter.FilterHandlerLabel.EqualUserId]);
    //mzg.ExtFilter.FilterHandler.OwnerOperators.push(["NotEqualUserId", Dlv.ExtFilter.FilterHandler.NotEqualUserId, Dlv.ExtFilter.FilterHandlerLabel.NotEqualUserId]);

    //mzg.ExtFilter.FilterHandler.SystemUserOperators = Dlv.ExtFilter.FilterHandler.OwnerOperators.concat();

    //mzg.ExtFilter.FilterHandler.BusinessUnitOperators = Dlv.ExtFilter.FilterHandler.LookUpOperators.concat();

    //mzg.ExtFilter.FilterHandler.BusinessUnitOperators.push(["EqualBusinessId", Dlv.ExtFilter.FilterHandler.EqualBusinessId, Dlv.ExtFilter.FilterHandlerLabel.EqualBusinessId]);
    //mzg.ExtFilter.FilterHandler.BusinessUnitOperators.push(["NotEqualBusinessId", Dlv.ExtFilter.FilterHandler.NotEqualBusinessId, Dlv.ExtFilter.FilterHandlerLabel.NotEqualBusinessId]);

    //mzg.ExtFilter.FilterHandler.OrganizationOperators = Dlv.ExtFilter.FilterHandler.LookUpOperators.concat();
    //mzg.ExtFilter.FilterHandler.OrganizationOperators.push(["EqualOrganizationId", Dlv.ExtFilter.FilterHandler.EqualOrganizationId, Dlv.ExtFilter.FilterHandlerLabel.EqualOrganizationId]);
    //mzg.ExtFilter.FilterHandler.OrganizationOperators.push(["NotEqualOrganizationId", Dlv.ExtFilter.FilterHandler.NotEqualOrganizationId, Dlv.ExtFilter.FilterHandlerLabel.NotEqualOrganizationId]);

    //mzg.ExtFilter.FilterHandler.PickListOperators = Dlv.ExtFilter.FilterHandler.CommonOperators.concat();
    //mzg.ExtFilter.FilterHandler.PickListOperators.push(["In", Dlv.ExtFilter.FilterHandler.In, Dlv.ExtFilter.FilterHandlerLabel.In]);
    //mzg.ExtFilter.FilterHandler.PickListOperators.push(["NotIn", Dlv.ExtFilter.FilterHandler.NotIn, Dlv.ExtFilter.FilterHandlerLabel.NotIn]);

    //mzg.ExtFilter.FilterHandlers = [];
    //mzg.ExtFilter.FilterHandlers["nvarchar"] = Dlv.ExtFilter.FilterHandler.StringOperators.concat();
    //mzg.ExtFilter.FilterHandlers["datetime"] = Dlv.ExtFilter.FilterHandler.DateTimeOperators.concat();
    //mzg.ExtFilter.FilterHandlers["lookup"] = Dlv.ExtFilter.FilterHandler.LookUpOperators.concat();
    //mzg.ExtFilter.FilterHandlers["owner"] = Dlv.ExtFilter.FilterHandler.OwnerOperators.concat();
    //mzg.ExtFilter.FilterHandlers["picklist"] = Dlv.ExtFilter.FilterHandler.PickListOperators.concat();
    //mzg.ExtFilter.FilterHandlers["bit"] = Dlv.ExtFilter.FilterHandler.PickListOperators.concat();
    //mzg.ExtFilter.FilterHandlers["int"] = Dlv.ExtFilter.FilterHandler.NumberOperators.concat();
    //mzg.ExtFilter.FilterHandlers["money"] = Dlv.ExtFilter.FilterHandler.NumberOperators.concat();
    //mzg.ExtFilter.FilterHandlers["float"] = Dlv.ExtFilter.FilterHandler.NumberOperators.concat();
    //mzg.ExtFilter.FilterHandlers["decimal"] = Dlv.ExtFilter.FilterHandler.NumberOperators.concat();
    //mzg.ExtFilter.FilterHandlers["state"] = Dlv.ExtFilter.FilterHandler.PickListOperators.concat();
    //mzg.ExtFilter.FilterHandlers["businessunit"] = Dlv.ExtFilter.FilterHandler.OwnerOperators.concat();
    //mzg.ExtFilter.FilterHandlers["systemuser"] = Dlv.ExtFilter.FilterHandler.SystemUserOperators.concat();
    //mzg.ExtFilter.FilterHandlers["organization"] = Dlv.ExtFilter.FilterHandler.OrganizationOperators.concat();
})();