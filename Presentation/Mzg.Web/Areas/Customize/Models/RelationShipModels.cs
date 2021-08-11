using Mzg.Schema.Abstractions;
using Mzg.Web.Framework.Paging;
using System;

namespace Mzg.Web.Customize.Models
{
    public class RelationShipsModel : BasePaged<Schema.Domain.RelationShip>
    {
        public Guid EntityId { get; set; }
        public RelationShipType Type { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class EditRelationShipModel
    {
        public Guid RelationShipId { get; set; }
        public bool IsCustomizable { get; set; }
        public int CascadeLinkMask { get; set; }
        public int CascadeDelete { get; set; }
        public int CascadeAssign { get; set; }
        public int CascadeShare { get; set; }
        public int CascadeUnShare { get; set; }
        public Guid SolutionId { get; set; }
        public Schema.Domain.RelationShip RelationShipMeta { get; set; }
    }
}