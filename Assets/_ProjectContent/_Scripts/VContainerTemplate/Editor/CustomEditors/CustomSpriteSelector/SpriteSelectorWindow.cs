using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VContainerTemplateEditor.CustomEditors.CustomSpriteSelector
{
    public class SpriteSelectorPopup : PopupWindowContent
    {
        public event Action<Sprite> OnSpriteSelected;

        private List<Sprite> allSprites = new();
        private List<Sprite> filteredSprites = new();
        private string searchText = "";
        private Vector2 scrollPos;
        private Sprite previewSprite;

        private int columns = 4;
        private float cellSize = 100f;
        private float previewWidth = 350f;

        public override Vector2 GetWindowSize() => new Vector2(1300, 700);

        public override void OnOpen()
        {
            LoadSprites();
            ApplyFilter();
        }

        public override void OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                editorWindow.Close();
                Event.current.Use();
                return;
            }

            DrawToolbar();

            Rect remaining = GUILayoutUtility.GetRect(0, 10000, 0, 10000);
            float rightWidth = previewSprite != null ? previewWidth : 0;
            Rect listRect = new Rect(remaining.x, remaining.y, remaining.width - rightWidth, remaining.height);
            Rect previewRect = new Rect(remaining.width - rightWidth, remaining.y, rightWidth, remaining.height);

            DrawSpriteGrid(listRect);
            if (previewSprite != null) DrawPreview(previewRect);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Search:", GUILayout.Width(50));
            string newSearch = EditorGUILayout.TextField(searchText, GUILayout.MinWidth(150));
            if (newSearch != searchText)
            {
                searchText = newSearch;
                ApplyFilter();
            }

            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                LoadSprites();
                ApplyFilter();
            }

            if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                searchText = "";
                ApplyFilter();
            }

            if (GUILayout.Button("Close", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                editorWindow.Close();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSpriteGrid(Rect rect)
        {
            if (filteredSprites.Count == 0)
            {
                GUI.Label(rect, "No sprites", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            columns = Mathf.Max(1, Mathf.FloorToInt(rect.width / (cellSize + 10)));
            float cellWidth = rect.width / columns;
            float cellHeight = cellSize + 25;

            float totalHeight = Mathf.CeilToInt(filteredSprites.Count / (float)columns) * cellHeight;
            Rect viewRect = new Rect(0, 0, rect.width - 16, totalHeight);
            scrollPos = GUI.BeginScrollView(rect, scrollPos, viewRect);

            int index = 0;
            for (int row = 0; row < Mathf.CeilToInt(filteredSprites.Count / (float)columns); row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (index >= filteredSprites.Count) break;
                    Rect cellRect = new Rect(col * cellWidth, row * cellHeight, cellWidth, cellHeight);
                    DrawSpriteCell(cellRect, filteredSprites[index]);
                    index++;
                }
            }

            GUI.EndScrollView();
        }

        private void DrawSpriteCell(Rect rect, Sprite sprite)
        {
            EditorGUI.DrawRect(rect, new Color(0.22f, 0.22f, 0.22f));

            float imageSize = Mathf.Min(rect.width - 10, rect.height - 30);
            Rect imageRect = new Rect(rect.x + (rect.width - imageSize) / 2, rect.y + 5, imageSize, imageSize);
            if (sprite != null)
            {
                Rect uvRect = GetSpriteUVs(sprite);
                GUI.DrawTextureWithTexCoords(imageRect, sprite.texture, uvRect);
            }

            string name = sprite.name;
            if (name.Length > 25) name = name.Substring(0, 22) + "...";
            Rect labelRect = new Rect(rect.x, rect.y + rect.height - 22, rect.width, 20);
            EditorGUI.LabelField(labelRect, name, EditorStyles.miniLabel);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                OnSpriteSelected?.Invoke(sprite);
                if (Event.current.clickCount == 2)
                {
                    editorWindow.Close();
                }
                else
                {
                    previewSprite = sprite;
                    Event.current.Use();
                    editorWindow.Repaint();
                }
            }
        }

        private Rect GetSpriteUVs(Sprite sprite)
        {
            Vector2[] uvs = sprite.uv;
            if (uvs == null || uvs.Length < 4)
                return new Rect(0, 0, 1, 1);

            float minU = Mathf.Min(uvs[0].x, uvs[1].x, uvs[2].x, uvs[3].x);
            float minV = Mathf.Min(uvs[0].y, uvs[1].y, uvs[2].y, uvs[3].y);
            float maxU = Mathf.Max(uvs[0].x, uvs[1].x, uvs[2].x, uvs[3].x);
            float maxV = Mathf.Max(uvs[0].y, uvs[1].y, uvs[2].y, uvs[3].y);
            return new Rect(minU, minV, maxU - minU, maxV - minV);
        }

        private void DrawPreview(Rect rect)
        {
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.alignment = TextAnchor.MiddleCenter;

            if (previewSprite == null) return;

            float maxImageSize = Mathf.Min(rect.width - 20, rect.height - 100);
            Rect imageRect = new Rect(
                rect.x + (rect.width - maxImageSize) / 2,
                rect.y + 30,
                maxImageSize,
                maxImageSize
            );

            Rect uvRect = GetSpriteUVs(previewSprite);
            GUI.DrawTextureWithTexCoords(imageRect, previewSprite.texture, uvRect);

            float yOffset = imageRect.y + maxImageSize + 10;
            float labelWidth = rect.width - 10;

            EditorGUI.LabelField(new Rect(rect.x + 5, yOffset, labelWidth, 18), $"Name: {previewSprite.name}", EditorStyles.miniLabel);
            yOffset += 20;

            string assetPath = AssetDatabase.GetAssetPath(previewSprite);
            EditorGUI.LabelField(new Rect(rect.x + 5, yOffset, labelWidth, 18), $"Path: {assetPath}", EditorStyles.miniLabel);
            yOffset += 20;

            EditorGUI.LabelField(new Rect(rect.x + 5, yOffset, labelWidth, 18), $"Size: {previewSprite.rect.width}x{previewSprite.rect.height} px", EditorStyles.miniLabel);
        }

        private void LoadSprites()
        {
            allSprites.Clear();
            string[] guids = AssetDatabase.FindAssets("t:Sprite");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Contains("_Shared")) continue;
                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s == null) continue;
                allSprites.Add(s);
            }

            allSprites = allSprites.OrderBy(s => s.name).ToList();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(searchText))
                filteredSprites = new List<Sprite>(allSprites);
            else
                filteredSprites = allSprites.Where(s => s.name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            if (previewSprite != null && !filteredSprites.Contains(previewSprite))
                previewSprite = null;
        }
    }
}