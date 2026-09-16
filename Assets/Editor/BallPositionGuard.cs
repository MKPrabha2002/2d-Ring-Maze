#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// The ultimate safeguard. 
/// No matter what happens in play mode, this script forcefully restores the Ball
/// to EXACTLY the GameComponents parent at EXACTLY x: 2.78 local position
/// the moment you return to Edit mode.
/// </summary>
[InitializeOnLoad]
public static class BallPositionGuard
{
    static BallPositionGuard()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // DO NOT modify Rigidbody2D in ExitingPlayMode!
        // Doing so forces Unity's physics engine to sync the transform, permanently overwriting the saved Edit Mode position.
        
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Delay call to wait for Unity to fully reload the scene
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    var ball = GameObject.FindWithTag("Player");
                    if (ball == null) ball = GameObject.Find("Ball");

                    if (ball != null)
                    {
                        // 0. Restore the exact intended position as promised by this script
                        ball.transform.localPosition = new Vector3(2.78f, 0f, 0f);

                        // 1. Force Rigidbody state in the editor so it doesn't fight the transform next play
                        var rb = ball.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.simulated = false;
                            rb.bodyType = RigidbodyType2D.Kinematic;
                        }

                        // Mark as dirty so Unity knows we fixed it
                        EditorUtility.ClearDirty(ball);
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(ball.scene);
                    }
                };
            };
        }
    }
}
#endif
