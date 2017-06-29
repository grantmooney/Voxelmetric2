using System.Threading;

public class Utils {
    public static void ProfileCall(ThreadStart threadStart, string sampleName)
    {
        UnityEngine.Profiling.Profiler.BeginSample(sampleName);
        threadStart.DynamicInvoke();
        UnityEngine.Profiling.Profiler.EndSample();
    }
}


