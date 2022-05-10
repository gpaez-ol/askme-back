using System;


namespace AskMe.Data.BaseEntities
{
    public class Entity
    {
        public Guid Id { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
