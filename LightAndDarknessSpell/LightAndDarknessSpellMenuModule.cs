using System.IO;
using Newtonsoft.Json;
using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;

namespace LightAndDarknessSpell
{
    public class LightAndDarknessSpellMenuModule : MenuModule
    {
        private Button _isLightPathStatusButton;
        public string modFolderName = "LightAndDarknessSpell";
        public string dataFilePath = "\\Data\\LightAndDarknessSpellData.json";

        // ReSharper disable once InconsistentNaming
        public LightAndDarknessSpellController lightAndDarknessSpellController;

        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);

            _isLightPathStatusButton = menu.GetCustomReference("StatusButton").GetComponent<Button>();

            LoadData();

            _isLightPathStatusButton.GetComponentInChildren<Text>().text =
                lightAndDarknessSpellController.data.isLightPath ? "Light" : "Darkness";

            _isLightPathStatusButton.onClick.AddListener(() =>
            {
                lightAndDarknessSpellController.data.isLightPath ^= true;
                _isLightPathStatusButton.GetComponentInChildren<Text>().text =
                    lightAndDarknessSpellController.data.isLightPath ? "Light" : "Darkness";
                SaveData();
            });
        }

        private string GetDataFilePath()
        {
            return FileManager.GetFullPath(FileManager.Type.JSONCatalog, FileManager.Source.Mods,
                modFolderName + dataFilePath);
        }

        public void LoadData()
        {
            var jsonInput = File.ReadAllText(GetDataFilePath());

            var savedData =
                JsonConvert.DeserializeObject<LightAndDarknessSpellData>(jsonInput,
                    Catalog.GetJsonNetSerializerSettings());

            lightAndDarknessSpellController =
                GameManager.local.gameObject.AddComponent<LightAndDarknessSpellController>();

            lightAndDarknessSpellController.data = savedData;

            lightAndDarknessSpellController.lightSpellController.angelColor = new Color(
                lightAndDarknessSpellController.data.angelBrightness,
                lightAndDarknessSpellController.data.angelBrightness,
                lightAndDarknessSpellController.data.angelBrightness,
                1
            );

            Debug.Log("Loaded Light And Darkness Spell");
        }

        public void SaveData()
        {
            var json = JsonConvert.SerializeObject(lightAndDarknessSpellController.data,
                Catalog.GetJsonNetSerializerSettings());

            File.WriteAllText(GetDataFilePath(), json);
        }
    }
}