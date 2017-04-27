using Blog.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public bool IsAuthor(string name)
        {
            return this.Author.UserName.Equals(name);
        }

        [Required]
        public CategoryType Category { get; set; }      
        
        public int RatesSum { get; set; }
        
        public int RatesNum { get; set; }

        public decimal AverageRating { get; set; }

        [ImageUrl]        
        [Display(Name ="Image URL")]
        public string ImageURL { get; set; }

        public DateTime DateAdded { get; set; }
    }
}