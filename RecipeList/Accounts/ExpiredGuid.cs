using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeList.Accounts
{
    [Table("expired_guids")]
    public class ExpiredGuid
    {
        [Column("id")] [Key]
        public int Id { get; set; }
        [Column("expired_guid")]
        public Guid ExpiredGuId { get; set; }
    }
}