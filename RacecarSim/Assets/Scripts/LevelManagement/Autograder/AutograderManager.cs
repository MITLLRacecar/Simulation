using System.Collections.Generic;
using UnityEngine;

public class AutograderManager : MonoBehaviour
{
    #region Set in Unity Editor
    [SerializeField]
    private AutograderTask[] tasks = new AutograderTask[1];
    #endregion

    #region Constants
    public const int AutograderSummaryBuildIndex = 25;
    #endregion

    #region Public Interface
    public static readonly List<AutograderLevelScore> levelScores = new List<AutograderLevelScore>();
   
    public static void ResetAutograder()
    {
        AutograderManager.levelIndex = 0;
        AutograderManager.levelScores.Clear();
    }

    public static void CompleteTask(AutograderTask task)
    {
        if (AutograderManager.CurTask == task)
        {
            AutograderManager.instance.levelScore += task.Points;
            AutograderManager.instance.hud.UpdateScore(AutograderManager.instance.levelScore, AutograderManager.LevelInfo.MaxPoints);

            task.Disable();
            AutograderManager.instance.taskIndex++;
            if (AutograderManager.instance.taskIndex >= AutograderManager.instance.tasks.Length)
            {
                AutograderManager.instance.FinishLevel();
            }
            else
            {
                AutograderManager.CurTask.Enable();
            }
        }
    }

    public void HandleStart(IAutograderHud hud)
    {
        this.startTime = Time.time;
        this.hud = hud;
        this.hud.SetLevelInfo(AutograderManager.levelIndex, AutograderManager.LevelInfo.Title, AutograderManager.LevelInfo.Description);
        this.hud.UpdateScore(this.levelScore, AutograderManager.LevelInfo.MaxPoints);
        this.hud.UpdateTime(0, AutograderManager.LevelInfo.TimeLimit);
    }

    public void HandleError()
    {
        AutograderManager.levelScores.Add(new AutograderLevelScore()
        {
            Score = this.levelScore,
            Time = Time.time - this.startTime ?? Time.time
        });
    }

    public void HandleFailure()
    {
        this.FinishLevel();
    }
    #endregion

    private static AutograderManager instance;

    private static int levelIndex = 0;

    private IAutograderHud hud;

    private float? startTime = null;

    private int taskIndex = 0;

    private float levelScore = 0;

    private static AutograderLevelInfo LevelInfo { get { return LevelManager.LevelInfo.AutograderLevels[AutograderManager.levelIndex]; } }

    private static AutograderTask CurTask { get { return AutograderManager.instance.tasks[AutograderManager.instance.taskIndex]; } }

    private void Awake()
    {
        AutograderManager.instance = this;
    }

    private void Start()
    {
        this.tasks[taskIndex].Enable();
    }

    private void Update()
    {
        if (this.startTime.HasValue)
        {
            float elapsedTime = Time.time - this.startTime.Value;
            this.hud.UpdateTime(elapsedTime, AutograderManager.LevelInfo.TimeLimit);
            if (elapsedTime > AutograderManager.LevelInfo.TimeLimit)
            {
                this.FinishLevel();
            }
        }
    }

    private void FinishLevel()
    {
        AutograderManager.levelScores.Add(new AutograderLevelScore()
        { 
            Score = this.levelScore,
            Time = Time.time - this.startTime ?? Time.time
        });

        if (AutograderManager.levelIndex == LevelManager.LevelInfo.AutograderLevels.Length - 1)
        {
            LevelManager.FinishAutograder();
        }
        else
        {
            AutograderManager.levelIndex++;
            LevelManager.NextAutograderLevel();
        }
    }
}
