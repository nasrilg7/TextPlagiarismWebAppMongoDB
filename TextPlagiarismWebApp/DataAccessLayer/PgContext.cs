using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Common;
using System.Linq;
using System.Web;
using TextPlagiarismWebApp.Models;
using Npgsql;

namespace TextPlagiarismWebApp.DataAccessLayer
{
    public class PgContext :DbContext
    {
        public PgContext(DbConnection connection, bool ownsConnection)
            : base(connection, ownsConnection)
        {

        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Sentence> Sentences { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(("public"));
            base.OnModelCreating(modelBuilder);
        }

        
    }
}
