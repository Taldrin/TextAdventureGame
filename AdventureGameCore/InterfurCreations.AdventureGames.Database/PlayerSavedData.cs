using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InterfurCreations.AdventureGames.Database
{
    public class PlayerSavedData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Player")]
        public string PlayerId { get; set; }
        public string DataName { get; set; }
        public string DataValue { get; set; }
        public string DataType { get; set; }

        public Player Player { get; set; }
    }
}
