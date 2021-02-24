using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elektronik.Common.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class UISettingsGenerator : MonoBehaviour
    {
        public GameObject StringFieldPrefab;
        public GameObject BrowseFieldPrefab;
        
        private ScrollRect _scrollView;
        private readonly List<GameObject> _fields = new List<GameObject>();
        private SettingsBag _current;

        public void Awake()
        {
            _scrollView = GetComponentInChildren<ScrollRect>();
        }

        public void Generate(SettingsBag settings)
        {
            _fields.ForEach(Destroy);
            
            settings.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => Attribute.IsDefined(f, typeof(TooltipAttribute)))
                    .ToList()
                    .ForEach((f => AddField(f, settings)));
        }

        private void AddField(FieldInfo fieldInfo, SettingsBag obj)
        {
            GameObject newField;
            if (fieldInfo.FieldType == typeof(string) 
                && Attribute.GetCustomAttribute(fieldInfo, typeof(PathAttribute)) != null)
            {
                newField = Instantiate(BrowseFieldPrefab, _scrollView.content);
            }
            else
            {
                newField = Instantiate(StringFieldPrefab, _scrollView.content);
            }

            newField.name = $"{fieldInfo.Name}";
            var uiField = newField.GetComponent<UISettingsField>();
            uiField.SettingsBag = obj;
            uiField.FieldToolTip =
                    ((TooltipAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute))).tooltip;
            uiField.FieldName = fieldInfo.Name;
            uiField.FieldType = fieldInfo.FieldType;
            uiField.FieldText = fieldInfo.GetValue(obj)?.ToString() ?? "";
            _fields.Add(newField);
        }
    }
}