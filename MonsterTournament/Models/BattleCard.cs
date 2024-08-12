namespace MonsterTournament.Models
{
    public class BattleCard
    {
        public string Name { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public int Hp { get; set; }
        public string ImageFileName { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }
}
