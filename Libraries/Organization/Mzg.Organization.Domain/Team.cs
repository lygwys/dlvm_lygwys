using PetaPoco;
using System;

namespace Mzg.Organization.Domain
{
    [TableName("Team")]
    [PrimaryKey("TeamId", AutoIncrement = false)]
    public class Team
    {
        public Guid TeamId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}