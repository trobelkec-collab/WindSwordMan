using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Cinemachine; // Cinemachine 2.x namespace
using Controllers;
using Managers;
using UnityEngine.UI;
// Trigger Recompile

public class ProjectSetupTools : EditorWindow
{
    [MenuItem("Tools/Edo Highway/Setup Phase 1")]
    public static void SetupPhase1()
    {
        SetupPlayer();
        SetupCamera();
        SetupGround();
        Debug.Log("Success: Phase 1 Setup Complete!");
    }

    private static void SetupGround()
    {
        GameObject ground = GameObject.Find("Ground");
        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(20, 1, 20); // Big enough area
            ground.transform.position = Vector3.zero;
        }
    }

    private static void SetupPlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0, 1, 0);
        }

        // 1. CharacterController
        if (!player.GetComponent<CharacterController>())
            player.AddComponent<CharacterController>();

        // 2. PlayerInput
        PlayerInput input = player.GetComponent<PlayerInput>();
        if (input == null)
            input = player.AddComponent<PlayerInput>();

        // Load Input Actions
        InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        if (actions != null)
        {
            input.actions = actions;
            input.defaultActionMap = "Player";
        }
        else
        {
            Debug.LogError("Error: Could not find 'InputSystem_Actions.inputactions' in Assets root.");
        }

        // 3. PlayerController
        if (!player.GetComponent<PlayerController>())
            player.AddComponent<PlayerController>();
        
        Selection.activeGameObject = player;
    }

    private static void SetupCamera()
    {
        // 1. Main Camera + Brain
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        // Add CinemachineBrain if missing
        if (!cam.GetComponent<CinemachineBrain>())
            cam.gameObject.AddComponent<CinemachineBrain>();

        // 2. Create Virtual Camera
        GameObject vcamObj = GameObject.Find("CM_PlayerCam");
        if (vcamObj == null)
            vcamObj = new GameObject("CM_PlayerCam");

        // Use CinemachineVirtualCamera (v2 API) instead of CinemachineCamera (v3 API)
        CinemachineVirtualCamera vcam = vcamObj.GetComponent<CinemachineVirtualCamera>();
        if (vcam == null)
            vcam = vcamObj.AddComponent<CinemachineVirtualCamera>();

        // 3. Configure Target
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform; 
        }

        // 4. Basic Settings (v2 API usage)
        vcam.m_Lens.FieldOfView = 60f;
        
        // 5. Setup Body (Transposer) for Top-Down View
        var transposer = vcam.AddCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            transposer.m_FollowOffset = new Vector3(0, 10, -10);
        }
        
        // 6. Setup Aim (Composer) to look at player
        var composer = vcam.AddCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_TrackedObjectOffset = new Vector3(0, 0, 0); // Look at center
            composer.m_ScreenX = 0.5f;
            composer.m_ScreenY = 0.5f;
            composer.m_DeadZoneWidth = 0f;
            composer.m_SoftZoneWidth = 0.8f;
            composer.m_SoftZoneHeight = 0.8f;
        }

        // 7. Add Zoom Script
        if (vcamObj.GetComponent<Controllers.CameraZoom>() == null)
        {
            vcamObj.AddComponent<Controllers.CameraZoom>();
        }

        Debug.Log("Cinemachine (v2) Setup Complete! Check 'CM_PlayerCam' in hierarchy.");
    }


    [MenuItem("Tools/Edo Highway/Setup Phase 1.5 (Systems)")]
    public static void SetupPhase1_5()
    {
        SetupWebtoonSystem();
        SetupCombatSystem();
        Debug.Log("Success: Phase 1.5 Systems Setup Complete!");
    }

    private static void SetupWebtoonSystem()
    {
        // 1. Create Canvas
        GameObject canvasObj = GameObject.Find("WebtoonCanvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("WebtoonCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        // 2. Create Manager
        GameObject managerObj = GameObject.Find("CineComicManager");
        if (managerObj == null)
        {
            managerObj = new GameObject("CineComicManager");
            managerObj.AddComponent<CineComicManager>();
        }
    }

    private static void SetupCombatSystem()
    {
        GameObject goreObj = GameObject.Find("BloodAssistant");
        if (goreObj == null)
        {
            goreObj = new GameObject("BloodAssistant");
            goreObj.AddComponent<Combat.BloodAssistant>();
        }
    }

    [MenuItem("Tools/Edo Highway/Fix Missing Scripts")]
    public static void CleanMissingScripts()
    {
        // Find all GameObjects in the scene (including disabled ones)
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int count = 0;
        int objectCount = 0;

        foreach (GameObject go in allObjects)
        {
            // Filter out assets (prefabs on disk) and only affect scene objects
            // hideFlags check ensures we don't touch internal engine objects
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && 
                (go.hideFlags == HideFlags.None || go.hideFlags == HideFlags.HideInHierarchy))
            {
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                if (removed > 0)
                {
                    count += removed;
                    objectCount++;
                    Debug.Log($"Removed {removed} missing scripts from '{go.name}'", go);
                }
            }
        }
        
        if (count > 0)
        {
            Debug.Log($"<color=green>Success:</color> Removed total {count} missing scripts from {objectCount} GameObjects.");
            EditorUtility.DisplayDialog("Cleanup Complete", $"Removed {count} missing scripts from {objectCount} GameObjects.\nCheck Console for details.", "OK");
        }
        else
        {
            Debug.Log("No missing scripts found in the current scene.");
            EditorUtility.DisplayDialog("Cleanup Complete", "No missing scripts found in the current scene.", "OK");
        }
    }
    [MenuItem("Tools/Edo Highway/Setup Combat Test")]
    public static void SetupCombatTest()
    {
        // Ensure Player exists
        SetupPhase1(); // Ensures Player, Ground, Camera

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            // 1. Setup Weapon
            Transform weaponHolder = player.transform.Find("WeaponHolder");
            if (weaponHolder == null)
            {
                weaponHolder = new GameObject("WeaponHolder").transform;
                weaponHolder.SetParent(player.transform);
                weaponHolder.localPosition = new Vector3(0.5f, 0, 0.5f); // Offset to side
            }

            Transform sword = weaponHolder.Find("Sword");
            if (sword == null)
            {
                GameObject swordObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                swordObj.name = "Sword";
                swordObj.transform.SetParent(weaponHolder);
                swordObj.transform.localPosition = Vector3.zero;
                swordObj.transform.localScale = new Vector3(0.1f, 0.1f, 1.5f); // Long stick
                sword = swordObj.transform;
                
                // Remove default collider of primitive to replace or keep? 
                // Weapon script requires a collider. Primitive has one.
                // Ensure it is trigger.
                Collider col = swordObj.GetComponent<Collider>();
                if (col != null) col.isTrigger = true;
            }
            
            // Add Weapon Component
            Combat.Weapon weaponComp = sword.GetComponent<Combat.Weapon>();
            if (weaponComp == null)
            {
                weaponComp = sword.gameObject.AddComponent<Combat.Weapon>();
            }

            // Link Weapon to PlayerController
            var controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                SerializedObject so = new SerializedObject(controller);
                so.Update();
                SerializedProperty weaponProp = so.FindProperty("weapon");
                if (weaponProp != null)
                {
                    weaponProp.objectReferenceValue = weaponComp;
                    so.ApplyModifiedProperties();
                }
            }
        }

        // 2. Setup Sandbag
        GameObject sandbag = GameObject.Find("Sandbag");
        if (sandbag == null)
        {
            sandbag = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            sandbag.name = "Sandbag";
            sandbag.transform.position = new Vector3(2, 1, 2); // Near player
        }
        
        if (!sandbag.GetComponent<Combat.Sandbag>())
        {
            sandbag.AddComponent<Combat.Sandbag>();
        }

        Debug.Log("Combat Test Scene Setup Complete!");
    }

    [MenuItem("Tools/Edo Highway/Play Test")]
    public static void TestPlayMode()
    {
        Debug.Log("Starting Play Mode Test...");
        EditorApplication.isPlaying = true;
    }
    [MenuItem("Tools/Edo Highway/Force Fix Camera")]
    public static void ForceFixCamera()
    {
        // 1. Destroy existing camera to reset state
        GameObject vcamObj = GameObject.Find("CM_PlayerCam");
        if (vcamObj != null)
        {
            DestroyImmediate(vcamObj);
        }

        // 2. Re-run setup
        SetupPhase1();
        
        // 3. Ensure Main Camera Brain
        Camera cam = Camera.main;
        if (cam != null)
        {
            var brain = cam.GetComponent<CinemachineBrain>();
            if (brain == null) cam.gameObject.AddComponent<CinemachineBrain>();
        }

        Debug.Log("Camera Force Reset Complete. Please check Game View.");
    }

    [MenuItem("Tools/Edo Highway/Setup Hex Grid")]
    public static void SetupHexGrid()
    {
        // 1. Find or Create Manager
        GameObject managerObj = GameObject.Find("HexGridManager");
        if (managerObj == null)
        {
            managerObj = new GameObject("HexGridManager");
        }
        
        Grid.HexGridManager manager = managerObj.GetComponent<Grid.HexGridManager>();
        if (manager == null)
            manager = managerObj.AddComponent<Grid.HexGridManager>();

        // 2. Prepare Tile Prefab
        // Unlike setup scripts, we need a prefab asset or a scene object to clone.
        // Let's create a temporary primitive in scene, make it a child of Manager, and use it as template?
        // Or better: Create a rudimentary prefab in Resources if possible?
        // For simplicity: Create a "TileTemplate" object in scene, hide it, and use it.
        
        Transform templateTr = managerObj.transform.Find("TileTemplate");
        Grid.HexTile tileTemplate;
        
        if (templateTr == null)
        {
            GameObject templateObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            templateObj.name = "TileTemplate";
            templateObj.transform.SetParent(managerObj.transform);
            
            // Hex shape approximation: Cylinder scaled Y is flat
            templateObj.transform.localScale = new Vector3(1, 0.1f, 1); 
            
            // Add component
            tileTemplate = templateObj.AddComponent<Grid.HexTile>();
            
            // Disable mesh? No, we want to copy it.
            // Just deactivate the template object so it doesn't show up at (0,0)
            templateObj.SetActive(false); 
        }
        else
        {
            tileTemplate = templateTr.GetComponent<Grid.HexTile>();
        }

        // 3. Assign to Manager via SerializedObject (since field is private/serialized)
        SerializedObject so = new SerializedObject(manager);
        so.Update();
        SerializedProperty prefabProp = so.FindProperty("tilePrefab");
        if (prefabProp != null)
        {
            prefabProp.objectReferenceValue = tileTemplate;
        }
        so.ApplyModifiedProperties();

        // 4. Generate
        manager.GenerateGrid();
        
        Debug.Log("Hex Grid Setup & Generation Complete!");
    }

    [MenuItem("Tools/Edo Highway/Setup Health Bars")]
    public static void SetupHealthBars()
    {
        // Setup for Player and Sandbag
        SetupUnitHealthBar("Player");
        SetupUnitHealthBar("Sandbag");
        
        Debug.Log("Health Bar Setup Complete!");
    }

    private static void SetupUnitHealthBar(string targetName)
    {
        GameObject target = GameObject.Find(targetName);
        if (target == null) return;

        Combat.DamageableObject damageable = target.GetComponent<Combat.DamageableObject>();
        if (damageable == null) return;

        // Check if existing canvas
        Transform existingCanvas = target.transform.Find("HealthCanvas");
        if (existingCanvas != null) DestroyImmediate(existingCanvas.gameObject);

        // Create Canvas (World Space)
        GameObject canvasObj = new GameObject("HealthCanvas");
        canvasObj.transform.SetParent(target.transform);
        canvasObj.transform.localPosition = new Vector3(0, 2.2f, 0); // Above head
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // Scale down huge canvas
        RectTransform rect = canvasObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f, 40f); // 200px wide, 40px high
        canvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 1f); // Scale down 100x

        // Add BG (Black)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = Color.black;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Add Damage Fill (Red)
        GameObject dmgObj = new GameObject("DamageFill");
        dmgObj.transform.SetParent(canvasObj.transform, false);
        Image dmgImg = dmgObj.AddComponent<Image>();
        dmgImg.color = Color.red;
        dmgImg.type = Image.Type.Filled;
        dmgImg.fillMethod = Image.FillMethod.Horizontal;
        RectTransform dmgRect = dmgObj.GetComponent<RectTransform>();
        dmgRect.anchorMin = Vector2.zero;
        dmgRect.anchorMax = Vector2.one;
        dmgRect.offsetMin = Vector2.zero;
        dmgRect.offsetMax = Vector2.zero;

        // Add Health Fill (Green)
        GameObject hpObj = new GameObject("HealthFill");
        hpObj.transform.SetParent(canvasObj.transform, false);
        Image hpImg = hpObj.AddComponent<Image>();
        hpImg.color = Color.green;
        hpImg.type = Image.Type.Filled; // Ensure Filled
        hpImg.fillMethod = Image.FillMethod.Horizontal;
        RectTransform hpRect = hpObj.GetComponent<RectTransform>();
        hpRect.anchorMin = Vector2.zero;
        hpRect.anchorMax = Vector2.one;
        hpRect.offsetMin = Vector2.zero;
        hpRect.offsetMax = Vector2.zero;

        // Add Logic Script to Canvas (or separate object, but canvas works)
        UI.HealthBarUI uiScript = canvasObj.AddComponent<UI.HealthBarUI>();
        
        // Assign References via SerializedObject
        SerializedObject so = new SerializedObject(uiScript);
        so.Update();
        so.FindProperty("healthFill").objectReferenceValue = hpImg;
        so.FindProperty("damageFill").objectReferenceValue = dmgImg;
        so.FindProperty("target").objectReferenceValue = damageable;
        so.ApplyModifiedProperties();
        
        Debug.Log($"Attached Health Bar to {targetName}");
    }
}
