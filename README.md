# BuildTimeDisplay - VRChatワールド用ビルド時刻表示

ワールドをビルドした時刻を自動で記録し、「○分前にビルド」のように表示する看板を簡単に設置できるUdonSharpパッケージです。

## インストール

1. このフォルダ（`BuildTimeDisplay`）をUnityプロジェクトの`Assets`フォルダにコピー
2. VRChat SDK3とUdonSharpがインストールされていることを確認

## 使い方

### 方法1: 既存のTextMeshProに追加

1. シーン内のTextMeshProオブジェクトを選択
2. `Add Component` → `BuildTimeDisplay` を追加
3. ビルドすると自動でタイムスタンプが設定されます

### 方法2: Prefabを作成して使い回す

1. 空のGameObjectを作成
2. TextMeshProコンポーネントを追加
3. BuildTimeDisplayコンポーネントを追加
4. 見た目を調整してPrefab化
5. 好きな場所に配置

## 設定項目

| 項目 | 説明 | デフォルト |
|------|------|-----------|
| Update Interval | 表示更新間隔（秒） | 1 |
| Display Format | 表示フォーマット（{0}に経過時間） | `ビルド: {0}前` |
| Hour Format | 1時間以上の表示 | `{0}時間{1}分` |
| Minute Format | 1分以上1時間未満の表示 | `{0}分` |
| Second Format | 1分未満の表示 | `{0}秒` |

## 仕組み

1. **Editor拡張** (`BuildTimeStamper.cs`)
   - VRChat SDK の `IVRCSDKBuildRequestedCallback` を実装
   - ビルド開始時にシーン内の全 `BuildTimeDisplay` を検索
   - 現在時刻をUnixタイムスタンプとして注入

2. **ランタイム** (`BuildTimeDisplay.cs`)
   - UdonSharpで動作
   - 毎秒（設定可能）現在時刻とビルド時刻の差分を計算
   - TextMeshPro/TextMeshProUGUIに経過時間を表示

## 表示例

- ビルド直後: `ビルド: 5秒前`
- 数分後: `ビルド: 3分前`
- 数時間後: `ビルド: 2時間15分前`
- 翌日以降: `ビルド: 1日5時間前`

## 注意事項

- TextMeshProまたはTextMeshProUGUIコンポーネントが同じGameObjectに必要です
- ビルド時刻はUTC基準で保存され、表示時にローカル時間と比較されます
- Test Build / Build & Publish どちらでもタイムスタンプが更新されます

## ライセンス

MIT License
