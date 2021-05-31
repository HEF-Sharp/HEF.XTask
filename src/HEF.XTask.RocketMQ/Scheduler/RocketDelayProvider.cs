using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HEF.XTask.RocketMQ
{
    public interface IRocketDelayProvider
    {
        RocketDelay CreateRocketDelay(int delaySeconds);

        RocketDelay GetMinRocketDelay();

        RocketDelay GetNextRocketDelay(int delayLevel);
    }

    public class RocketDelayProvider : IRocketDelayProvider
    {
        private readonly SortedList<int, int> _rocketDelayTimeLevels = new SortedList<int, int>();

        //缓存 与 目标延迟时间 最接近的预定义延迟时间
        private readonly ConcurrentDictionary<int, int> _delayNearestTimeCache = new ConcurrentDictionary<int, int>();

        #region Constructor
        public RocketDelayProvider(params string[] messageDelayLevels)
        {
            if (messageDelayLevels == null || messageDelayLevels.Length == 0)
                throw new ArgumentNullException(nameof(messageDelayLevels));

            InitDelayTimeLevels(messageDelayLevels);
        }

        #region Init SortedDelayLevels
        private void InitDelayTimeLevels(params string[] messageDelayLevels)
        {
            for (int i = 0; i < messageDelayLevels.Length; i++)
            {
                var delaySeconds = ParseDelaySeconds(messageDelayLevels[i]);

                _rocketDelayTimeLevels.Add(delaySeconds, i + 1);
            }
        }

        private static int ParseDelaySeconds(string delayLevelStr)
        {
            if (string.IsNullOrWhiteSpace(delayLevelStr))
                return 0;

            return delayLevelStr switch
            {
                string delayTimeStr when delayTimeStr.EndsWith("s") => Convert.ToInt32(delayTimeStr.TrimEnd('s')),
                string delayTimeStr when delayTimeStr.EndsWith("m") => Convert.ToInt32(delayTimeStr.TrimEnd('m')) * 60,
                string delayTimeStr when delayTimeStr.EndsWith("h") => Convert.ToInt32(delayTimeStr.TrimEnd('h')) * 60 * 60,
                string delayTimeStr when delayTimeStr.EndsWith("d") => Convert.ToInt32(delayTimeStr.TrimEnd('d')) * 24 * 60 * 60,
                _ => throw new InvalidCastException("parse rocketmq message delay level failed"),
            };
        }
        #endregion

        #endregion

        public RocketDelay CreateRocketDelay(int delaySeconds)
        {
            if (delaySeconds < 1)
                return new RocketDelay();

            var (currentDelaySeconds, currentDelayLevel) = GetDelayTimeWithLevel(delaySeconds);

            if (currentDelayLevel == 0)  //延迟时间小于 所有预定义的延迟时间，发送即时消息
                return new RocketDelay();

            return new RocketDelay
            {
                DelayTimeLevel = currentDelayLevel,
                RemainDelaySeconds = delaySeconds - currentDelaySeconds
            };
        }

        public RocketDelay GetMinRocketDelay()
        {
            return new RocketDelay { DelayTimeLevel = _rocketDelayTimeLevels.Values[0] };
        }

        public RocketDelay GetNextRocketDelay(int delayLevel)
        {
            if (!_rocketDelayTimeLevels.ContainsValue(delayLevel))
                throw new InvalidOperationException("target message delay level is undefined");

            var currentIndex = _rocketDelayTimeLevels.IndexOfValue(delayLevel);
            var nextIndex = currentIndex + 1;
            if (nextIndex == _rocketDelayTimeLevels.Count)   //已经是最大的延迟时间
            {
                return new RocketDelay { DelayTimeLevel = delayLevel };
            }

            return new RocketDelay { DelayTimeLevel = _rocketDelayTimeLevels.Values[nextIndex] };
        }

        #region Helper Functions
        /// <summary>
        /// 获取 延迟时间和Level
        /// </summary>
        /// <returns></returns>
        private (int, int) GetDelayTimeWithLevel(int delaySeconds)
        {
            if (_rocketDelayTimeLevels.TryGetValue(delaySeconds, out var delayLevel))
            {
                return (delaySeconds, delayLevel);
            }

            var nearestDelaySeconds = _delayNearestTimeCache.GetOrAdd(delaySeconds, GetNearestLessThanDelayTime);

            if (nearestDelaySeconds > 0)
                return (nearestDelaySeconds, _rocketDelayTimeLevels[nearestDelaySeconds]);

            return (0, 0);  //目标延迟秒数 小于 所有预定义的延迟时间，发送即时消息
        }

        /// <summary>
        /// 获取小于目标延迟秒数 且与其最接近的预定义延迟时间
        /// </summary>
        /// <param name="delaySeconds"></param>
        /// <returns></returns>
        private int GetNearestLessThanDelayTime(int delaySeconds)
        {
            var maxLessThanDelaySecond = 0;

            foreach (var delayLevelItem in _rocketDelayTimeLevels)
            {
                if (delayLevelItem.Key >= delaySeconds)
                    break;

                maxLessThanDelaySecond = delayLevelItem.Key;
            }

            return maxLessThanDelaySecond;
        }
        #endregion
    }
}
