namespace MonsterTournament.Models
{
    public class SpecialAttack
    {
        public string Name { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }
}
