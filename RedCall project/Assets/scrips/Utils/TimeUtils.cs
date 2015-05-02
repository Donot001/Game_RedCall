using UnityEngine;
using System.Collections;

namespace RedCallFrame
{
    public class TimeUtils
    {
        public static float MFDeltaTime = 0;
        public static float LFDeltaTime = 0;

        public static void Update(float deltaTime)
        {
            MFDeltaTime += deltaTime;
            LFDeltaTime += deltaTime;
        }

        /// <summary>
        /// 保存时间同步时服务器下发的服务器时间
        /// </summary>
        static long serverTimeCache;
        /// <summary>
        /// 时间同步时的时间
        /// </summary>
        static float syncTime;

        /// <summary>
        /// 当前服务器时间， 服务器时间缓存 + 距离同步时的时间差就是现在服务器的时间,包括年月日
        /// </summary>
        public static long CurServerTime
        {
            get
            {
                //使用RealTime；
                return serverTimeCache + (long)(Time.realtimeSinceStartup - syncTime);
            }
        }

        /// <summary>
        /// 进入游戏後时间
        /// </summary>
        public static long DuringTime
        {
            get
            {
                return (long)(Time.realtimeSinceStartup - syncTime);
            }
        }

        public static void SyncServerTime(long serverTime)
        {
            //Log.Trace("SyncServerTime[" + serverTime + "]");
            serverTimeCache = serverTime;
            syncTime = Time.realtimeSinceStartup;
        }

        #region c#时间与java时间转换
        ///// <summary>
        ///// Java.currentTimeMillis
        ///// </summary>
        //public static long CurrentTimeMillis
        //{
        //    get
        //    {
        //        TimeSpan ts = new TimeSpan(System.DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
        //        return (long)ts.TotalMilliseconds;
        //    }
        //}
        #endregion
    }
}
