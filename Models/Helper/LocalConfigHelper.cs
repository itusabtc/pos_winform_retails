using System;
using System.Collections.Generic;
using System.IO;

namespace NailsChekin.Models.Helper
{
    public static class LocalConfigHelper
    {
        private static string ConfigDir
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "AntPay");
            }
        }

        private static string ConfigPath
        {
            get
            {
                return Path.Combine(ConfigDir, "localflags.ini");
            }
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(key))
                return defaultValue;

            try
            {
                var map = LoadAll();

                string raw;
                if (!map.TryGetValue(key, out raw))
                    return defaultValue;

                bool value;
                if (bool.TryParse(raw, out value))
                    return value;

                raw = (raw ?? "").Trim();
                if (raw == "1" || raw.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (raw == "0" || raw.Equals("no", StringComparison.OrdinalIgnoreCase))
                    return false;

                return defaultValue;
            }
            catch (Exception ex)
            {
                try
                {
                    LogHelper.SaveLOG_Crash(ex.ToString(), "LocalConfigHelper.GetBool ERROR");
                }
                catch { }

                return defaultValue;
            }
        }

        public static void SetBool(string key, bool value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            try
            {
                var map = LoadAll();
                map[key] = value ? "true" : "false";
                SaveAll(map);
            }
            catch (Exception ex)
            {
                try
                {
                    LogHelper.SaveLOG_Crash(ex.ToString(), "LocalConfigHelper.SetBool ERROR");
                }
                catch { }
            }
        }

        private static Dictionary<string, string> LoadAll()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (!File.Exists(ConfigPath))
                    return map;

                foreach (var line in File.ReadAllLines(ConfigPath))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string s = line.Trim();
                    if (s.StartsWith("#") || s.StartsWith(";"))
                        continue;

                    int idx = s.IndexOf('=');
                    if (idx <= 0)
                        continue;

                    string key = s.Substring(0, idx).Trim();
                    string val = s.Substring(idx + 1).Trim();

                    if (!string.IsNullOrWhiteSpace(key))
                        map[key] = val;
                }
            }
            catch
            {
            }

            return map;
        }

        private static void SaveAll(Dictionary<string, string> map)
        {
            Directory.CreateDirectory(ConfigDir);

            var lines = new List<string>();
            foreach (var kv in map)
            {
                lines.Add(kv.Key + "=" + kv.Value);
            }

            File.WriteAllLines(ConfigPath, lines.ToArray());
        }
    }
}
