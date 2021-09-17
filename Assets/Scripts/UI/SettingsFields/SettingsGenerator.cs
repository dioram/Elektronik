using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using Humanizer;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SettingsFields
{
    public class SettingsGenerator : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private GameObject StringFieldPrefab;
        [SerializeField] private GameObject PathFieldPrefab;
        [SerializeField] private GameObject BoolFieldPrefab;
        [SerializeField] private GameObject IntegerFieldPrefab;
        [SerializeField] private GameObject FloatFieldPrefab;
        [SerializeField] private GameObject Vector3FieldPrefab;
        [SerializeField] private GameObject RangedIntegerFieldPrefab;
        [SerializeField] private GameObject RangedFloatFieldPrefab;
        [SerializeField] private GameObject SettingsButtonPrefab;
        [SerializeField] private Transform Target;

        #endregion
        
        [CanBeNull] public SettingsBag Settings { get; private set; }

        public void Generate(SettingsBag settings)
        {
            Settings = settings;
            Clear();
            var fields = settings.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => !HasAttribute<HideAttribute>(f));
            foreach (var field in fields)
            {
                try
                {
                    AddField(field, settings);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Field of that type can not be rendered. Just log exception and go to next.
                    Debug.LogException(e);
                }
            }
        }

        public void Clear()
        {
            foreach (var field in _fields.Where(f => f != null))
            {
                Destroy(field.gameObject);
            }
        }

        #region Private
        
        private readonly List<SettingsFieldBase> _fields = new List<SettingsFieldBase>();

        private void AddField(FieldInfo fieldInfo, SettingsBag obj)
        {
            SettingsFieldBase uiField;

            if (fieldInfo.FieldType == typeof(bool))
            {
                uiField = AddField<BoolField, bool>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(int) && HasAttribute<RangeAttribute>(fieldInfo))
            {
                uiField = AddRangedField<RangedIntegerField, int>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                uiField = AddField<IntegerField, int>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(float) && HasAttribute<RangeAttribute>(fieldInfo))
            {
                uiField = AddRangedField<RangedFloatField, float>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                uiField = AddField<FloatField, float>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(Vector3))
            {
                uiField = AddField<Vector3Field, Vector3>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(string) && HasAttribute<PathAttribute>(fieldInfo))
            {
                var go = Instantiate(PathFieldPrefab, Target);
                var field = go.GetComponent<PathField>();
                var tooltip = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
                var path = (PathAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(PathAttribute))!;
                field.Setup(fieldInfo.Name.Humanize(), tooltip?.tooltip ?? "", (string)fieldInfo.GetValue(obj),
                            path.PathType == PathAttribute.PathTypes.Directory, path.Extensions);
                field.OnValueChanged()
                        .Subscribe(v => fieldInfo.SetValue(obj, v))
                        .AddTo(field);
                uiField = field;
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                uiField = AddField<StringField, string>(fieldInfo, obj);
            }
            else if (fieldInfo.FieldType == typeof(Action))
            {
                var go = Instantiate(SettingsButtonPrefab, Target);
                var field = go.GetComponent<SettingsButton>();
                var tooltip = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
                field.Setup(fieldInfo.Name.Humanize(), tooltip?.tooltip ?? "");
                uiField = field;
                field.OnClick()
                        .Subscribe(_ => ((Action)fieldInfo.GetValue(obj))?.Invoke())
                        .AddTo(field);
            }
            else
            {
                throw new ArgumentOutOfRangeException(fieldInfo.Name,
                                                      $"Don't know how to make field for {fieldInfo.FieldType.Name}.");
            }

            _fields.Add(uiField);
        }
        
        private TFieldComponentType AddField<TFieldComponentType, TFieldType>(
            FieldInfo fieldInfo, SettingsBag obj)
                where TFieldComponentType : SettingsField<TFieldType>
        {
            var field = InstantiateField<TFieldComponentType, TFieldType>();
            var tooltip = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
            field.Setup(fieldInfo.Name.Humanize(), tooltip?.tooltip ?? "", (TFieldType)fieldInfo.GetValue(obj));
            field.OnValueChanged()
                    .Subscribe(v => fieldInfo.SetValue(obj, v))
                    .AddTo(field);
            return field;
        }

        private TFieldComponentType AddRangedField<TFieldComponentType, TFieldType>(
            FieldInfo fieldInfo, SettingsBag obj)
                where TFieldComponentType : RangedSettingsField<TFieldType>
        {
            var field = InstantiateRangedField<TFieldComponentType, TFieldType>();
            var tooltip = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
            var range = (RangeAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(RangeAttribute));
            TFieldType min, max;
            if (typeof(int) == typeof(TFieldType))
            {
                min = (TFieldType)(object)(int)range.min;
                max = (TFieldType)(object)(int)range.max;
            }
            else if (typeof(float) == typeof(TFieldType))
            {
                min = (TFieldType)(object)range.min;
                max = (TFieldType)(object)range.max;
            }
            else
            {
                throw new ArgumentOutOfRangeException(typeof(TFieldType).Name,
                                                      $"Don't know how to make field for {typeof(TFieldType).Name}.");
            }
            field.Setup(fieldInfo.Name.Humanize(), tooltip?.tooltip ?? "", (TFieldType)fieldInfo.GetValue(obj),
                        min, max);
            field.OnValueChanged()
                    .Subscribe(v => fieldInfo.SetValue(obj, v))
                    .AddTo(field);

            return field;
        }

        private static bool HasAttribute<T>(MemberInfo field) => Attribute.IsDefined(field, typeof(T));

        private TFieldComponentType InstantiateField<TFieldComponentType, TFieldType>()
                where TFieldComponentType : SettingsField<TFieldType>
        {
            GameObject prefab;
            if (typeof(bool) == typeof(TFieldType))
            {
                prefab = BoolFieldPrefab;
            }
            else if (typeof(int) == typeof(TFieldType))
            {
                prefab = IntegerFieldPrefab;
            }
            else if (typeof(float) == typeof(TFieldType))
            {
                prefab = FloatFieldPrefab;
            }
            else if (typeof(string) == typeof(TFieldType))
            {
                prefab = StringFieldPrefab;
            }
            else if (typeof(Vector3) == typeof(TFieldType))
            {
                prefab = Vector3FieldPrefab;
            }
            else
            {
                throw new ArgumentOutOfRangeException(typeof(TFieldType).Name,
                                                      $"Don't know how to make field for {typeof(TFieldType).Name}.");
            }

            var go = Instantiate(prefab, Target);
            var f = go.GetComponent<TFieldComponentType>();
            return f;
        }

        private TFieldComponentType InstantiateRangedField<TFieldComponentType, TFieldType>()
                where TFieldComponentType : RangedSettingsField<TFieldType>
        {
            GameObject prefab;
            if (typeof(int) == typeof(TFieldType))
            {
                prefab = RangedIntegerFieldPrefab;
            }
            else if (typeof(float) == typeof(TFieldType))
            {
                prefab = RangedFloatFieldPrefab;
            }
            else
            {
                throw new ArgumentOutOfRangeException(typeof(TFieldType).Name,
                                                      $"Don't know how to make ranged field for {typeof(TFieldType).Name}.");
            }

            var go = Instantiate(prefab, Target);
            var f = go.GetComponent<TFieldComponentType>();
            return f;
        }

        #endregion
    }
}