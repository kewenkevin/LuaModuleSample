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
//     File Name           :        StringI18nExtension.cs
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


using ND.Core.ConstDefine;
using ND.Core.Enums;

namespace ND.Core.Extensions.CSharp
{
    /// <summary>
    /// Support for multi-language features，like sort by korean word or chinese pinyin
    /// </summary>
    public static class StringI18nExtension
    {
         #region Language Support

         /// <summary>
         /// get a string first char language type
         /// </summary>
         /// <param name="content">string</param>
         /// <returns>language type of first char</returns>
        public static LanguageType FirstCharLanguage(this string content)
        {
            if (string.IsNullOrEmpty(content)) return LanguageType.Other;

            char c = content.ToCharArray()[0];

            if (c >= 0x4e00 && c <= 0x9fbb) return LanguageType.CHN;
            if (c >= 0xac00 && c <= 0xD7AF) return LanguageType.KOR;
            if ((c >= 0x41 && c <= 0x5A) || (c >= 0x61 && c <= 0x7A)) return LanguageType.ENG;

            return LanguageType.Other;
        }

         /// <summary>
         /// get a string first char code， like: 你好(ni hao) return: N
         /// </summary>
         /// <param name="content">string</param>
         /// <returns>first char code of string</returns>
         public static string FirstCharCode(this string content)
        {
            switch (FirstCharLanguage(content))
            {
                case LanguageType.CHN:
                    return FirstPYCode(content).ToUpper();
                case LanguageType.KOR:
                    return FirstKORCode(content).ToUpper();
                case LanguageType.ENG:
                case LanguageType.Other:
                    return content.Substring(0, 1).ToUpper();
                default:
                    return string.Empty;
            }
        }



        /// <summary>
        /// get a string first korean spell code， like: 안녕하세요(hello) return: ㅇ (ar)
        /// </summary>
        /// <param name="content">string</param>
        /// <returns>first spell code of string</returns>
        public static string FirstKORCode(this string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            return content.ToCharArray()[0].GetKoreanSpell();
        }
        
        /// <summary>
        /// get a string first chinese pinyin spell code， like: 你好(ni hao) return: NH
        /// </summary>
        /// <param name="content">string</param>
        /// <returns>first spell code of string</returns>
        public static string FirstPYCode(this string content)
        {
            return content.Substring(0, 1).GetChineseSpell();
        }
        
        /// <summary>
        /// get a sortable spell string for asian language string. like:你好(ni hao) return: NH
        /// </summary>
        /// <param name="content">string</param>
        /// <returns>sortable string</returns>
        public static string CharSortCode(this string content)
        {
            string spellString = null;
            for (int i = 0; i < content.Length; i++)
                spellString = spellString + FirstCharCode(content.Substring(i, 1));
            return spellString;
        }

        /// <summary>
        /// get a spell code for Korean word
        /// </summary>
        /// <param name="content">char code</param>
        /// <returns>spell code</returns>
        public static string GetKoreanSpell(this char content)
        {
            if (content <= 0xac00) return string.Empty;
            else if (content <= 0xae4b) return "ㄱ";
            else if (content <= 0xb097) return "ㄲ";
            else if (content <= 0xb2e3) return "ㄴ";
            else if (content <= 0xb52f) return "ㄷ";
            else if (content <= 0xb77b) return "ㄸ";
            else if (content <= 0xb9c7) return "ㄹ";
            else if (content <= 0xbc13) return "ㅁ";
            else if (content <= 0xbe5f) return "ㅂ";
            else if (content <= 0xc0ab) return "ㅃ";
            else if (content <= 0xc2f7) return "ㅅ";
            else if (content <= 0xc543) return "ㅆ";
            else if (content <= 0xc78f) return "ㅇ";
            else if (content <= 0xc9db) return "ㅈ";
            else if (content <= 0xcc27) return "ㅉ";
            else if (content <= 0xce73) return "ㅊ";
            else if (content <= 0xd0bf) return "ㅋ";
            else if (content <= 0xd30b) return "ㅌ";
            else if (content <= 0xd557) return "ㅍ";
            else if (content <= 0xd7a3) return "ㅎ";
            return string.Empty;
        }

        /// <summary>
        /// get a spell code for Chinese word
        /// </summary>
        /// <param name="content">char code</param>
        /// <returns>spell code</returns>
        public static string GetChineseSpell(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;
            System.Text.StringBuilder spellString = new System.Text.StringBuilder();
            foreach (char vChar in content)
            {
                // 若是字母则直接输出
                if ((vChar >= 'a' && vChar <= 'z') || (vChar >= 'A' && vChar <= 'Z'))
                    spellString.Append(char.ToUpper(vChar));
                else if ((int) vChar >= 19968 && (int) vChar <= 40869)
                {
                    // 若字符Unicode编码在编码范围则 查汉字列表进行转换输出
                    foreach (string strList in StringConstDefine.ChineseCharList)
                    {
                        if (strList.IndexOf(vChar) > 0)
                        {
                            spellString.Append(strList[0]);
                            break;
                        }
                    }
                }
            }

            return spellString.ToString();
        }

        #endregion
    }
}