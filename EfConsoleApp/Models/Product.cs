using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EfConsoleApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descripton { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
