using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace SS.TweenAnimations
{
    /// <summary>
    /// Editor window tool to automate the creation of new animation types and easing curves.
    /// Accessible via the top menu bar: SudStack -> TweenAnimation -> Create New Animation.
    /// </summary>
    public class TweenAnimationFactoryWindow : EditorWindow
    {
        private int _toolMode = 0;
        private string[] _modeLabels = { "Add Curve to Existing Type", "Create New Animation Type" };

        // Variables for Mode 0
        private string[] _existingAnims;
        private int _selectedAnimIndex = 0;
        private string _newCurveName = "EaseIn";

        // Variables for Mode 1
        private string _newAnimTypeName = "Fade";
        private string _initialCurveName = "Linear";

        [MenuItem("SudStack/TweenAnimation/Create New Animation")]
        public static void ShowWindow()
        {
            var window = GetWindow<TweenAnimationFactoryWindow>("Animation Factory");
            window.minSize = new Vector2(400, 300);
            window.RefreshExistingAnimations();
        }

        private void OnGUI()
        {
            GUILayout.Label("Tween Engine Code Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            _toolMode = GUILayout.Toolbar(_toolMode, _modeLabels);
            EditorGUILayout.Space(15);

            if (_toolMode == 0)
            {
                DrawAddCurveGUI();
            }
            else
            {
                DrawNewTypeGUI();
            }
        }

        private void DrawAddCurveGUI()
        {
            if (_existingAnims == null || _existingAnims.Length == 0)
            {
                RefreshExistingAnimations();
            }

            if (_existingAnims == null || _existingAnims.Length == 0)
            {
                EditorGUILayout.HelpBox("No base configs inheriting from TweenConfigBase found in the project.", MessageType.Warning);
                return;
            }

            _selectedAnimIndex = EditorGUILayout.Popup("Select Target Animation", _selectedAnimIndex, _existingAnims);
            _newCurveName = EditorGUILayout.TextField("New Curve Name", _newCurveName);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate Curve Script", GUILayout.Height(35)))
            {
                GenerateCurveScriptOnly(_existingAnims[_selectedAnimIndex], _newCurveName.Trim());
            }
        }

        private void DrawNewTypeGUI()
        {
            _newAnimTypeName = EditorGUILayout.TextField("Animation Name (e.g. Fade)", _newAnimTypeName);
            _initialCurveName = EditorGUILayout.TextField("Initial Curve Name", _initialCurveName);

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate New Animation Ecosystem", GUILayout.Height(35)))
            {
                GenerateFullAnimationEcosystem(_newAnimTypeName.Trim(), _initialCurveName.Trim());
            }
        }

        private void RefreshExistingAnimations()
        {
            _existingAnims = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TweenConfigBase).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Select(t => t.Name.Replace("TweenConfig", ""))
                .ToArray();
        }

        private string GetRuntimeRootPath()
        {
            string[] guids = AssetDatabase.FindAssets("TweenEngine t:Script");
            if (guids == null || guids.Length == 0)
            {
                Debug.LogError("[TweenFactory] Core TweenEngine.cs script not found! Cannot determine package directory structure.");
                return null;
            }

            string coreScriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            string coreFolder = Path.GetDirectoryName(coreScriptPath);
            return Path.GetDirectoryName(coreFolder);
        }

        private void GenerateCurveScriptOnly(string animName, string curveName)
        {
            string runtimeRoot = GetRuntimeRootPath();
            if (runtimeRoot == null) return;

            string animFolder = Path.Combine(runtimeRoot, "Animations", animName);
            if (!Directory.Exists(animFolder))
            {
                Directory.CreateDirectory(animFolder);
            }

            string fileName = $"{animName}{curveName}Animation.cs";
            string fullPath = Path.Combine(animFolder, fileName);

            string mutationCode = "";
            if (animName.Equals("Move", StringComparison.OrdinalIgnoreCase))
            {
                mutationCode = "Vector3 currentPos = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);\n\n" +
                               "            if (state.IsLocal)\n" +
                               "                state.Target.localPosition = currentPos;\n" +
                               "            else\n" +
                               "                state.Target.position = currentPos;";
            }
            else if (animName.Equals("Scale", StringComparison.OrdinalIgnoreCase))
            {
                mutationCode = "state.Target.localScale = Vector3.Lerp(state.StartVal, state.EndVal, smoothProgress);";
            }
            else
            {
                mutationCode = "// Apply your custom mathematical state mutation onto state.Target here";
            }

            // FIXED: All literal curly braces are now doubled {{ }} so C# interpolator doesn't crash
            string scriptContent = $@"using UnityEngine;
using System;

namespace SS.TweenAnimations
{{
    /// <summary>
    /// The data representation of the ""{curveName}"" logic curve.
    /// This class is serialized into the Unity Inspector via the ITweenLogic interface.
    /// It acts strictly as a data-holder and router, keeping UI concerns completely separated from the high-speed math engine.
    /// </summary>
    [Serializable]
    public class {animName}{curveName}Config : ITweenLogic
    {{
        // The display name shown in the ""Logic Curve"" dropdown in the Inspector.
        public string LogicName => ""{curveName}"";

        /// <summary>
        /// Bakes any specific UI variables (like custom bounciness or elasticity) into the struct before execution.
        /// Left intentionally blank here because standard ""{curveName}"" requires no extra parameters.
        /// </summary>
        public void Bake(ref TweenState state) {{ }}

        /// <summary>
        /// Routes this configuration to the specific static math function mapped inside the core engine.
        /// </summary>
        public int GetLogicID() => {animName}{curveName}Animation.LogicID;
    }}

    /// <summary>
    /// Executes a {curveName} {animName} transformation loop.
    /// </summary>
    public static class {animName}{curveName}Animation
    {{

        // The unique identifier assigned by the TweenEngine upon boot.
        public static int LogicID;

        /// <summary>
        /// Automatically injects and registers this calculation method into the core engine 
        /// routing array before the first scene loads. This entirely automates engine extensibility.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {{
            LogicID = TweenEngine.RegisterLogic(CalculateTick);
        }}

        /// <summary>
        /// The primary mathematical calculation loop executed by the engine every frame.
        /// Passed by 'ref' to ensure the struct memory is mutated directly without allocations.
        /// </summary>
        public static void CalculateTick(ref TweenState state)
        {{
            float progress = Mathf.Clamp01(state.Elapsed / state.Duration);
            float smoothProgress = EaseUtility.EvaluateLinear(progress); // Swap with your preferred EaseUtility formula

            {mutationCode}
        }}
    }}
}}";

            File.WriteAllText(fullPath, scriptContent);
            AssetDatabase.Refresh();
            Debug.Log($"[TweenFactory] Successfully added new curve logic script: {fullPath}");
        }

        private void GenerateFullAnimationEcosystem(string animName, string curveName)
        {
            if (string.IsNullOrEmpty(animName) || string.IsNullOrEmpty(curveName))
            {
                Debug.LogError("[TweenFactory] Animation Name or Curve Name cannot be empty!");
                return;
            }

            string runtimeRoot = GetRuntimeRootPath();
            if (runtimeRoot == null) return;

            string configsFolder = Path.Combine(runtimeRoot, "Configs");
            if (!Directory.Exists(configsFolder)) Directory.CreateDirectory(configsFolder);

            string configFileName = $"{animName}TweenConfig.cs";
            string configFullPath = Path.Combine(configsFolder, configFileName);

            // FIXED: Braces doubled {{ }}
            string configContent = $@"using UnityEngine;
using System;

namespace SS.TweenAnimations
{{
    /// <summary>
    /// A concrete data container for ""{animName}"" animations.
    /// Notice how lightweight this is—because the TweenConfigBase handles all lifecycle, 
    /// memory mapping, and engine routing, this class purely defines 'what' spatial data is changing.
    /// </summary>
    [Serializable]
    public class {animName}TweenConfig : TweenConfigBase
    {{
        // The name for this type of animations.
        public override string ConfigName => ""{animName}"";

        [Space(10)]
        [Header(""Animation Specifics"")]
        public float targetValue; // TODO: Change data type or fields according to your animation requirements

        /// <summary>
        /// Injects the {animName}-specific parameters directly into the master engine state.
        /// </summary>
        protected override void ApplyCustomValues(ref TweenState state)
        {{
            // TODO: Capture initial start properties and map desired destination values safely
            state.StartVal = Vector3.zero;
            state.EndVal = new Vector3(targetValue, 0f, 0f);
        }}
    }}
}}";
            File.WriteAllText(configFullPath, configContent);

            GenerateCurveScriptOnly(animName, curveName);

            RefreshExistingAnimations();
            Debug.Log($"[TweenFactory] Full animation type [{animName}] ecosystem successfully generated!");
        }
    }
}