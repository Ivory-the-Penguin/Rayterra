using Raylib_cs;

namespace Rayterra.Core;

public static class Profiler
{
    public record struct Profile(float time);
    private record struct StackProfile(string name, float startTime);

    private static readonly Dictionary<string, Profile> _profiles = new();
    private static readonly Stack<StackProfile> _stack = new();

    public static void BeginProfile(string name)
    {
        _stack.Push(new StackProfile(name, (float)(Raylib.GetTime() * 1000)));
    }

    public static void EndProfile()
    {
        if (_stack.Count <= 0)
        {
            throw new InvalidOperationException("Profile stack is empty!");
        }

        StackProfile profile = _stack.Pop();

        float currentTime = (float)(Raylib.GetTime() * 1000);

        _profiles[profile.name] = new Profile(currentTime - profile.startTime);
    }

    public static Profile GetProfile(string name) => _profiles.GetValueOrDefault(name);
}

public class ProfilerScope : IDisposable
{
    public ProfilerScope(string name) => Profiler.BeginProfile(name);

    public void Dispose() => Profiler.EndProfile();
}