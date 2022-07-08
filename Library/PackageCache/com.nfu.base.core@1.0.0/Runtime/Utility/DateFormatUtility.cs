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
//     File Name           :        DateFormatUtility.cs
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

namespace ND.Core.Utility
{
    public class DateFormatUtility
    {
        /// <summary>
        /// 大写一到十
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        private static string FormatCaseOneToTen(int dayNum)
        {
            switch (dayNum)
            {
                case 1:
                    return "One"; //TDLanguageTable.Get("One");
                case 2:
                    return "Two"; //TDLanguageTable.Get("Two");
                case 3:
                    return "Three"; //TDLanguageTable.Get("Three");
                case 4:
                    return "Four"; //TDLanguageTable.Get("Four");
                case 5:
                    return "Five"; //TDLanguageTable.Get("Five");
                case 6:
                    return "Six"; //TDLanguageTable.Get("Six");
                case 7:
                    return "Seven"; //TDLanguageTable.Get("Seven");
                case 8:
                    return "Eight"; //TDLanguageTable.Get("Eight");
                case 9:
                    return "Nine"; //TDLanguageTable.Get("Nine");
                case 10:
                    return "Ten"; //TDLanguageTable.Get("Ten");
            }

            return string.Empty;
        }

        /// <summary>
        /// 格式化输出  今天，明天， x天后，x天后， 从1开始
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        public static string FormatDateDayNum(int dayNum)
        {
            if (dayNum == 1)
            {
                return "今天"; //TDLanguageTable.Get("UI_Summon_Today");
            }
            else if (dayNum == 2)
            {
                return "明天"; //TDLanguageTable.Get("UI_Summon_Tomorrow");
            }
            else
            {
                return "{0}天后"; //TDLanguageTable.GetFormat("UI_Summon_Days", FormatCaseDayNum(dayNum - 1));
            }
        }

        /// <summary>
        /// 格式化天，大写 一，二，三，四，十一， 二十, 最大支持99, 从一开始
        /// </summary>
        /// <param name="dayNum"></param>
        /// <returns></returns>
        public static string FormatCaseDayNum(int dayNum)
        {
            int oneDigit = dayNum / 10;
            int twoDigit = dayNum % 10;
            if (dayNum > 10)
            {
                if (oneDigit == 1)
                {
                    return $"{FormatCaseOneToTen(10)}{FormatCaseOneToTen(twoDigit)}";
                }
                else
                {
                    if (twoDigit == 0)
                    {
                        return $"{FormatCaseOneToTen(oneDigit)}{FormatCaseOneToTen(10)}";
                    }
                    else
                    {
                        return $"{FormatCaseOneToTen(oneDigit)}{FormatCaseOneToTen(twoDigit)}";
                    }
                }
            }
            else
            {
                return $"{FormatCaseOneToTen(dayNum)}";
            }
        }

        /// <summary>
        /// 最大单位是天，输出1位(x天、x小时, x分, x秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatMaxUnitDayOutOne(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = "日"; //TDLanguageTable.Get("Day");
            string hoursFormat = "时"; //TDLanguageTable.Get("Hours");
            string minuteFormat = "分"; //TDLanguageTable.Get("Minute");
            string secondFormat = "秒"; //TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天
                return string.Format("{0}{1}", day, dayFormat);
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时
                    return string.Format("{0}{1}", hours, hoursFormat);
                }
                else
                {
                    if (minute >= 1)
                    {
                        // X分
                        return string.Format("{0}{1}", minute, minuteFormat);
                    }
                    else
                    {
                        // X秒
                        return string.Format("{0}{1}", second, secondFormat);
                    }
                }
            }
        }

        /// <summary>
        /// 最大单位是天，输出2位(x天y小时、x小时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatMaxUnitDayOutTwo(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = "日"; //TDLanguageTable.Get("Day");
            string hoursFormat = "时"; //TDLanguageTable.Get("Hours");
            string minuteFormat = "分"; //TDLanguageTable.Get("Minute");
            string secondFormat = "秒"; //TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天Y小时
                return $"{day}{dayFormat}{hours}{hoursFormat}";
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return $"{hours}{hoursFormat}{minute}{minuteFormat}";
                }
                else
                {
                    // X分Y秒
                    return $"{minute}{minuteFormat}{second}{secondFormat}";
                }
            }
        }

        /// <summary>
        /// 最大单位是天，输出2位(x天y小时、x小时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatMaxUnitTimeOutTwo(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);

            if (day >= 1)
            {
                // X天Y小时
                return $"{day}:{hours:D2}";
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return $"{hours:D2}:{minute:D2}";
                }
                else
                {
                    // X分Y秒
                    return $"{minute:D2}:{second:D2}";
                }
            }
        }

        /// <summary>
        /// 最大单位是天，
        ///     时间格式(大于1天) ：7天3时6分10秒
        ///     时间格式(小于1天) ：3时6分10秒
        ///     时间格式（小于1小时）：6分10秒
        ///     时间格式（小于1小时）：10秒
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatRemainTime(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = "日"; //TDLanguageTable.Get("Day");
            string hoursFormat = "时"; //TDLanguageTable.Get("Hours");
            string minuteFormat = "分"; //TDLanguageTable.Get("Minute");
            string secondFormat = "秒"; //TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // a天b时c分d秒
                return $"{day}{dayFormat}{hours}{hoursFormat}{minute}{minuteFormat}{second}{secondFormat}";
            }
            else
            {
                if (hours >= 1)
                {
                    // b时c分d秒
                    return $"{hours}{hoursFormat}{minute}{minuteFormat}{second}{secondFormat}";
                }
                else
                {
                    if (minute >= 1)
                    {
                        // c分d秒
                        return $"{minute}{minuteFormat}{second}{secondFormat}";
                    }
                    else
                    {
                        // d秒
                        return $"{second}{secondFormat}";
                    }
                }
            }
        }


        /// <summary>
        /// 最大单位是天，输出2位(x天y小时、x小时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatColorMaxUnitDayOutTwo(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = "日"; //TDLanguageTable.Get("Day");
            string hoursFormat = "时"; //TDLanguageTable.Get("Hours");
            string minuteFormat = "分"; //TDLanguageTable.Get("Minute");
            string secondFormat = "秒"; //TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天Y小时
                return $"[a1ee3e]{day}[-]{dayFormat}[a1ee3e]{hours}[-]{hoursFormat}";
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return $"[a1ee3e]{hours}[-]{hoursFormat}[a1ee3e]{minute}[-]{minuteFormat}";
                }
                else
                {
                    // X分Y秒
                    return $"[a1ee3e]{minute}[-]{minuteFormat}[a1ee3e]{second}[-]{secondFormat}";
                }
            }
        }

        /// <summary>
        /// 最大单位是天，输出2位(x天y时、x时y分、x分y秒）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string FormatColorMaxUnitDayOutTwoEx(long timestamp)
        {
            long day = timestamp / 86400;
            long hours = (timestamp % 86400) / 3600;
            long minute = (timestamp % 3600) / 60;
            long second = (timestamp % 60);
            string dayFormat = "日"; //TDLanguageTable.Get("Day");
            string hoursFormat = "时"; //TDLanguageTable.Get("Hours");
            string minuteFormat = "分"; //TDLanguageTable.Get("Minute");
            string secondFormat = "秒"; //TDLanguageTable.Get("Second");
            if (day >= 1)
            {
                // X天Y小时
                return $"[a1ee3e]{day}{dayFormat}{hours}{hoursFormat}[-]";
            }
            else
            {
                if (hours >= 1)
                {
                    // X小时Y分
                    return $"[a1ee3e]{hours}{hoursFormat}{minute}{minuteFormat}[-]";
                }
                else
                {
                    // X分Y秒
                    return $"[a1ee3e]{minute}{minuteFormat}{second}{secondFormat}[-]";
                }
            }
        }


        /// <summary>
        /// 输出格式化时间 00:00:00
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatTime(int seconds)
        {
            int hour = seconds / 3600;
            int min = (seconds % 3600) / 60;
            int sec = seconds % 60;
            return $"{hour:D2}:{min:D2}:{sec:D2}";
        }
    }
}