using System;

namespace ND.UI
{
    public static class Utils
    {
        private static readonly string[] LuaKeyWorlds =
        {
            "and", "break", "do", "else", "elseif", "end", "false", "for",
            "function", "if", "in", "local", "nil", "not", "or", "repeat",
            "return", "then", "true", "until", "while", "goto"
        };
        public static bool CheckLuaKeyWorlds(string str)
        {
            foreach (var world in LuaKeyWorlds)
            {
                if (string.CompareOrdinal(str, world) == Decimal.Zero)
                {
                    return false;
                }
            }
            return true;
        }
    }
}