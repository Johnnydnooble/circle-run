using GameAnalyticsSDK;
using Voodoo.Sauce.Internal;


public static class TinySauce
{ 
    public const string Version = "3.1.0";
    /// <summary>
    ///  Method to call whenever the user starts a game.
    /// </summary>
    /// <param name="levelNumber">The game Level, this parameter is optional for game without level</param>
    public static void OnGameStarted(string  levelNumber = "game")
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start,levelNumber);
    }

    /// <summary>
    /// Method to call whenever the user completes a game.
    /// </summary>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(float score)
    {
        OnGameFinished("game",  score);
    }
    
    /// <summary>
    /// Method to call whenever the user completes a game with levels.
    /// </summary>
    /// <param name="levelNumber">The game Level</param>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(string  levelNumber, float score)
    {
        OnGameFinished(levelNumber,true, score);
    }
    
    
    /// <summary>
    /// Method to call whenever the user finishes a game, even when leaving a game.
    /// </summary>
    /// <param name="levelNumber">The game Level</param>
    /// <param name="levelComplete">Whether the user finished the game</param>
    /// <param name="score">The score of the game</param>
    public static void OnGameFinished(string  levelNumber, bool levelComplete, float score)
    {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (levelComplete)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelNumber, (int)score);
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelNumber, (int)score);
        }
    }

    /// <summary>
    /// Call this method to track any custom event you want.
    /// </summary>
    /// <param name="eventName">The name of the event to track</param>
    public static void TrackCustomEvent(string eventName)
    {
        GameAnalytics.NewDesignEvent(eventName);
    }
    
    /// <summary>
    /// Call this method to track any custom event you want.
    /// </summary>
    /// <param name="eventName">The name of the event to track</param>
    /// <param name="eventValue">Number value of event.</param>
    public static void TrackCustomEvent(string eventName, float eventValue)
    {
        GameAnalytics.NewDesignEvent(eventName, eventValue);
    }
    
    /// <summary>
    /// Call this method to track a custom event on Tenjin.
    /// Use this if you're sure what you are going to track.
    /// Useful when your game is not suited for "games played" events.
    /// </summary>
    /// <param name="eventName"></param>
    public static void TrackTenjinCustomEvent(string eventName)
    {
        Tenjin.getInstance(TinySettings.tenjinApiKey).SendEvent(eventName);
    }
    
}
