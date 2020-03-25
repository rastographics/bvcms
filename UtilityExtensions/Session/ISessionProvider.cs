using System.Collections.Generic;

namespace UtilityExtensions.Session
{
    public interface ISessionProvider
    {
        T Get<T>(string name) where T : class;
        T Get<T>(string name, T defaultValue) where T : class;
        void Add<T>(string name, T value) where T : class;
        void Delete(string name);
        void Clear();
        IEnumerable<string> Keys { get; }
    }
}
