using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item Variables Saver", menuName = "Inventory System/Items/Variables Saves", order = 50)]
public class SaveVariableNames : ScriptableObject
{
    public const string PATH = "C:/Users/Taras/Downloads/Varialbes";
    public const string PATH_RENAMED = "C:/Users/Taras/Downloads/Varialbes/Renamed";

    [SerializeField] private Item[] _itemsForSave;

    [ContextMenu("Save All Items")]
    public void SaveAll()
    {
        foreach (var item in _itemsForSave)
        {
            Save(item);
        }
    }

    public void Save(Item item)
    {
        string json = JsonUtility.ToJson(item, true);

        string savePath = Path.Combine(PATH, $"{item.name}.json");
        File.WriteAllText(savePath, json);
    }

    [ContextMenu("Rename Jsons")]
    public void RenameJson()
    {
        string[] paths = Directory.GetFiles(PATH, "*.json");

        foreach (string path in paths)
        {
            string json = File.ReadAllText(path);
            string renamedJson = RenameWords(json);
            string savePath = Path.Combine(PATH_RENAMED, $"{Path.GetFileName(path)}");
            File.WriteAllText(savePath, renamedJson);
        }
        Debug.Log(paths.Length);

    }

    [ContextMenu("Load All")]
    public void Load()
    {
        string[] paths = Directory.GetFiles(PATH_RENAMED, "*.json");

        foreach(string path in paths)
        {
            //Item item = _itemsForSave.
            
        }
    }

    private string RenameWords(string inputJson)
    {
        // Використовуємо регулярний вираз для знаходження "_ перед буквами
        const string quote = "\""; // символ лапок (")
        string pattern = quote + @"_([a-z])";
        Regex regex = new Regex(pattern);

        // Використовуємо делегат MatchEvaluator для визначення великої літери після "_
        string result = regex.Replace(inputJson, m => quote + m.Groups[1].Value.ToUpper());

        return result;
    }
}
