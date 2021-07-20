using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Elektronik.UI
{
    public class ControlsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject RowPrefab;
        
        private void Start()
        {
            var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
            SerializedObject obj = new SerializedObject(inputManager);
            SerializedProperty axisArray = obj.FindProperty("m_Axes");

            for (int i = 0; i < axisArray.arraySize; ++i)
            {
                var axis = axisArray.GetArrayElementAtIndex(i);

                var name = axis.FindPropertyRelative("m_Name").stringValue;
                if (name == "Reserved") continue;

                var negative = axis.FindPropertyRelative("negativeButton").stringValue;
                var positive = axis.FindPropertyRelative("positiveButton").stringValue;
                var altNegative = axis.FindPropertyRelative("altNegativeButton").stringValue;
                var altPositive = axis.FindPropertyRelative("altPositiveButton").stringValue;

                var data = new[] {negative, positive, altNegative, altPositive};

                if (data.All(string.IsNullOrEmpty)) continue;

                var row = Instantiate(RowPrefab, transform).GetComponent<ControlsRow>();
                if (row is null) return;

                row.NameLabel.text = name;
                row.MainInput.text = string.Join(" / ", data.Take(2).Where(s => !string.IsNullOrEmpty(s)));
                row.AltInput.text = string.Join(" / ", data.Skip(2).Take(2).Where(s => !string.IsNullOrEmpty(s)));
            }
        }
    }
}