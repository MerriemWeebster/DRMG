using System.IO;
using UnityEngine;

namespace DRMG.Gameplay
{
    public static class MatchDataManager
    {
        public static MatchDataSubject MatchDataSubject { get; private set; }
        private static string SaveFilePath = Path.Combine(Application.persistentDataPath, "MatchData.json");

        public static MatchDataSubject LoadMatchData()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogWarning($"[{nameof(MatchDataManager)}] MatchData file not found!");
                MatchDataSubject = new MatchDataSubject();
            }
            else
            {
                string json = File.ReadAllText(SaveFilePath);
                MatchDataSubject = JsonUtility.FromJson<MatchDataSubject>(json);
            }
            
            return MatchDataSubject;
        }

        public static void SaveMatchData()
        {
            string json = JsonUtility.ToJson(MatchDataSubject, prettyPrint: true);
            File.WriteAllText(SaveFilePath, json);
            Debug.LogWarning($"[{nameof(MatchDataManager)}] MatchData file saved.");
        }
    }
}