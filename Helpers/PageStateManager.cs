using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace DevTools.Helpers
{
    public class PageStateManager
    {
        private static readonly string StateFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DevTools",
            "page_states.json");

        private static Dictionary<string, Dictionary<string, string>> _pageStates = new();

        static PageStateManager()
        {
            LoadStates();
        }

        private static void LoadStates()
        {
            try
            {
                if (File.Exists(StateFilePath))
                {
                    var json = File.ReadAllText(StateFilePath);
                    _pageStates = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? new Dictionary<string, Dictionary<string, string>>();
                }
            }
            catch
            {
                _pageStates = new Dictionary<string, Dictionary<string, string>>();
            }
        }

        private static void SaveStates()
        {
            try
            {
                var directory = Path.GetDirectoryName(StateFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(_pageStates, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StateFilePath, json);
            }
            catch
            {
            }
        }

        public static void SavePageState(Page page, Dictionary<string, string> state)
        {
            var pageName = page.GetType().Name;
            _pageStates[pageName] = state;
            SaveStates();
        }

        public static Dictionary<string, string>? GetPageState(Page page)
        {
            var pageName = page.GetType().Name;
            return _pageStates.TryGetValue(pageName, out var state) ? state : null;
        }

        public static void ClearPageState(Page page)
        {
            var pageName = page.GetType().Name;
            _pageStates.Remove(pageName);
            SaveStates();
        }

        public static void ClearAllStates()
        {
            _pageStates.Clear();
            SaveStates();
        }
    }
}
