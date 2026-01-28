using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Controllers;
using Managers;

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
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        CameraFollow follow = cam.GetComponent<CameraFollow>();
        if (follow == null)
            follow = cam.gameObject.AddComponent<CameraFollow>();

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            follow.SetTarget(player.transform);
        }
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
}
