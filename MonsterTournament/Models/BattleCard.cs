namespace MonsterTournament.Models
{
    public class BattleCard
    {
        public string Name { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public int Hp { get; set; }
        public string ImageFileName { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];

        public List<BattleCard> Transforms { get; set; } = [];

        public List<BattleState> BattleStates { get; set; } = [];

        public List<SpecialAttack> SpecialAttacks { get; set; } = [];
    }
}
