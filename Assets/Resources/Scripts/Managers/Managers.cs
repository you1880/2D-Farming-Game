using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers _instance;
    public static Managers Instance { get { Init(); return _instance; } }

    private AreaManager _area = new AreaManager();
    private DataManager _data = new DataManager();
    private GameManager _game = new GameManager();  
    private GameTimeManager _time = new GameTimeManager();
    private InputManager _input = new InputManager();
    private SoundManager _sound = new SoundManager();
    private PropManager _prop = new PropManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private TileManager _tile = new TileManager();
    private UIManager _ui = new UIManager();

    public static AreaManager Area { get { return Instance._area; } }
    public static DataManager Data { get { return Instance._data; } }
    public static GameManager Game { get { return Instance._game; } }
    public static GameTimeManager Time { get { return Instance._time; } }
    public static InputManager Input { get { return Instance._input; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static PropManager Prop { get { return Instance._prop; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static TileManager Tile { get { return Instance._tile; } }
    public static UIManager UI { get { return Instance._ui; } }

    private static bool _isApplicationQuitting = false;
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    private static void Init()
    {
        if (_isApplicationQuitting)
        {
            return;
        }

        if (_instance == null)
        {
            GameObject manager = GameObject.Find("@Manager");

            if (manager == null)
            {
                manager = new GameObject { name = "@Manager" };
                manager.AddComponent<Managers>();
            }

            DontDestroyOnLoad(manager);

            _instance = manager.GetComponent<Managers>();

            _instance._data.Init();
            _instance._sound.Init();
            _instance._resource.Init();
            _instance._tile.Init();
            _instance._ui.Init();
        }
    }

    public static Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    public static void TerminateCoroutine(Coroutine coroutine)
    {
        if (coroutine == null)
        {
            return;
        }

        Instance.StopCoroutine(coroutine);
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }
}
