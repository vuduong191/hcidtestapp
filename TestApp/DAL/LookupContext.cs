using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestApp.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace TestApp.DAL
{
    public class LookupContext : DbContext
    {

        public LookupContext() : base("hcid_lookupEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}