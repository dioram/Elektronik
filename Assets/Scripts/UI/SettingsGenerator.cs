using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elektronik.Settings;
using Elektronik.UI.Fields;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class SettingsGenerator : MonoBehaviour
    {
        public GameObject StringFieldPrefab;
        public GameObject BrowseFieldPrefab;
        public GameObject BoolFieldPrefab;

        private ScrollRect _scrollView;
        private readonly List<SettingsField> _fields = new List<SettingsField>();

        public void Awake()
        {
            _scrollView = GetComponentInChildren<ScrollRect>();
        }

        public void Generate(SettingsBag settings)
        {
            _fields.ForEach(f => Destroy(f != null ? f.gameObject : null));

            settings.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => f.GetCustomAttribute<NotShowAttribute>() == null)
                    .ToList()
                    .ForEach((f => AddField(f, settings)));
        }

        private void AddField(FieldInfo fieldInfo, SettingsBag obj)
        {
            GameObject newField;
            SettingsField uiField;
            if (fieldInfo.FieldType == typeof(bool))
            {
                newField = Instantiate(BoolFieldPrefab, _scrollView.content);
                var f = newField.GetComponent<BoolSettingField>();
                f.CheckBox.isOn = (bool) fieldInfo.GetValue(obj);
                uiField = f;
            }
            else //(fieldInfo.FieldType == typeof(string))
            {    // Anything except bool is string for now 
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(PathAttribute)) != null)
                {
                    newField = Instantiate(BrowseFieldPrefab, _scrollView.content);
                }
                else
                {
                    newField = Instantiate(StringFieldPrefab, _scrollView.content);
                }
                var f = newField.GetComponent<StringSettingsField>();
                f.Field.text = fieldInfo.GetValue(obj)?.ToString() ?? "";
                uiField = f;
            }

            newField.name = $"{fieldInfo.Name}";
            uiField.SettingsBag = obj;
            var tooltip = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
            uiField.FieldToolTip = tooltip?.tooltip ?? fieldInfo.Name;
            uiField.FieldName = fieldInfo.Name;
            uiField.FieldType = fieldInfo.FieldType;
            _fields.Add(uiField);
        }
    }
}