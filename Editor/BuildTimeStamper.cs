using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using VRC.SDKBase.Editor.BuildPipeline;
using System;

/// <summary>
/// VRChatワールドビルド時にBuildTimeDisplayコンポーネントにタイムスタンプを注入するEditor拡張
/// </summary>
public class BuildTimeStamper : IVRCSDKBuildRequestedCallback
{
    // 早めに実行（他のコールバックより前）
    public int callbackOrder => -100;

    public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
    {
        // ワールドビルド時のみ処理
        if (requestedBuildType == VRCSDKRequestedBuildType.Scene)
        {
            StampBuildTime();
        }
        
        // trueを返してビルドを続行
        return true;
    }

    private void StampBuildTime()
    {
        var now = DateTime.UtcNow;
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double unixTimestamp = (now - epoch).TotalSeconds;
        
        // ローカル時刻の文字列
        string timeString = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        
        // シーン内の全BuildTimeDisplayを検索
        var scene = SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();
        int count = 0;
        
        foreach (var root in rootObjects)
        {
            var displays = root.GetComponentsInChildren<BuildTimeDisplay>(true);
            
            foreach (var display in displays)
            {
                // Undo対応（エディタ上での変更を記録）
                Undo.RecordObject(display, "Stamp Build Time");
                
                display.buildTimestamp = unixTimestamp;
                display.buildTimeString = timeString;
                
                // 変更をマーク
                EditorUtility.SetDirty(display);
                
                count++;
                Debug.Log($"[BuildTimeStamper] タイムスタンプを設定: {display.gameObject.name} -> {timeString}");
            }
        }
        
        if (count > 0)
        {
            Debug.Log($"[BuildTimeStamper] {count}個のBuildTimeDisplayにビルド時刻を設定しました: {timeString}");
        }
    }
}
