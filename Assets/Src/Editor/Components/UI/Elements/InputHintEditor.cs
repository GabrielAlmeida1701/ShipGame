using ShipGame.Helpers;
using ShipGame.UI.Elements;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ShipGameEditor.Components.UI.Elements
{
    [CustomEditor(typeof(InputHint), true)]
    public class InputHintEditor : Editor
    {
        private InputHint inputHint => target as InputHint;

        private string[] options;

        private void OnEnable()
        {
            options = Enum.GetNames(typeof(InputDeviceType));

            if (inputHint.hintInputIcons == null)
                inputHint.hintInputIcons = new List<Sprite>();

            if (inputHint.hintInputIcons.Count != options.Length)
                inputHint.hintInputIcons = FixInputList();

            var image = serializedObject.FindProperty("image");
            if (!image.objectReferenceValue)
            {
                image.objectReferenceValue = inputHint.GetComponent<Image>();
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorUtils.DefaultScriptField(inputHint);

            if (inputHint.hintInputIcons.Count != options.Length)
                inputHint.hintInputIcons = FixInputList();

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < options.Length; i++)
                inputHint.hintInputIcons[i] = EditorUtils.ObjectField(options[i], inputHint.hintInputIcons[i]);

            if (EditorGUI.EndChangeCheck())
                EditorUtils.SetDirty(inputHint);
        }

        private List<Sprite> FixInputList()
        {
            List<Sprite> result = new List<Sprite>();
            for (int i = 0; i < options.Length; i++)
            {
                if (i < inputHint.hintInputIcons.Count)
                    result.Add(inputHint.hintInputIcons[i]);
                else result.Add(null);
            }

            return result;
        }
    }
}