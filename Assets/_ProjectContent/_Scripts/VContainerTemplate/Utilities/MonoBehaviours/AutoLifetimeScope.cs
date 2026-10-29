using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerTemplate.Infrastructure.Logger;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VContainerTemplate.Utilities.MonoBehaviours
{
    public class AutoLifetimeScope : LifetimeScope
    {
#if UNITY_EDITOR
        private static readonly HashSet<Type> TypesWithInjectAttribute = new();
        private static readonly HashSet<Type> TypesWithoutInjectAttribute = new();

        internal bool AutoCollectInjectObjects()
        {
            if (autoInjectGameObjects == null)
                autoInjectGameObjects = new List<GameObject>();

            var uniqueGameObjects = new HashSet<GameObject>();

            var sceneMonos = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var mono in sceneMonos)
            {
                if (mono == null) continue;
                if (TypeHasInjectAttribute(mono.GetType()))
                    uniqueGameObjects.Add(mono.gameObject);
            }

            foreach (var mono in GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (mono == null) continue;
                if (TypeHasInjectAttribute(mono.GetType()))
                    uniqueGameObjects.Add(mono.gameObject);
            }

            var collected = uniqueGameObjects.ToList();
            if (SetsEqual(autoInjectGameObjects, collected))
                return false;

            autoInjectGameObjects = collected;
            EditorUtility.SetDirty(this);
            return true;
        }

        private static bool TypeHasInjectAttribute(Type type)
        {
            if (TypesWithInjectAttribute.Contains(type)) return true;
            if (TypesWithoutInjectAttribute.Contains(type)) return false;

            var hasInject = HasInjectAttribute(type);
            (hasInject ? TypesWithInjectAttribute : TypesWithoutInjectAttribute).Add(type);
            return hasInject;
        }

        private static bool HasInjectAttribute(Type type)
        {
            const System.Reflection.BindingFlags flags =
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic;

            while (type != null)
            {
                foreach (var field in type.GetFields(flags))
                {
                    if (System.Attribute.IsDefined(field, typeof(InjectAttribute)))
                        return true;
                }

                foreach (var method in type.GetMethods(flags))
                {
                    if (System.Attribute.IsDefined(method, typeof(InjectAttribute)))
                        return true;
                }

                foreach (var property in type.GetProperties(flags))
                {
                    if (System.Attribute.IsDefined(property, typeof(InjectAttribute)))
                        return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static bool SetsEqual(List<GameObject> a, List<GameObject> b)
        {
            if (a == null || b == null) return a == null && b == null;
            if (a.Count != b.Count) return false;

            var set = new HashSet<GameObject>(a);
            foreach (var go in b)
            {
                if (go == null || !set.Contains(go)) return false;
            }

            return true;
        }
#endif
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class AutoLifetimeScopeHooks
    {
        static AutoLifetimeScopeHooks()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        private static void OnSceneSaving(UnityEngine.SceneManagement.Scene scene, string path)
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying) return;

            RefreshScopesInScene(scene);
        }

        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying) return;

            RefreshScopesInScene(scene);
        }

        private static void RefreshScopesInScene(UnityEngine.SceneManagement.Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded) return;

            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var scope in root.GetComponentsInChildren<AutoLifetimeScope>(true))
                {
                    try
                    {
                        scope.AutoCollectInjectObjects();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
#endif
}