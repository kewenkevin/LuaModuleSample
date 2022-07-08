// 
// Copyright 2020 Yoozoo Net Inc.
// 
// UMT Framework and corresponding source code is free
// software: you can redistribute it and/or modify it under the terms of
// the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// UMT Framework and corresponding source code is distributed
// in the hope that it will be useful, but with permitted additional restrictions
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT
// distributed with this program. You should have received a copy of the
// GNU General Public License along with permitted additional restrictions
// with this program. If not, see https://gitlab.uuzu.com/yoozooopensource/client/framework/core
// 
// ***********************************************************************************************
// ***                  C O N F I D E N T I A L  ---  U M T   T E A M                          ***
// ***********************************************************************************************
// 
//     Project Name        :        UMT Framework Core Liberary
// 
//     File Name           :        TimeUtility.cs
// 
//     Programmer          :        Wei Wei (Battle Mage Gandalf)
// 
//     Start Date          :        04/07/2021
// 
//     Last Update         :        04/07/2021 16:30 [Wei]
// 
//     Description         :        write here
// 
// =============================================================================================
// Contributors:
// ---------------------------------------------------------------------------------------------
// Battle Mage Gandalf                 wwei@yoozoo.com             Product technology Center
// =============================================================================================

using System;

namespace Yoozoo.Core.Utility
{
    public static class TimeUtility
    {
        private static long serverTmOffset = 0;

        private static readonly DateTime UtcTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        private static readonly DateTime TimeZoneStartTime =
            TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));


        public const long HourTicks = 3600000;
        public const long DayTicks = 86400000;
        public const long MonthTicks = 2592000000;
        public const long YearTicks = 2592000000;


        public static long GetTime(
            this DateTime dt,
            int year = 0,
            int month = 0,
            int day = 0,
            int hour = 0,
            int minute = 0,
            int second = 0,
            int millisecond = 0)
        {
            dt.AddYears(year);
            dt.AddMonths(month);
            dt.AddDays(day);
            dt.AddHours(hour);
            dt.AddMinutes(minute);
            dt.AddSeconds(second);
            dt.AddMilliseconds(millisecond);

            TimeSpan ts = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }

        /// <summary>
        /// 根据时间戳获取时间对象
        /// </summary>
        /// <param name="timpStamp">时间戳</param>
        /// <returns></returns>
        public static DateTime GetFromStamp(long timpStamp)
        {
            long unixTime = timpStamp * (timpStamp >= 10000000000L ? 10000L : 10000000L);
            TimeSpan toNow = new TimeSpan(unixTime);
            return TimeZoneStartTime.Add(toNow);
        }


        /// <summary>
        /// 根据时间戳获取时间间隔对象
        /// </summary>
        /// <param name="timpStampFrom"></param>
        /// <param name="timeStampTo"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpan(long timpStampFrom, long timeStampTo)
        {
            DateTime dt0 = GetFromStamp(timpStampFrom);
            DateTime dt1 = GetFromStamp(timeStampTo);
            return dt1 - dt0;
        }


        /// <summary>
        /// 刷新服务器时间， 服务器下发时间后调用
        /// </summary>
        /// <param name="tm">服务器时间，时间戳 单位ms</param>
        public static void RefreshServerTm(long tm)
        {
            var clientTm = GetTimeStamp(false);
            serverTmOffset = tm - clientTm;
        }


        /// <summary>
        /// 获取当前模拟的服务器时间
        /// </summary>
        /// <returns>服务器时间，时间戳 单位ms</returns>
        public static long GetServerTm()
        {
            var clientTm = GetTimeStamp(false);
            return clientTm + serverTmOffset;
        }


        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="justSec">为真时获取10位时间戳,为假时获取13位时间戳.</param>
        /// <returns></returns>
        public static long GetTimeStamp(bool justSec = true)
        {
            TimeSpan ts = DateTime.UtcNow - UtcTime;
            return justSec ? Convert.ToInt64(ts.TotalSeconds) : Convert.ToInt64(ts.TotalMilliseconds);
        }


        public static string NormalizeTimpstamp0(long timpStamp)
        {
            return GetFromStamp(timpStamp).ToString("yyyy-MM-dd");
        }


        public static DateTime UtcTimeStampToDateTime(long timpStamp)
        {
            return TimeZoneStartTime.AddMilliseconds(timpStamp);
        }


        /// <summary>
        /// 时钟式倒计时
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string GetSecondString(int second)
        {
            return string.Format("{0:D2}", second / 3600) + string.Format("{0:D2}", second % 3600 / 60) + ":" +
                   string.Format("{0:D2}", second % 60);
        }


        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static System.DateTime ConvertIntDateTime(double d)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
            time = startTime.AddSeconds(d);
            return time;
        }


        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static double ConvertDateTimeInt(this DateTime time)
        {
            double intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return intResult;
        }


        /// <summary>
        /// 日期转换成unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(this DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }


        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="unixTimeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(this long timestamp, DateTimeKind kind)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, kind);
            return start.AddSeconds(timestamp);
        }
    }
}