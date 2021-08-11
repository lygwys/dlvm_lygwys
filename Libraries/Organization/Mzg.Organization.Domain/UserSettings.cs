using Mzg.Localization.Abstractions;
using System;

namespace Mzg.Organization.Domain
{
    public partial class UserSettings
    {
        public Guid DefaultDashboardId { get; set; }
        public string HomePageArea { get; set; }
        public bool EnabledNotification { get; set; }
        public LanguageCode LanguageId { get; set; } = LanguageCode.CHS;//为此枚举类型赋值
        public int PagingLimit { get; set; }
        public Guid CurrencyId { get; set; }
        public int LayoutType { get; set; }
        public int LanguageUniqueId { get; set; } = 2052;
        public Guid TransactionCurrencyId { get; set; }
    }
}