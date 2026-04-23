using UnityEditor;
using UnityEngine;

namespace VContainerTemplateEditor.Editor.CustomEditors.CustomSpriteSelector
{
    [CustomPropertyDrawer(typeof(Sprite))]
    public class SpritePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);
        
            EditorGUI.PrefixLabel(labelRect, label);
            EditorGUI.BeginChangeCheck();
        
            var currentSprite = (Sprite)property.objectReferenceValue;
            var newSprite = EditorGUI.ObjectField(fieldRect, currentSprite, typeof(Sprite), false);
        
            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = newSprite;
            }
        
            var buttonRect = new Rect(fieldRect.xMax - 20, fieldRect.y, 20, fieldRect.height);
            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_EditCollider")))
            {
                var popup = new SpriteSelectorPopup();
                popup.OnSpriteSelected += (sprite) =>
                {
                    property.objectReferenceValue = sprite;
                    property.serializedObject.ApplyModifiedProperties();
                };
                PopupWindow.Show(buttonRect, popup);
            }
        }
    }
}