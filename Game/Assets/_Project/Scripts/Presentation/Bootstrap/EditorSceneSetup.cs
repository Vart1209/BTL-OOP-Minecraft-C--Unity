#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Minecraft.Presentation.Environment;
using Minecraft.Presentation.Player;
using Minecraft.Presentation.UI;
using Minecraft.Presentation.World;

namespace Minecraft.Presentation.Bootstrap
{
    /// <summary>
    /// Công cụ tự động dựng Scene chơi được ngay lập tức trong 1 cú nhấp chuột trên thanh Menu của Unity Editor (Tuần T5).
    /// <para>
    /// <b>Mục đích học thuật &amp; Bảo vệ đồ án:</b>
    /// <list type="bullet">
    ///   <item>Giúp các thành viên trong nhóm hoặc giảng viên chấm bài khi mở dự án trên máy tính bất kỳ chỉ cần bấm <c>Minecraft OOP -&gt; 1. Auto-Setup Playable Game Scene</c> rồi nhấn <b>Play</b> là trải nghiệm được toàn bộ 12 Use Case ngay lập tức.</item>
    ///   <item>Tự động liên kết (wire) toàn bộ các trường <c>[SerializeField]</c> của <see cref="GameBootstrap"/> thông qua <see cref="SerializedObject"/> mà không sợ quên kéo thả thủ công trên Inspector.</item>
    /// </list>
    /// </para>
    /// </summary>
    public static class EditorSceneSetup
    {
        private const string ScenesFolderPath = "Assets/_Project/Scenes";
        private const string MaterialsFolderPath = "Assets/_Project/Art/Materials";
        private const string DefaultTerrainMaterialPath = "Assets/_Project/Art/Materials/VoxelTerrainMaterial.mat";

        /// <summary>
        /// Tự động tạo hoặc làm sạch toàn bộ các đối tượng cốt lõi trong Scene hiện tại để nhấn Play chơi được ngay.
        /// </summary>
        [MenuItem("Minecraft OOP/1. Auto-Setup Playable Game Scene", priority = 10)]
        public static void AutoSetupPlayableGameScene()
        {
            SetupCurrentSceneHierarchy();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("[EditorSceneSetup] Đã dựng xong Scene hoàn chỉnh! Hãy nhấn nút Play (▶) để trải nghiệm ngay.");
        }

        /// <summary>
        /// Tự động khởi tạo và lưu đầy đủ 4 Scene chuẩn của dự án (<c>MainMenu</c>, <c>Game</c>, <c>Sandbox_A</c>, <c>Sandbox_B</c>) vào <c>Assets/_Project/Scenes/</c>.
        /// </summary>
        [MenuItem("Minecraft OOP/2. Generate All 4 Scenes (MainMenu, Game, Sandbox_A, Sandbox_B)", priority = 20)]
        public static void GenerateAllFourScenes()
        {
            EnsureFolderExists("Assets/_Project");
            EnsureFolderExists(ScenesFolderPath);

            string[] sceneNames = { "MainMenu", "Game", "Sandbox_A", "Sandbox_B" };
            foreach (string sceneName in sceneNames)
            {
                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                newScene.name = sceneName;

                SetupCurrentSceneHierarchy();

                string fullScenePath = $"{ScenesFolderPath}/{sceneName}.unity";
                EditorSceneManager.SaveScene(newScene, fullScenePath);
                Debug.Log($"[EditorSceneSetup] Đã tạo và lưu Scene: {fullScenePath}");
            }

            // Mở lại Scene Game chính sau khi tạo xong 4 Scene
            string mainGameScenePath = $"{ScenesFolderPath}/Game.unity";
            if (File.Exists(mainGameScenePath))
            {
                EditorSceneManager.OpenScene(mainGameScenePath, OpenSceneMode.Single);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[EditorSceneSetup] Hoàn tất khởi tạo cả 4 Scenes (MainMenu, Game, Sandbox_A, Sandbox_B)!");
        }

        private static void SetupCurrentSceneHierarchy()
        {
            // Dọn sạch các đối tượng cũ nếu chạy lại menu nhiều lần
            DestroyExistingIfPresent("Directional Light");
            DestroyExistingIfPresent("Main Camera");
            DestroyExistingIfPresent("WorldRoot");
            DestroyExistingIfPresent("Player");
            DestroyExistingIfPresent("HUD");
            DestroyExistingIfPresent("GameBootstrap");

            // 1. Tạo Directional Light gắn DayNightView & AtmosphereFogView
            GameObject lightGo = new GameObject("Directional Light");
            Light directionalLight = lightGo.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
            directionalLight.color = new Color(1f, 0.95f, 0.85f);
            directionalLight.intensity = 1.0f;
            directionalLight.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            DayNightView dayNightView = lightGo.AddComponent<DayNightView>();
            AtmosphereFogView atmosphereFogView = lightGo.AddComponent<AtmosphereFogView>();

            // 2. Tạo WorldRoot gắn WorldView & ResourcePackView kèm Material mặc định
            GameObject worldRootGo = new GameObject("WorldRoot");
            WorldView worldView = worldRootGo.AddComponent<WorldView>();
            ResourcePackView resourcePackView = worldRootGo.AddComponent<ResourcePackView>();

            Material terrainMaterial = GetOrCreateDefaultTerrainMaterial();
            SerializedObject rpSerialized = new SerializedObject(resourcePackView);
            SetObjectReferenceIfPresent(rpSerialized, "_worldView", worldView);
            SetObjectReferenceIfPresent(rpSerialized, "_targetMaterial", terrainMaterial);
            rpSerialized.ApplyModifiedPropertiesWithoutUndo();

            // 3. Tạo Player ở tọa độ (0, 42, 0) gắn CharacterController, FirstPersonPlayerController, PlayerInteractionController và Camera con ở tầm mắt (0, 1.6f, 0)
            GameObject playerGo = new GameObject("Player");
            playerGo.transform.position = new Vector3(0f, 42f, 0f);

            CharacterController cc = playerGo.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.9f, 0f);

            FirstPersonPlayerController fpsController = playerGo.AddComponent<FirstPersonPlayerController>();
            PlayerInteractionController interactionController = playerGo.AddComponent<PlayerInteractionController>();

            GameObject cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.transform.SetParent(playerGo.transform, worldPositionStays: false);
            cameraGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            cameraGo.transform.localRotation = Quaternion.identity;

            Camera cam = cameraGo.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 250f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.62f, 0.82f, 0.98f, 1f);
            cameraGo.AddComponent<AudioListener>();

            // Nối _cameraTransform vào FirstPersonPlayerController & PlayerInteractionController
            SerializedObject fpsSerialized = new SerializedObject(fpsController);
            SetObjectReferenceIfPresent(fpsSerialized, "_cameraTransform", cameraGo.transform);
            fpsSerialized.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject interactionSerialized = new SerializedObject(interactionController);
            SetObjectReferenceIfPresent(interactionSerialized, "_cameraTransform", cameraGo.transform);
            interactionSerialized.ApplyModifiedPropertiesWithoutUndo();

            // 4. Tạo HUD gắn SandboxHudView
            GameObject hudGo = new GameObject("HUD");
            SandboxHudView hudView = hudGo.AddComponent<SandboxHudView>();

            // 5. Tạo GameBootstrap và nối toàn bộ các serialized fields
            GameObject bootstrapGo = new GameObject("GameBootstrap");
            GameBootstrap bootstrap = bootstrapGo.AddComponent<GameBootstrap>();

            SerializedObject bootstrapSerialized = new SerializedObject(bootstrap);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_worldView", worldView);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_resourcePackView", resourcePackView);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_playerController", fpsController);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_interactionController", interactionController);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_hudView", hudView);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_dayNightView", dayNightView);
            SetObjectReferenceIfPresent(bootstrapSerialized, "_atmosphereFogView", atmosphereFogView);
            bootstrapSerialized.ApplyModifiedPropertiesWithoutUndo();

            Selection.activeGameObject = bootstrapGo;
        }

        private static Material GetOrCreateDefaultTerrainMaterial()
        {
            EnsureFolderExists("Assets/_Project");
            EnsureFolderExists("Assets/_Project/Art");
            EnsureFolderExists(MaterialsFolderPath);

            Material existingMat = AssetDatabase.LoadAssetAtPath<Material>(DefaultTerrainMaterialPath);
            if (existingMat != null)
            {
                return existingMat;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material mat = new Material(shader)
            {
                name = "VoxelTerrainMaterial"
            };

            if (mat.HasProperty("_Smoothness"))
            {
                mat.SetFloat("_Smoothness", 0.0f);
            }

            if (mat.HasProperty("_Glossiness"))
            {
                mat.SetFloat("_Glossiness", 0.0f);
            }

            AssetDatabase.CreateAsset(mat, DefaultTerrainMaterialPath);
            AssetDatabase.SaveAssets();
            return mat;
        }

        private static void SetObjectReferenceIfPresent(SerializedObject serializedObject, string propertyName, Object target)
        {
            SerializedProperty prop = serializedObject.FindProperty(propertyName);
            if (prop != null)
            {
                prop.objectReferenceValue = target;
            }
        }

        private static void DestroyExistingIfPresent(string objectName)
        {
            GameObject existing = GameObject.Find(objectName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
                string folderName = Path.GetFileName(folderPath);
                if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(folderName))
                {
                    AssetDatabase.CreateFolder(parent, folderName);
                }
            }
        }
    }
}
#endif
