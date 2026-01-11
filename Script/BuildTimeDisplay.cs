using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;

/// <summary>
/// ビルド時刻からの経過時間を表示するUdonSharpスクリプト
/// Editor拡張によってビルド時に buildTimestamp が自動設定されます
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BuildTimeDisplay : UdonSharpBehaviour
{
    [Header("設定")]
    [Tooltip("表示を更新する間隔（秒）")]
    public float updateInterval = 1f;
    
    [Tooltip("表示フォーマット（{0}にビルド日時、{1}に経過時間が入ります）")]
    [TextArea(2, 4)]
    public string displayFormat = "最後のビルド {0}\n{1}前";

    [Tooltip("1日以上経過時のフォーマット")]
    public string dayFormat = "{0}日{1}時間";

    [Tooltip("1時間以上経過時のフォーマット")]
    public string hourFormat = "{0}時間{1}分";

    [Tooltip("1時間未満のフォーマット")]
    public string minuteFormat = "{0}分";

    [Tooltip("1分未満のフォーマット")]
    public string secondFormat = "{0}秒";

    [Header("自動設定（Editor拡張が設定）")]
    [Tooltip("ビルド時のUnixタイムスタンプ（秒）- Editorが自動設定")]
    public double buildTimestamp = 0;
    
    [Tooltip("ビルド時刻の文字列表示")]
    public string buildTimeString = "未ビルド";
    
    // 内部変数
    private TextMeshPro _textMeshPro;
    private TextMeshProUGUI _textMeshProUGUI;
    private float _nextUpdateTime;
    
    void Start()
    {
        // TextMeshProコンポーネントを取得
        _textMeshPro = GetComponent<TextMeshPro>();
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        
        // 即座に更新
        UpdateDisplay();
    }
    
    void Update()
    {
        if (Time.time >= _nextUpdateTime)
        {
            UpdateDisplay();
            _nextUpdateTime = Time.time + updateInterval;
        }
    }
    
    private void UpdateDisplay()
    {
        string text = FormatElapsedTime();
        
        if (_textMeshPro != null)
        {
            _textMeshPro.text = text;
        }
        
        if (_textMeshProUGUI != null)
        {
            _textMeshProUGUI.text = text;
        }
    }
    
    private string FormatElapsedTime()
    {
        if (buildTimestamp <= 0)
        {
            return "ビルド時刻未設定";
        }

        double currentUnixTime = GetCurrentUnixTime();
        double elapsedSeconds = currentUnixTime - buildTimestamp;

        if (elapsedSeconds < 0)
        {
            return "時刻同期中...";
        }

        string elapsed = FormatDuration(elapsedSeconds);
        return string.Format(displayFormat, buildTimeString, elapsed);
    }
    
    private double GetCurrentUnixTime()
    {
        // UdonSharpではSystem.DateTimeが使えるが制限あり
        // VRChatのネットワーク時間を使う方法もあるが、シンプルにローカル時間を使用
        var now = System.DateTime.UtcNow;
        var epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (now - epoch).TotalSeconds;
    }
    
    private string FormatDuration(double totalSeconds)
    {
        int seconds = (int)totalSeconds;
        int minutes = seconds / 60;
        int hours = minutes / 60;
        int days = hours / 24;

        if (days > 0)
        {
            int remainingHours = hours % 24;
            return string.Format(dayFormat, days, remainingHours);
        }
        else if (hours > 0)
        {
            int remainingMinutes = minutes % 60;
            return string.Format(hourFormat, hours, remainingMinutes);
        }
        else if (minutes > 0)
        {
            return string.Format(minuteFormat, minutes);
        }
        else
        {
            return string.Format(secondFormat, seconds);
        }
    }
}
