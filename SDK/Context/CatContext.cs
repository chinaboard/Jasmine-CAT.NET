using System.Collections.Generic;

namespace Framework.Zhaogang.Cat.Context
{
    public class CatContext : ContextBase
    {
        public Dictionary<string, string> Map = new Dictionary<string, string>();

        public override void AddProperty(string key, string value)
        {
            if (!Map.ContainsKey(key))
                Map.Add(key, value);
            else
                Map[key] = value;
        }

        public override string GetProperty(string key)
        {
            return Map[key];
        }
    }
}