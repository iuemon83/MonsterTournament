﻿using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace MonsterTournament.Client.Models
{
    public static class Config
    {
        public static readonly (int X, int Y) ImageSize = (250, 250);
    }

    public class BattleCard
    {
        public string Name { get; set; } = "";
        public string FlavorText { get; set; } = "";
        public int Hp { get; set; }
        public string ImageFileName { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }

    public enum BattleCardAttackType
    {
        Other,
        Miss,
        Attack,
        Heal,
        Bouhatu,
        ReAttack,
        BattleState,
        Special,
        Buff,
        Transform,
    }

    public static class BattleCardAttackTypeExtensions
    {
        public static string ToDisplayString(this BattleCardAttackType type)
        {
            return type switch
            {
                BattleCardAttackType.Other => "その他",
                BattleCardAttackType.Miss => "ミス",
                BattleCardAttackType.Attack => "攻撃",
                BattleCardAttackType.Heal => "回復",
                BattleCardAttackType.Bouhatu => "暴発",
                BattleCardAttackType.ReAttack => "再攻撃",
                BattleCardAttackType.BattleState => "状態異常",
                BattleCardAttackType.Special => "必殺技",
                BattleCardAttackType.Buff => "バフ",
                BattleCardAttackType.Transform => "変身",
                _ => "エラー",
            };
        }
    }

    public class BattleCardAttack
    {
        public BattleCardAttackType Type { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public int IntValue { get; set; } = 0;
    }

    public class BattleState
    {
        public string Name { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }

    public class SpecialAttack
    {
        public string Name { get; set; } = "";
        public BattleCardAttack[] Attacks { get; set; } = [];
    }

    public class BattleCardLoader
    {
        public static readonly string jsonFileName = "card.json";

        public async Task<byte[]> Save(BattleCard battleCard, byte[] imageBytes)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(jsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, battleCard);
                }

                if (imageBytes.Any())
                {
                    var entry = zip.CreateEntry(battleCard.ImageFileName);
                    using var stream = entry.Open();
                    await stream.WriteAsync(imageBytes);
                }
            }

            return fileStream.ToArray();
        }

        public async Task<(BattleCard?, byte[] image)> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            BattleCard? cardOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == jsonFileName)
                {
                    using var stream = entry.Open();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(json);

                    cardOrNull = JsonSerializer.Deserialize<BattleCard>(json);
                    break;
                }
            }

            if (cardOrNull == null)
            {
                //TODO : カードの読み込みに失敗 error handling
                Console.WriteLine("カードの読み込みに失敗");
                return default;
            }

            var imageMs = new MemoryStream();
            foreach (var entry in archive.Entries)
            {
                if (entry.Name == cardOrNull.ImageFileName)
                {
                    Console.WriteLine("image found");

                    using var stream = entry.Open();
                    await stream.CopyToAsync(imageMs);

                    break;
                }
            }

            return (cardOrNull, imageMs.ToArray());
        }
    }

    public class BattleStateLoader
    {
        public static readonly string jsonFileName = "state.json";

        public async Task<byte[]> Save(BattleState battleState)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(jsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, battleState);
                }
            }

            return fileStream.ToArray();
        }

        public async Task<BattleState?> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            BattleState? stateOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == jsonFileName)
                {
                    using var stream = entry.Open();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(json);

                    stateOrNull = JsonSerializer.Deserialize<BattleState>(json);
                    break;
                }
            }

            if (stateOrNull == null)
            {
                //TODO : カードの読み込みに失敗 error handling
                Console.WriteLine("状態の読み込みに失敗");
                return default;
            }

            return stateOrNull;
        }
    }

    public class SpecialAttackLoader
    {
        public static readonly string jsonFileName = "special.json";

        public async Task<byte[]> Save(SpecialAttack model)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(jsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, model);
                }
            }

            return fileStream.ToArray();
        }

        public async Task<SpecialAttack?> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            SpecialAttack? stateOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == jsonFileName)
                {
                    using var stream = entry.Open();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine(json);

                    stateOrNull = JsonSerializer.Deserialize<SpecialAttack>(json);
                    break;
                }
            }

            if (stateOrNull == null)
            {
                //TODO : カードの読み込みに失敗 error handling
                Console.WriteLine("状態の読み込みに失敗");
                return default;
            }

            return stateOrNull;
        }
    }
}
