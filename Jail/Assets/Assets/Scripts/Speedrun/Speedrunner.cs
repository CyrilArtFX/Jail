using Jail.UI.Glyphs;
using Jail.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jail.Speedrun
{
    public class Speedrunner : MonoBehaviour
    {
        public static Speedrunner instance;

        public bool IsRunning => isRunning;
        public bool HasStarted => hasStarted;
        public bool IsPersistent { get; protected set; }

        public double TotalTime => accumulationTime + levelTimer.CurrentTime;

        [SerializeField]
        TMP_Text totalTimeTMP;
        [SerializeField]
        SpeedrunTimer levelTimer, zoneTimer;

        [SerializeField]
        List<SpeedrunComment> commentsOverTime;
        [SerializeField]
        string defaultComment;

        double accumulationTime = 0.0d;

        bool hasStarted = false;
        bool isRunning = false;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Start()
        {
            SetPause(true);
        }

        void Update()
        {
            if (isRunning)
            {
                VariousUtility.SetTMPTime(totalTimeTMP, TotalTime);
            }
        }

        public void MakePersistent()
        {
            if (IsPersistent) return;

            DontDestroyOnLoad(gameObject);
            IsPersistent = true;
        }

        public void UndoPersistent()
        {
            if (!IsPersistent) return;

            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            IsPersistent = false;
        }

        public void StartRun()
        {
            //  reset timers
            levelTimer.ResetTime();
            zoneTimer.ResetTime();
            accumulationTime = 0.0d;

            //  validate run
            SpeedrunMessager.instance.SetVisible(false);

            //  run!
            hasStarted = true;
            SetPause(false);
            print("Speedrun: starting run!");
        }

        public void EndRun(bool is_valid = true)
        {
            if (is_valid)
			{
                //  comment the run
                if (!SpeedrunMessager.instance.IsVisible)
                {
                    string text = defaultComment;
                    foreach (SpeedrunComment comment in commentsOverTime)
                    {
                        if (TotalTime <= comment.Time)
                        {
                            text = comment.Text;
                            break;
                        }
                    }

                    SpeedrunMessager.instance.SetMessage(text, true);
                }
			}
            else
			{
                SpeedrunMessager.instance.SetMessage("Invalid!", false);
			}

            hasStarted = false;
            SetPause(true);
            print("Speedrun: ending run!");
        }

        public void SetPause(bool paused)
        {
            levelTimer.enabled = !paused;
            zoneTimer.enabled = !paused;

            isRunning = !paused;
        }

        public void SplitLevelTime()
        {
            accumulationTime += levelTimer.CurrentTime;
            levelTimer.SplitTime();

            print("Speedrun: split level time!");
        }

        public void SplitZoneTime()
        {
            zoneTimer.SplitTime();

            print("Speedrun: split zone time!");
        }

        public void RevertZoneTime()
        {
            levelTimer.CurrentTime -= zoneTimer.CurrentTime;
            zoneTimer.CurrentTime = 0.0d;
        }
    }
}