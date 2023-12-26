﻿namespace Thss0.Web.Models.Entities
{
    public class Substance : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public virtual HashSet<Procedure> Procedure { get; set; } = new();
    }
}