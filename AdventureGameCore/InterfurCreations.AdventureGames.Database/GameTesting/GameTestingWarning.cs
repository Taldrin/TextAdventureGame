using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Database.GameTesting
{
    public class GameTestingWarning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string WarningMessage { get; set; }
        public int TimesOccured { get; set; }
        public string GameName { get; set; }
        public GameTestingGameSave LatestGameSave { get; set; }
    }
}
