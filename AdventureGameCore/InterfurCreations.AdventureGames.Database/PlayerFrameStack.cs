using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InterfurCreations.AdventureGames.Database
{
    public class PlayerFrameStack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ReturnStateId { get; set; }
        [Required]
        public PlayerGameSave Save { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
