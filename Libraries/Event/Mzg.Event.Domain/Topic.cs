﻿using PetaPoco;
using System;

namespace Mzg.Event.Domain
{
    [TableName("Topic")]
    [PrimaryKey("TopicId", AutoIncrement = false)]
    public class Topic
    {
        public Guid TopicId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}