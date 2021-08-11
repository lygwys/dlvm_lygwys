using Mzg.Core;
using PetaPoco;
using System;

namespace Mzg.MultisDc.Domain
{
    /// <summary>
    /// 第一步：设置数据表字段
    /// xmg
    /// 202007180954
    /// </summary>
    [TableName("MultistageDc")]
    [PrimaryKey("MultistagedcId", AutoIncrement = false)]
    public class MultistageDc
    {
        public Guid MultistagedcId { get; set; } = Guid.NewGuid();

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public Guid ParentMultistagedcId { get; set; }

        public string Url { get; set; }

        public string OpenTarget { get; set; }

        public int DisplayOrder { get; set; }

        public bool AuthorizationEnabled { get; set; }

        public bool IsVisibled { get; set; }

        public string Description { get; set; }

        public string SmallIcon { get; set; }

        public string BigIcon { get; set; }

        public int Level { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ResultColumn]
        [LinkEntity(typeof(MultistageDc), AliasName = "ParentMultistageDc", LinkFromFieldName = "ParentMultistagedcId", LinkToFieldName = "MultistageDcId", TargetFieldName = "DisplayName")]
        public string ParentPrivilegeName { get; set; }

    }
}
