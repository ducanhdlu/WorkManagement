﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkManagement.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QLNghiPhepEntities : DbContext
    {
        public QLNghiPhepEntities()
            : base("name=QLNghiPhepEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AbsenceLetter> AbsenceLetters { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<BonusDayOff> BonusDayOffs { get; set; }
        public virtual DbSet<DefaultValue> DefaultValues { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<GoOutLetter> GoOutLetters { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Reason> Reasons { get; set; }
        public virtual DbSet<Timekeeping> Timekeepings { get; set; }
    }
}
