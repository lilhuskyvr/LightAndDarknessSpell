// ﻿using System.IO;
// using System.Net.Mime;
// using Newtonsoft.Json;
// using ThunderRoad;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace LightAndDarknessSpell
// {
//     public class TimeStopMenuModule : MenuModule
//     {
//         private Button _hasZaWarudoStatusButton;
//         private Button _hasDioVoiceStatusButton;
//         public string modFolderName = "TimeStopSpellU9";
//         public string dataFilePath = "\\Data\\TimeStopSpellData.json";
//
//         // ReSharper disable once InconsistentNaming
//         public TimeStopData timeStopData;
//
//         public override void Init(MenuData menuData, Menu menu)
//         {
//             base.Init(menuData, menu);
//
//             _hasZaWarudoStatusButton = menu.GetCustomReference("HasZaWarudoStatusButton").GetComponent<Button>();
//             _hasDioVoiceStatusButton = menu.GetCustomReference("HasDioVoiceStatusButton").GetComponent<Button>();
//
//             LoadData();
//             
//             _hasZaWarudoStatusButton.GetComponentInChildren<Text>().text =
//                 timeStopData.data.hasZaWarudo ? "Enabled" : "Disabled";
//
//             _hasZaWarudoStatusButton.onClick.AddListener(() =>
//             {
//                 timeStopData.data.hasZaWarudo ^= true;
//                 _hasZaWarudoStatusButton.GetComponentInChildren<Text>().text =
//                     timeStopData.data.hasZaWarudo ? "Enabled" : "Disabled";
//                 SaveData();
//             });
//             
//             //dio's voice
//             _hasDioVoiceStatusButton.GetComponentInChildren<Text>().text =
//                 timeStopData.data.hasDioVoice ? "Enabled" : "Disabled";
//
//             _hasDioVoiceStatusButton.onClick.AddListener(() =>
//             {
//                 timeStopData.data.hasDioVoice ^= true;
//                 _hasDioVoiceStatusButton.GetComponentInChildren<Text>().text =
//                     timeStopData.data.hasDioVoice ? "Enabled" : "Disabled";
//                 SaveData();
//             });
//         }
//
//         private string GetDataFilePath()
//         {
//             return FileManager.GetFullPath(FileManager.Type.JSONCatalog, FileManager.Source.Mods,
//                 modFolderName + dataFilePath);
//         }
//
//         public void LoadData()
//         {
//             var jsonInput = File.ReadAllText(GetDataFilePath());
//
//             var savedData =
//                 JsonConvert.DeserializeObject<TimeStopJSONData>(jsonInput, Catalog.GetJsonNetSerializerSettings());
//
//             timeStopData = GameManager.local.gameObject.AddComponent<TimeStopData>();
//
//             Debug.Log("Time stop data loaded in menubook");
//             Debug.Log(timeStopData);
//             timeStopData.data = savedData;
//         }
//
//         public void SaveData()
//         {
//             var json = JsonConvert.SerializeObject(timeStopData.data, Catalog.GetJsonNetSerializerSettings());
//
//             File.WriteAllText(GetDataFilePath(), json);
//         }
//     }
// }