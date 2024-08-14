namespace MonsterTournament.Models
{
    public class BattleCard
    {
        public Monster Main { get; set; } = new();

        public List<Monster> Transforms { get; set; } = [];

        public List<BattleState> BattleStates { get; set; } = [];

        public List<SpecialAttack> SpecialAttacks { get; set; } = [];
    }
}
