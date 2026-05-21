using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS.TweenAnimations
{
    /// <summary>
    /// Custom property drawer for polymorphic Tween Configurations.
    /// Dynamically filters Logic Curves based on the active Animation Type to prevent cross-assignment bugs.
    /// </summary>
    [CustomPropertyDrawer(typeof(TweenConfigBase), true)]
    [CustomPropertyDrawer(typeof(ITweenConfig), true)]
    public class TweenConfigDrawer : PropertyDrawer
    {
        private List<Type> _configTypes;
        private string[] _configNames;

        private List<Type> _logicTypes;
        private string[] _logicNames;
        private Type _lastEvaluatedConfigType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (_configTypes == null) InitializeConfigTypes();

            Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            int currentConfigIndex = 0;
            object currentConfigObj = GetManagedReference(property);
            if (currentConfigObj != null)
            {
                currentConfigIndex = _configTypes.IndexOf(currentConfigObj.GetType());
                if (currentConfigIndex < 0) currentConfigIndex = 0;
            }

            Rect foldoutRect = new Rect(currentRect.x, currentRect.y, EditorGUIUtility.labelWidth, currentRect.height);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            Rect configDropdownRect = new Rect(currentRect.x + EditorGUIUtility.labelWidth, currentRect.y, currentRect.width - EditorGUIUtility.labelWidth, currentRect.height);
            int newConfigIndex = EditorGUI.Popup(configDropdownRect, currentConfigIndex, _configNames);

            if (newConfigIndex != currentConfigIndex)
            {
                property.managedReferenceValue = (newConfigIndex == 0) ? null : Activator.CreateInstance(_configTypes[newConfigIndex]);
                property.serializedObject.ApplyModifiedProperties();
                _lastEvaluatedConfigType = null; // Force logic filter reload
                EditorGUI.EndProperty();
                return;
            }

            if (property.isExpanded && property.managedReferenceValue != null)
            {
                Type currentConfigType = currentConfigObj.GetType();

                // DYNAMIC FILTERING: Rebuild logic list only if the config type changed
                if (currentConfigType != _lastEvaluatedConfigType)
                {
                    FilterLogicTypesForConfig(currentConfigType);
                }

                currentRect.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.indentLevel++;

                SerializedProperty logicProp = property.FindPropertyRelative("logicConfig");

                // Safely auto-assign the first matching curve for this specific animation type
                if (logicProp != null && logicProp.managedReferenceValue == null && _logicTypes.Count > 1)
                {
                    logicProp.managedReferenceValue = Activator.CreateInstance(_logicTypes[1]);
                    logicProp.serializedObject.ApplyModifiedProperties();
                }

                if (logicProp != null)
                {
                    DrawLogicDropdown(ref currentRect, logicProp);
                }

                if (logicProp != null && logicProp.managedReferenceValue != null)
                {
                    SerializedProperty iterator = property.Copy();
                    SerializedProperty endProperty = iterator.GetEndProperty();

                    SerializedProperty loopTypeProp = property.FindPropertyRelative("loopType");
                    bool hideLoops = loopTypeProp != null && loopTypeProp.enumValueIndex == 0;

                    iterator.NextVisible(true);

                    while (!SerializedProperty.EqualContents(iterator, endProperty))
                    {
                        if (iterator.name == "logicConfig")
                        {
                            iterator.NextVisible(false);
                            continue;
                        }

                        if (iterator.name == "loops" && hideLoops)
                        {
                            iterator.NextVisible(false);
                            continue;
                        }

                        float h = EditorGUI.GetPropertyHeight(iterator, true);
                        currentRect.height = h;
                        EditorGUI.PropertyField(currentRect, iterator, true);
                        currentRect.y += h + 2;

                        iterator.NextVisible(false);
                    }

                    SerializedProperty logicIterator = logicProp.Copy();
                    SerializedProperty logicEndProperty = logicIterator.GetEndProperty();

                    logicIterator.NextVisible(true);
                    while (!SerializedProperty.EqualContents(logicIterator, logicEndProperty))
                    {
                        float h = EditorGUI.GetPropertyHeight(logicIterator, true);
                        currentRect.height = h;
                        EditorGUI.PropertyField(currentRect, logicIterator, true);
                        currentRect.y += h + 2;

                        logicIterator.NextVisible(false);
                    }
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private void DrawLogicDropdown(ref Rect currentRect, SerializedProperty logicProp)
        {
            int currentLogicIndex = 0;
            object currentLogicObj = GetManagedReference(logicProp);
            if (currentLogicObj != null)
            {
                currentLogicIndex = _logicTypes.IndexOf(currentLogicObj.GetType());
                if (currentLogicIndex < 0) currentLogicIndex = 0;
            }

            Rect labelRect = new Rect(currentRect.x, currentRect.y, EditorGUIUtility.labelWidth, currentRect.height);
            Rect popupRect = new Rect(currentRect.x + EditorGUIUtility.labelWidth, currentRect.y, currentRect.width - EditorGUIUtility.labelWidth, currentRect.height);

            EditorGUI.LabelField(labelRect, "Animation Logic");
            int newLogicIndex = EditorGUI.Popup(popupRect, currentLogicIndex, _logicNames);

            if (newLogicIndex != currentLogicIndex)
            {
                logicProp.managedReferenceValue = (newLogicIndex == 0) ? null : Activator.CreateInstance(_logicTypes[newLogicIndex]);
                logicProp.serializedObject.ApplyModifiedProperties();
            }

            currentRect.y += EditorGUIUtility.singleLineHeight + 2;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded && property.managedReferenceValue != null)
            {
                SerializedProperty logicProp = property.FindPropertyRelative("logicConfig");

                if (logicProp != null)
                {
                    totalHeight += EditorGUIUtility.singleLineHeight + 2;
                }

                if (logicProp != null && logicProp.managedReferenceValue != null)
                {
                    SerializedProperty iterator = property.Copy();
                    SerializedProperty endProperty = iterator.GetEndProperty();

                    SerializedProperty loopTypeProp = property.FindPropertyRelative("loopType");
                    bool hideLoops = loopTypeProp != null && loopTypeProp.enumValueIndex == 0;

                    iterator.NextVisible(true);

                    while (!SerializedProperty.EqualContents(iterator, endProperty))
                    {
                        if (iterator.name == "logicConfig" || (iterator.name == "loops" && hideLoops))
                        {
                            iterator.NextVisible(false);
                            continue;
                        }

                        totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + 2;
                        iterator.NextVisible(false);
                    }

                    SerializedProperty logicIterator = logicProp.Copy();
                    SerializedProperty logicEndProperty = logicIterator.GetEndProperty();

                    logicIterator.NextVisible(true);
                    while (!SerializedProperty.EqualContents(logicIterator, logicEndProperty))
                    {
                        totalHeight += EditorGUI.GetPropertyHeight(logicIterator, true) + 2;
                        logicIterator.NextVisible(false);
                    }
                }
                totalHeight += 4;
            }
            return totalHeight;
        }

        private void InitializeConfigTypes()
        {
            _configTypes = new List<Type> { null };
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TweenConfigBase).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            _configTypes.AddRange(types);

            _configNames = new string[_configTypes.Count];
            _configNames[0] = "None (Select Type)";
            for (int i = 1; i < _configTypes.Count; i++)
            {
                var tempInstance = (TweenConfigBase)Activator.CreateInstance(_configTypes[i]);
                _configNames[i] = tempInstance.ConfigName;
            }
        }

        /// <summary>
        /// Filters and extracts only the ITweenLogic implementations that match the prefix of the current configuration.
        /// </summary>
        private void FilterLogicTypesForConfig(Type configType)
        {
            _lastEvaluatedConfigType = configType;

            // Extract prefix (e.g., "RotateTweenConfig" becomes "Rotate")
            string prefix = configType.Name.Replace("TweenConfig", "");

            var allLogicTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ITweenLogic).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            _logicTypes = new List<Type> { null };

            foreach (var type in allLogicTypes)
            {
                // Strict rule: Only include curves that belong to this animation group name prefix
                if (type.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    _logicTypes.Add(type);
                }
            }

            _logicNames = new string[_logicTypes.Count];
            _logicNames[0] = "None (Select Curve)";
            for (int i = 1; i < _logicTypes.Count; i++)
            {
                var tempInstance = (ITweenLogic)Activator.CreateInstance(_logicTypes[i]);
                _logicNames[i] = tempInstance.LogicName;
            }
        }

        private object GetManagedReference(SerializedProperty prop)
        {
            string[] path = prop.propertyPath.Split('.');
            object obj = prop.serializedObject.targetObject;
            foreach (var element in path)
            {
                if (obj == null) return null;
                var type = obj.GetType();
                var field = type.GetField(element, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (field != null) obj = field.GetValue(obj);
            }
            return obj;
        }
    }
}