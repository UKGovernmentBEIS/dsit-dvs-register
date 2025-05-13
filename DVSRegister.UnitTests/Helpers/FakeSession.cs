using Microsoft.AspNetCore.Http;

namespace DVSRegister.UnitTests.Helpers
{
    public class FakeSession : ISession
    {
        readonly Dictionary<string, byte[]> _store = new();

        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);

        // no-ops for the async/session lifecycle methods:
        public Task LoadAsync(CancellationToken cancellationToken = default)   => Task.CompletedTask;
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}