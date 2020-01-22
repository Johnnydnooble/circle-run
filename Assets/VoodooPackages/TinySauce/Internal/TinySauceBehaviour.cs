using System;
using Facebook.Unity;
using GameAnalyticsSDK;
using UnityEngine;

namespace Voodoo.Sauce.Internal
{
    internal class TinySauceBehaviour : MonoBehaviour

    {
        private const string Tag = "TinySauce";
        private static TinySauceBehaviour _instance;
        [SerializeField] private GameAnalytics gameAnalyticsPrefab;
        private TinySettings _settings;

        private void Awake()
        {

            if (transform != transform.root)
                throw new Exception("TinySauce prefab HAS to be at the ROOT level!");

            _settings = Resources.Load<TinySettings>("TinySauce/Settings");
            if (_settings == null)
                throw new Exception("Can't find TinySauce settings file.");

            VoodooLog.Initialize(VoodooLogLevel.WARNING);
            if (_instance != null)
            {
                VoodooLog.LogW(Tag, "TinySauce is already initialized!. This object will be destroyed: " + gameObject.name);
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);

            // init TinySauce sdk
            InitFacebook();
            InitGameAnalytics();
            InitTenjin();
        }
        
        private static void InitFacebook()
        {
            VoodooLog.Log(Tag, "Initializing Facebook");
            FB.Init(() => VoodooLog.Log(Tag, "Facebook Initialized"));
        }

        private static void InitTenjin()
        {
            VoodooLog.Log(Tag, "Initializing Tenjin");
            Tenjin.getInstance(TinySettings.tenjinApiKey).Connect();
        }

        private void InitGameAnalytics()
        {
            VoodooLog.Log(Tag, "Initializing GameAnalytics");
            var gameAnalyticsInstance = FindObjectOfType<GameAnalytics>();
            if (gameAnalyticsInstance == null)
            {
                gameAnalyticsInstance = Instantiate(gameAnalyticsPrefab);
                gameAnalyticsInstance.gameObject.SetActive(true);
            }
            GameAnalytics.Initialize();
        }



    private void OnApplicationPause(bool pauseStatus)
        {
            // Brought forward after soft closing 
            if (!pauseStatus)
            {
                InitTenjin();
            }
        }
    }
}
