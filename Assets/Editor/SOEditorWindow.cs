using UnityEditor;
using UnityEngine;

public class SOEditorWindow : EditorWindow
{
    // Enum을 UI 탭과 정확히 일치시킵니다.
    private enum EditorTab { GameSettings, Weapons, Player, Enemy, EnemyAttack }
    private EditorTab _currentTab = EditorTab.GameSettings;

    // 🌟 모든 SO의 부모인 ScriptableObject 배열로 일반화합니다.
    private ScriptableObject[] _assets;
    private ScriptableObject _selectedAsset;
    private Editor _assetEditor;

    private Vector2 _scrollPosList;
    private Vector2 _scrollPosDetails;

    [MenuItem("Tools/Player Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<SOEditorWindow>("플레이어 데이터 센터");
    }

    private void OnEnable()
    {
        LoadAssets();
    }

    private void LoadAssets()
    {
        string filter = "";
        switch (_currentTab)
        {
            case EditorTab.GameSettings: filter = "t:GameSettingsSO"; break;
            case EditorTab.Weapons: filter = "t:PlayerWeaponSO"; break;
            case EditorTab.Player: filter = "t:PlayerSO"; break;
            case EditorTab.Enemy: filter = "t:EnemySO "; break;
            case EditorTab.EnemyAttack: filter = "t:EnemyAttackSO"; break;
        }

        string[] guids = AssetDatabase.FindAssets(filter);
        _assets = new ScriptableObject[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            _assets[i] = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
        }
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        _currentTab = (EditorTab)GUILayout.Toolbar((int)_currentTab, new string[] { "게임 설정", "무기 설정", "플레이어", "몬스터", "몬스터 공격" }, GUILayout.Height(30));

        // 유저가 상단 탭을 클릭해서 바꾸었다면?
        if (EditorGUI.EndChangeCheck())
        {
            _selectedAsset = null;
            if (_assetEditor != null) DestroyImmediate(_assetEditor);
            LoadAssets(); // 해당 탭의 데이터로 새로고침!
        }

        EditorGUILayout.Space();

        // 메인 화면 분할 레이아웃
        EditorGUILayout.BeginHorizontal();
        DrawLeftPanel();
        DrawRightPanel();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLeftPanel()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(250), GUILayout.ExpandHeight(true));
        _scrollPosList = EditorGUILayout.BeginScrollView(_scrollPosList);

        if (GUILayout.Button("🔄 리스트 새로고침", GUILayout.Height(25)))
        {
            LoadAssets();
        }

        EditorGUILayout.Space();

        if (_assets != null)
        {
            foreach (var asset in _assets)
            {
                if (asset == null) continue;

                GUI.backgroundColor = (_selectedAsset == asset) ? Color.cyan : Color.white;

                if (GUILayout.Button(asset.name, GUILayout.Height(25)))
                {
                    _selectedAsset = asset;

                    if (_assetEditor != null) DestroyImmediate(_assetEditor);
                    _assetEditor = Editor.CreateEditor(_selectedAsset);
                }
                GUI.backgroundColor = Color.white;
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawRightPanel()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        _scrollPosDetails = EditorGUILayout.BeginScrollView(_scrollPosDetails);

        if (_selectedAsset != null && _assetEditor != null)
        {
            EditorGUILayout.LabelField($"📝 {_selectedAsset.name} 상세 설정", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 🌟 부모 타입인 ScriptableObject로 넘겨도 유니티 에디터가 실제 자식(HealthSO, LocomotionSO 등)의 인스펙터를 완벽하게 그려냅니다.
            _assetEditor.OnInspectorGUI();
        }
        else
        {
            EditorGUILayout.HelpBox("왼쪽 리스트에서 수정할 에셋을 선택해 주세요.", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}