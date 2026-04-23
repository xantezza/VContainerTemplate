#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VContainer;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
#endif
using VContainer.Unity;
using VContainerTemplate.Infrastructure.Services.Log;

namespace VContainerTemplate.Utilities.MonoBehaviours
{
    public class AutoLifetimeScope : LifetimeScope
    {
#if UNITY_EDITOR
        [ContextMenu("Refresh Auto Inject Objects")]
        [Button]
        private void RefreshAutoInjectObjects()
        {
            AutoCollectInjectObjects();
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            Log.Trace($"AutoInjectScope: found {autoInjectGameObjects.Count} objects for injection");
        }
    
        private void AutoCollectInjectObjects()
        {
            autoInjectGameObjects.Clear();
        
            var allMonos = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).Concat(GetComponentsInChildren<MonoBehaviour>(true));
            var uniqueGameObjects = new HashSet<GameObject>();
        
            foreach (var mono in allMonos)
            {
                if (mono == null) continue;
            
                if (HasInjectAttribute(mono))
                {
                    uniqueGameObjects.Add(mono.gameObject);
                }
            }
        
            autoInjectGameObjects = uniqueGameObjects.ToList();
            EditorUtility.SetDirty(this);
        }
    
        private bool HasInjectAttribute(MonoBehaviour mono)
        {
            var type = mono.GetType();
        
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance | 
                                        System.Reflection.BindingFlags.Public | 
                                        System.Reflection.BindingFlags.NonPublic);
        
            foreach (var field in fields)
            {
                if (System.Attribute.IsDefined(field, typeof(InjectAttribute)))
                {
                    return true;
                }
            }
        
            var methods = type.GetMethods(System.Reflection.BindingFlags.Instance | 
                                          System.Reflection.BindingFlags.Public | 
                                          System.Reflection.BindingFlags.NonPublic);
        
            foreach (var method in methods)
            {
                if (System.Attribute.IsDefined(method, typeof(InjectAttribute)))
                {
                    return true;
                }
            }
        
            var properties = type.GetProperties(System.Reflection.BindingFlags.Instance | 
                                                System.Reflection.BindingFlags.Public | 
                                                System.Reflection.BindingFlags.NonPublic);
        
            foreach (var property in properties)
            {
                if (System.Attribute.IsDefined(property, typeof(InjectAttribute)))
                {
                    return true;
                }
            }
        
            return false;
        }
#endif
    }
}