using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Belp.Build.Test.MSBuild.XUnit.Resources;

/// <summary>
/// Manages the test projects and test caches.
/// </summary>
public static class TestProjectManager
{
    /// <summary>
    /// Provides common paths used by <see cref="TestProjectManager"/>.
    /// </summary>
    public static class Paths
    {
        private static string HexHash(ReadOnlySpan<byte> source)
        {
            const int HASH_SIZE = 256;
            Span<byte> buffer = stackalloc byte[HASH_SIZE / 8];
            _ = SHA256.HashData(source, buffer);
            Span<char> charBuffer = stackalloc char[HASH_SIZE / 8 * 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].TryFormat(charBuffer.Slice(i * 2, 2), out _, "X2"))
                {
                    throw new Exception("Unable to format byte into hexadecimal notation.");
                }
            }
            return charBuffer.ToString();
        }

        /// <summary>
        /// Gets the directory which contains the test projects.
        /// </summary>
        public static string TestProjectsRoot => AppContext.BaseDirectory;

        /// <summary>
        /// Gets the temporary directory where all caches are located.
        /// </summary>
        public static string TempRoot { get; } = Path.Combine(
            Path.GetTempPath(),
            "23bf55c5-7020-43d0-a313-9695fe6c313b",
            HexHash(MemoryMarshal.AsBytes(TestProjectsRoot.AsSpan()))
        );

        /// <summary>
        /// Gets the cache directory for cloned projects.
        /// </summary>
        public static string ProjectCache { get; } = Path.Combine(TempRoot, "projects");
    }

    private static readonly Dictionary<string, TestProject> InternalTestProjects;

    /// <summary>
    /// Gets a dictionary of test projects.
    /// </summary>
    public static IReadOnlyDictionary<string, TestProject> TestProjects => InternalTestProjects.AsReadOnly();

    static TestProjectManager()
    {
        if (Paths.TestProjectsRoot is null)
        {
            throw new InvalidProgramException($"{nameof(Paths)}.{nameof(Paths.TestProjectsRoot)} has not been set.");
        }

        string[] testProjectDirectories = Directory.GetDirectories(Paths.TestProjectsRoot);
        var testProjects = new Dictionary<string, TestProject>(testProjectDirectories.Length);
        for (int i = 0; i < testProjectDirectories.Length; i++)
        {
            string testProjectDirectory = testProjectDirectories[i];
            testProjects[Path.GetFileName(testProjectDirectory)] = new TestProject(testProjectDirectory);
        }

        InternalTestProjects = testProjects;
    }

    /// <summary>
    /// Deletes and recreates <see cref="Paths.TempRoot"/>.
    /// </summary>
    public static void ClearCache()
    {
        Directory.Delete(Paths.TempRoot, true);
        _ = Directory.CreateDirectory(Paths.TempRoot);
    }

    /// <summary>
    /// Deletes and recreates <see cref="Paths.ProjectCache"/>.
    /// </summary>
    public static void ClearProjectsCache()
    {
        Directory.Delete(Paths.ProjectCache, true);
        _ = Directory.CreateDirectory(Paths.ProjectCache);
    }
}
