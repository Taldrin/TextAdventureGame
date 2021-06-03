using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database.GameTesting
{
    public class GameTestingGameSaveData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DataName { get; set; }
        public string DataValue { get; set; }
        [ForeignKey("GameTestingGameSave")]
        public int GameTestingGameSaveId { get; set; }
    }
}
