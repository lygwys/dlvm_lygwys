if (typeof (Dlv) == "undefined") { Dlv = { __namespace: true }; }
Dlv.Fetch = function () { };
//mzg.Fetch.LogicalOperator = function () { };
Dlv.Fetch.LogicalOperator = {
    And: 0,
    Or: 1
};
//mzg.Fetch.ConditionOperator = function () { };
Dlv.Fetch.ConditionOperator = {
    Equal: 0,
    NotEqual: 1,
    GreaterThan: 2,
    LessThan: 3,
    GreaterEqual: 4,
    LessEqual: 5,
    Like: 6,
    NotLike: 7,
    In: 8,
    NotIn: 9,
    Between: 10,
    NotBetween: 11,
    Null: 12,
    NotNull: 13,
    Yesterday: 14,
    Today: 15,
    Tomorrow: 16,
    Last7Days: 17,
    Next7Days: 18,
    LastWeek: 19,
    ThisWeek: 20,
    NextWeek: 21,
    LastMonth: 22,
    ThisMonth: 23,
    NextMonth: 24,
    On: 25,
    OnOrBefore: 26,
    OnOrAfter: 27,
    Before: 28,
    After: 29,
    LastYear: 30,
    ThisYear: 31,
    NextYear: 32,
    LastXHours: 33,
    NextXHours: 34,
    LastXDays: 35,
    NextXDays: 36,
    LastXWeeks: 37,
    NextXWeeks: 38,
    LastXMonths: 39,
    NextXMonths: 40,
    LastXYears: 41,
    NextXYears: 42,
    EqualUserId: 43,
    NotEqualUserId: 44,
    EqualBusinessId: 45,
    NotEqualBusinessId: 46,
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
Dlv.Fetch.ConditionOperatorLabel = {
    Equal: '等于',
    NotEqual: '不等于',
    GreaterThan: '大于',
    LessThan: '小于',
    GreaterEqual: '大于等于',
    LessEqual: '小于等于',
    Like: '包含',
    NotLike: '不包含',
    In: '包含',
    NotIn: '不包含',
    Between: '介于',
    NotBetween: '不介于',
    Null: '不包含数据',
    NotNull: '包含数据',
    Yesterday: '昨天',
    Today: '今天',
    Tomorrow: '明天',
    Last7Days: '往前7天及以后提示',
    Next7Days: '往后7天',
    LastWeek: '上周',
    ThisWeek: '本周',
    NextWeek: '下周',
    LastMonth: '上个月',
    ThisMonth: '本月',
    NextMonth: '下个月',
    On: '等于',
    OnOrBefore: '早于(包含当天)',
    OnOrAfter: '晚于(包含当天)',
    Before: '早于',
    After: '晚于',
    LastYear: '去年',
    ThisYear: '今年',
    NextYear: '明年',
    LastXHours: '往前X小时',
    NextXHours: '往后X小时',
    LastXDays: '往前X天及以后不提示',
    NextXDays: '往后X天',
    LastXWeeks: '往前X周',
    NextXWeeks: '往后X周',
    LastXMonths: '往前X月',
    NextXMonths: '往后X月',
    LastXYears: '往前X年',
    NextXYears: '往后X年',
    EqualUserId: '等于当前用户',
    NotEqualUserId: '不等于当前用户',
    EqualBusinessId: '等于当前部门',
    NotEqualBusinessId: '不等于当前部门',
    ChildOf: '下属',
    Mask: '',
    NotMask: '',
    MasksSelect: '',
    Contains: '包含',
    DoesNotContain: '不包含',
    EqualUserLanguage: '等于当前用户语言',
    NotOn: '不等于',
    OlderThanXMonths: 'X个月以前',
    BeginsWith: '开头等于',
    DoesNotBeginWith: '开头不等于',
    EndsWith: '结尾等于',
    DoesNotEndWith: '结尾不等于',
    ThisFiscalYear: '当前会计年度',
    ThisFiscalPeriod: '当前会计期间',
    NextFiscalYear: '下一会计年度',
    NextFiscalPeriod: '下一会计期间',
    LastFiscalYear: '上一会计年度',
    LastFiscalPeriod: '上一会计期间',
    LastXFiscalYears: '过去X个会计年度',
    LastXFiscalPeriods: '过去X个会计期间',
    NextXFiscalYears: '往后X个会计年度',
    NextXFiscalPeriods: '往后X个会计期间',
    InFiscalYear: '在会计年度内',
    InFiscalPeriod: '在会计期间内',
    InFiscalPeriodAndYear: '在会计期间及年度内',
    InOrBeforeFiscalPeriodAndYear: '在会计期间及年度之前',
    InOrAfterFiscalPeriodAndYear: '在会计期间及年度之后',
    EqualUserTeams: '当前用户团队',
    EqualOrganizationId: '等于当前组织',
    NotEqualOrganizationId: '不等于当前组织',
    OnOrBeforeToday: '今天之前(包含当天)',
    OnOrAfterToday: '今天之后(包含当天)',
    BeforeToday: '今天之前',
    AfterToday: '今天之后',
    OlderThanXYears: 'X年以前',
    OlderThanXDays: 'X天以前',
    AfterXYears: 'X年以后',
    AfterXMonths: 'X个月以后',
    AfterXDays: 'X天以后'
};
//models
Dlv.Fetch.FilterExpression = function (operator) {
    var self = new Object();
    self.FilterOperator = operator || Dlv.Fetch.LogicalOperator.And;
    self.Conditions = [];
    self.Filters = [];
    return self;
};
Dlv.Fetch.ConditionExpression = function () {
    var self = new Object();
    self.AttributeName = '';
    self.Operator = Dlv.Fetch.ConditionOperator.Equal;
    self.Values = [];
    self.EnityId = '';
    self.YinruEntityId = '';
    return self;
};

//类型比较符
Dlv.Fetch.ConditionOperator.CommonOperators = [];
Dlv.Fetch.ConditionOperator.CommonOperators.push(["Equal", Dlv.Fetch.ConditionOperator.Equal, Dlv.Fetch.ConditionOperatorLabel.Equal]);
Dlv.Fetch.ConditionOperator.CommonOperators.push(["NotEqual", Dlv.Fetch.ConditionOperator.NotEqual, Dlv.Fetch.ConditionOperatorLabel.NotEqual]);
Dlv.Fetch.ConditionOperator.CommonOperators.push(["NotNull", Dlv.Fetch.ConditionOperator.NotNull, Dlv.Fetch.ConditionOperatorLabel.NotNull]);
Dlv.Fetch.ConditionOperator.CommonOperators.push(["Null", Dlv.Fetch.ConditionOperator.Null, Dlv.Fetch.ConditionOperatorLabel.Null]);

Dlv.Fetch.ConditionOperator.StringOperators = Dlv.Fetch.ConditionOperator.CommonOperators.concat();
Dlv.Fetch.ConditionOperator.StringOperators.push(["Like", Dlv.Fetch.ConditionOperator.Like, Dlv.Fetch.ConditionOperatorLabel.Like]);
Dlv.Fetch.ConditionOperator.StringOperators.push(["NotLike", Dlv.Fetch.ConditionOperator.NotLike, Dlv.Fetch.ConditionOperatorLabel.NotLike]);
Dlv.Fetch.ConditionOperator.StringOperators.push(["BeginsWith", Dlv.Fetch.ConditionOperator.BeginsWith, Dlv.Fetch.ConditionOperatorLabel.BeginsWith]);
Dlv.Fetch.ConditionOperator.StringOperators.push(["DoesNotBeginWith", Dlv.Fetch.ConditionOperator.DoesNotBeginWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
Dlv.Fetch.ConditionOperator.StringOperators.push(["EndsWith", Dlv.Fetch.ConditionOperator.EndsWith, Dlv.Fetch.ConditionOperatorLabel.EndsWith]);
Dlv.Fetch.ConditionOperator.StringOperators.push(["DoesNotEndWith", Dlv.Fetch.ConditionOperator.DoesNotEndWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotEndWith]);

Dlv.Fetch.ConditionOperator.NumberOperators = Dlv.Fetch.ConditionOperator.CommonOperators.concat();
Dlv.Fetch.ConditionOperator.NumberOperators.push(["GreaterThan", Dlv.Fetch.ConditionOperator.GreaterThan, Dlv.Fetch.ConditionOperatorLabel.GreaterThan]);
Dlv.Fetch.ConditionOperator.NumberOperators.push(["LessThan", Dlv.Fetch.ConditionOperator.LessThan, Dlv.Fetch.ConditionOperatorLabel.LessThan]);
Dlv.Fetch.ConditionOperator.NumberOperators.push(["GreaterEqual", Dlv.Fetch.ConditionOperator.GreaterEqual, Dlv.Fetch.ConditionOperatorLabel.GreaterEqual]);
Dlv.Fetch.ConditionOperator.NumberOperators.push(["LessEqual", Dlv.Fetch.ConditionOperator.LessEqual, Dlv.Fetch.ConditionOperatorLabel.LessEqual]);

Dlv.Fetch.ConditionOperator.DateTimeOperators = Dlv.Fetch.ConditionOperator.CommonOperators.concat();
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["Last7Days", Dlv.Fetch.ConditionOperator.Last7Days, Dlv.Fetch.ConditionOperatorLabel.Last7Days]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastWeek", Dlv.Fetch.ConditionOperator.LastWeek, Dlv.Fetch.ConditionOperatorLabel.LastWeek]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastMonth", Dlv.Fetch.ConditionOperator.LastMonth, Dlv.Fetch.ConditionOperatorLabel.LastMonth]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastYear", Dlv.Fetch.ConditionOperator.LastYear, Dlv.Fetch.ConditionOperatorLabel.LastYear]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastXHours", Dlv.Fetch.ConditionOperator.LastXHours, Dlv.Fetch.ConditionOperatorLabel.LastXHours]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastXDays", Dlv.Fetch.ConditionOperator.LastXDays, Dlv.Fetch.ConditionOperatorLabel.LastXDays]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastXWeeks", Dlv.Fetch.ConditionOperator.LastXWeeks, Dlv.Fetch.ConditionOperatorLabel.LastXWeeks]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastXMonths", Dlv.Fetch.ConditionOperator.LastXMonths, Dlv.Fetch.ConditionOperatorLabel.LastXMonths]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["LastXYears", Dlv.Fetch.ConditionOperator.LastXYears, Dlv.Fetch.ConditionOperatorLabel.LastXYears]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["Next7Days", Dlv.Fetch.ConditionOperator.Next7Days, Dlv.Fetch.ConditionOperatorLabel.Next7Days]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextWeek", Dlv.Fetch.ConditionOperator.NextWeek, Dlv.Fetch.ConditionOperatorLabel.NextWeek]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextMonth", Dlv.Fetch.ConditionOperator.NextMonth, Dlv.Fetch.ConditionOperatorLabel.NextMonth]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextYear", Dlv.Fetch.ConditionOperator.NextYear, Dlv.Fetch.ConditionOperatorLabel.NextYear]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextXHours", Dlv.Fetch.ConditionOperator.NextXHours, Dlv.Fetch.ConditionOperatorLabel.NextXHours]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextXDays", Dlv.Fetch.ConditionOperator.NextXDays, Dlv.Fetch.ConditionOperatorLabel.NextXDays]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextXWeeks", Dlv.Fetch.ConditionOperator.NextXWeeks, Dlv.Fetch.ConditionOperatorLabel.NextXWeeks]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextXMonths", Dlv.Fetch.ConditionOperator.NextXMonths, Dlv.Fetch.ConditionOperatorLabel.NextXMonths]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["NextXYears", Dlv.Fetch.ConditionOperator.NextXYears, Dlv.Fetch.ConditionOperatorLabel.NextXYears]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OlderThanXYears", Dlv.Fetch.ConditionOperator.OlderThanXYears, Dlv.Fetch.ConditionOperatorLabel.OlderThanXYears]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OlderThanXMonths", Dlv.Fetch.ConditionOperator.OlderThanXMonths, Dlv.Fetch.ConditionOperatorLabel.OlderThanXMonths]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OlderThanXDays", Dlv.Fetch.ConditionOperator.OlderThanXDays, Dlv.Fetch.ConditionOperatorLabel.OlderThanXDays]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["AfterXYears", Dlv.Fetch.ConditionOperator.AfterXYears, Dlv.Fetch.ConditionOperatorLabel.AfterXYears]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["AfterXMonths", Dlv.Fetch.ConditionOperator.AfterXMonths, Dlv.Fetch.ConditionOperatorLabel.AfterXMonths]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["AfterXDays", Dlv.Fetch.ConditionOperator.AfterXDays, Dlv.Fetch.ConditionOperatorLabel.AfterXDays]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrBefore", Dlv.Fetch.ConditionOperator.OnOrBefore, Dlv.Fetch.ConditionOperatorLabel.OnOrBefore]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrAfter", Dlv.Fetch.ConditionOperator.OnOrAfter, Dlv.Fetch.ConditionOperatorLabel.OnOrAfter]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["Before", Dlv.Fetch.ConditionOperator.Before, Dlv.Fetch.ConditionOperatorLabel.Before]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["After", Dlv.Fetch.ConditionOperator.After, Dlv.Fetch.ConditionOperatorLabel.After]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["ThisWeek", Dlv.Fetch.ConditionOperator.ThisWeek, Dlv.Fetch.ConditionOperatorLabel.ThisWeek]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["ThisMonth", Dlv.Fetch.ConditionOperator.ThisMonth, Dlv.Fetch.ConditionOperatorLabel.ThisMonth]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["ThisYear", Dlv.Fetch.ConditionOperator.ThisYear, Dlv.Fetch.ConditionOperatorLabel.ThisYear]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["Today", Dlv.Fetch.ConditionOperator.Today, Dlv.Fetch.ConditionOperatorLabel.Today]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["Tomorrow", Dlv.Fetch.ConditionOperator.Tomorrow, Dlv.Fetch.ConditionOperatorLabel.Tomorrow]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["Yesterday", Dlv.Fetch.ConditionOperator.Yesterday, Dlv.Fetch.ConditionOperatorLabel.Yesterday]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrBeforeToday", Dlv.Fetch.ConditionOperator.OnOrBeforeToday, Dlv.Fetch.ConditionOperatorLabel.OnOrBeforeToday]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["OnOrAfterToday", Dlv.Fetch.ConditionOperator.OnOrAfterToday, Dlv.Fetch.ConditionOperatorLabel.OnOrAfterToday]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["BeforeToday", Dlv.Fetch.ConditionOperator.BeforeToday, Dlv.Fetch.ConditionOperatorLabel.BeforeToday]);
Dlv.Fetch.ConditionOperator.DateTimeOperators.push(["AfterToday", Dlv.Fetch.ConditionOperator.AfterToday, Dlv.Fetch.ConditionOperatorLabel.AfterToday]);

Dlv.Fetch.ConditionOperator.LookUpOperators = Dlv.Fetch.ConditionOperator.CommonOperators.concat();
Dlv.Fetch.ConditionOperator.LookUpOperators.push(["Like", Dlv.Fetch.ConditionOperator.Like, Dlv.Fetch.ConditionOperatorLabel.Like]);
Dlv.Fetch.ConditionOperator.LookUpOperators.push(["NotLike", Dlv.Fetch.ConditionOperator.NotLike, Dlv.Fetch.ConditionOperatorLabel.NotLike]);
Dlv.Fetch.ConditionOperator.LookUpOperators.push(["BeginsWith", Dlv.Fetch.ConditionOperator.BeginsWith, Dlv.Fetch.ConditionOperatorLabel.BeginsWith]);
Dlv.Fetch.ConditionOperator.LookUpOperators.push(["DoesNotBeginWith", Dlv.Fetch.ConditionOperator.DoesNotBeginWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
Dlv.Fetch.ConditionOperator.LookUpOperators.push(["EndsWith", Dlv.Fetch.ConditionOperator.EndsWith, Dlv.Fetch.ConditionOperatorLabel.EndsWith]);
Dlv.Fetch.ConditionOperator.LookUpOperators.push(["DoesNotEndWith", Dlv.Fetch.ConditionOperator.DoesNotEndWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotEndWith]);

Dlv.Fetch.ConditionOperator.OwnerOperators = Dlv.Fetch.ConditionOperator.CommonOperators.concat();
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["Like", Dlv.Fetch.ConditionOperator.Like, Dlv.Fetch.ConditionOperatorLabel.Like]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["NotLike", Dlv.Fetch.ConditionOperator.NotLike, Dlv.Fetch.ConditionOperatorLabel.NotLike]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["BeginsWith", Dlv.Fetch.ConditionOperator.BeginsWith, Dlv.Fetch.ConditionOperatorLabel.BeginsWith]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["DoesNotBeginWith", Dlv.Fetch.ConditionOperator.DoesNotBeginWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["EndsWith", Dlv.Fetch.ConditionOperator.EndsWith, Dlv.Fetch.ConditionOperatorLabel.EndsWith]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["DoesNotEndWith", Dlv.Fetch.ConditionOperator.DoesNotEndWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotEndWith]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["EqualUserId", Dlv.Fetch.ConditionOperator.EqualUserId, Dlv.Fetch.ConditionOperatorLabel.EqualUserId]);
Dlv.Fetch.ConditionOperator.OwnerOperators.push(["NotEqualUserId", Dlv.Fetch.ConditionOperator.NotEqualUserId, Dlv.Fetch.ConditionOperatorLabel.NotEqualUserId]);

Dlv.Fetch.ConditionOperator.SystemUserOperators = Dlv.Fetch.ConditionOperator.OwnerOperators.concat();

Dlv.Fetch.ConditionOperator.BusinessUnitOperators = Dlv.Fetch.ConditionOperator.LookUpOperators.concat();
//mzg.Fetch.ConditionOperator.BusinessUnitOperators.push(["In", Dlv.Fetch.ConditionOperator.In, Dlv.Fetch.ConditionOperatorLabel.In]);
//mzg.Fetch.ConditionOperator.BusinessUnitOperators.push(["NotIn", Dlv.Fetch.ConditionOperator.NotIn, Dlv.Fetch.ConditionOperatorLabel.NotIn]);
//mzg.Fetch.ConditionOperator.BusinessUnitOperators.push(["BeginsWith", Dlv.Fetch.ConditionOperator.BeginsWith, Dlv.Fetch.ConditionOperatorLabel.BeginsWith]);
//mzg.Fetch.ConditionOperator.BusinessUnitOperators.push(["DoesNotBeginWith", Dlv.Fetch.ConditionOperator.DoesNotBeginWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotBeginWith]);
//mzg.Fetch.ConditionOperator.BusinessUnitOperators.push(["EndsWith", Dlv.Fetch.ConditionOperator.EndsWith, Dlv.Fetch.ConditionOperatorLabel.EndsWith]);
//mzg.Fetch.ConditionOperator.BusinessUnitOperators.push(["DoesNotEndWith", Dlv.Fetch.ConditionOperator.DoesNotEndWith, Dlv.Fetch.ConditionOperatorLabel.DoesNotEndWith]);
Dlv.Fetch.ConditionOperator.BusinessUnitOperators.push(["EqualBusinessId", Dlv.Fetch.ConditionOperator.EqualBusinessId, Dlv.Fetch.ConditionOperatorLabel.EqualBusinessId]);
Dlv.Fetch.ConditionOperator.BusinessUnitOperators.push(["NotEqualBusinessId", Dlv.Fetch.ConditionOperator.NotEqualBusinessId, Dlv.Fetch.ConditionOperatorLabel.NotEqualBusinessId]);

Dlv.Fetch.ConditionOperator.OrganizationOperators = Dlv.Fetch.ConditionOperator.LookUpOperators.concat();
Dlv.Fetch.ConditionOperator.OrganizationOperators.push(["EqualOrganizationId", Dlv.Fetch.ConditionOperator.EqualOrganizationId, Dlv.Fetch.ConditionOperatorLabel.EqualOrganizationId]);
Dlv.Fetch.ConditionOperator.OrganizationOperators.push(["NotEqualOrganizationId", Dlv.Fetch.ConditionOperator.NotEqualOrganizationId, Dlv.Fetch.ConditionOperatorLabel.NotEqualOrganizationId]);

Dlv.Fetch.ConditionOperator.PickListOperators = Dlv.Fetch.ConditionOperator.CommonOperators.concat();
Dlv.Fetch.ConditionOperator.PickListOperators.push(["In", Dlv.Fetch.ConditionOperator.In, Dlv.Fetch.ConditionOperatorLabel.In]);
Dlv.Fetch.ConditionOperator.PickListOperators.push(["NotIn", Dlv.Fetch.ConditionOperator.NotIn, Dlv.Fetch.ConditionOperatorLabel.NotIn]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["GreaterThan", Dlv.Fetch.ConditionOperator.GreaterThan, "大于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["LessThan", Dlv.Fetch.ConditionOperator.LessThan, "小于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["GreaterEqual", Dlv.Fetch.ConditionOperator.GreaterEqual, "大于等于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["LessEqual", Dlv.Fetch.ConditionOperator.LessEqual, "小于等于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["BeginsWith", Dlv.Fetch.ConditionOperator.BeginsWith, "开头等于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["DoesNotBeginWith", Dlv.Fetch.ConditionOperator.DoesNotBeginWith, "开头不等于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["EndsWith", Dlv.Fetch.ConditionOperator.EndsWith, "结尾等于"]);
//mzg.Fetch.ConditionOperator.PickListOperators.push(["DoesNotEndWith", Dlv.Fetch.ConditionOperator.DoesNotEndWith, "结尾不等于"]);

Dlv.Fetch.ConditionOperators = [];
Dlv.Fetch.ConditionOperators["nvarchar"] = Dlv.Fetch.ConditionOperator.StringOperators.concat();
Dlv.Fetch.ConditionOperators["datetime"] = Dlv.Fetch.ConditionOperator.DateTimeOperators.concat();
Dlv.Fetch.ConditionOperators["lookup"] = Dlv.Fetch.ConditionOperator.LookUpOperators.concat();
Dlv.Fetch.ConditionOperators["owner"] = Dlv.Fetch.ConditionOperator.OwnerOperators.concat();
Dlv.Fetch.ConditionOperators["picklist"] = Dlv.Fetch.ConditionOperator.PickListOperators.concat();
Dlv.Fetch.ConditionOperators["bit"] = Dlv.Fetch.ConditionOperator.PickListOperators.concat();
Dlv.Fetch.ConditionOperators["int"] = Dlv.Fetch.ConditionOperator.NumberOperators.concat();
Dlv.Fetch.ConditionOperators["money"] = Dlv.Fetch.ConditionOperator.NumberOperators.concat();
Dlv.Fetch.ConditionOperators["float"] = Dlv.Fetch.ConditionOperator.NumberOperators.concat();
Dlv.Fetch.ConditionOperators["decimal"] = Dlv.Fetch.ConditionOperator.NumberOperators.concat();
Dlv.Fetch.ConditionOperators["state"] = Dlv.Fetch.ConditionOperator.PickListOperators.concat();
Dlv.Fetch.ConditionOperators["businessunit"] = Dlv.Fetch.ConditionOperator.OwnerOperators.concat();
Dlv.Fetch.ConditionOperators["systemuser"] = Dlv.Fetch.ConditionOperator.SystemUserOperators.concat();
Dlv.Fetch.ConditionOperators["organization"] = Dlv.Fetch.ConditionOperator.OrganizationOperators.concat();

//根据数字获取操作符名字
Dlv.Fetch.getFilterName = function (number) {
    var res = '';
    for (var i in Dlv.Fetch.ConditionOperator) {
        if (Dlv.Fetch.ConditionOperator.hasOwnProperty(i)) {
            if (number == Dlv.Fetch.ConditionOperator[i]) {
                res = i;
                break;
            }
        }
    }
    return res;
}