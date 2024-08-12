using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace MonsterTournament.Models
{
    public class SpecialAttackLoader
    {
        public static readonly string FileExtension = ".mtsa";
        public static readonly string JsonFileName = "special.json";

        public async Task<MonsterTounamentFile> Save(SpecialAttack model)
        {
            var fileStream = new MemoryStream();

            // disposeしないとzipファイルに書き込まれない
            using (var zip = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var cardEntry = zip.CreateEntry(JsonFileName);
                using (var cardStream = cardEntry.Open())
                {
                    await JsonSerializer.SerializeAsync(cardStream, model);
                }
            }

            return new($"{model.Name}{FileExtension}", fileStream.ToArray());
        }

        public async Task<SpecialAttack?> Load(byte[] bytes)
        {
            using var archive = new ZipArchive(new MemoryStream(bytes));

            SpecialAttack? stateOrNull = null;
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.Name);

                if (entry.Name == JsonFileName)
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
