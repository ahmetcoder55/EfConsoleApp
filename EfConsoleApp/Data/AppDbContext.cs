using EfConsoleApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EfConsoleApp.Data
{
    public class AppDbContext:DbContext
    {
       
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=EfConsoleAppContext;Trusted_Connection=true;TrustServerCertificate=true;")
                ;
        }
    }
}
