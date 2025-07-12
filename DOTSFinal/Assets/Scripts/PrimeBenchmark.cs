using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PrimeBenchmark : MonoBehaviour
{
    [SerializeField] private long numberToTest = 2147483647;

    void Start()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        bool regular = IsPrimeRegular(numberToTest);
        sw.Stop();
        Debug.Log($"Regular took: {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        bool jobs = IsPrimeJobs(numberToTest);
        sw.Stop();
        Debug.Log($"Jobs took: {sw.ElapsedMilliseconds} ms");
        Debug.Log($"{numberToTest} is {(jobs ? "PRIME ✔" : "NOT PRIME ✘")}");
    }

    static bool IsPrimeRegular(long n)
    {
        if (n <= 1) return false;
        if (n % 2 == 0) return n == 2;
        long limit = (long)math.sqrt(n);
        for (long i = 3; i <= limit; i += 2)
            if (n % i == 0) return false;
        return true;
    }

    static bool IsPrimeJobs(long n)
    {
        if (n <= 1) return false;
        if (n % 2 == 0) return n == 2;

        long limit = (long)math.sqrt(n);
        int length = (int)(limit - 1);

        using var perIndex = new NativeArray<byte>(length, Allocator.TempJob);
        using var primeFlag = new NativeArray<byte>(1, Allocator.TempJob);

        var checkHandle = new DivisorCheckJob { number = n, results = perIndex }.Schedule(length, 64);
        var reduceHandle = new ReduceJob { results = perIndex, isPrime = primeFlag }.Schedule(checkHandle);
        reduceHandle.Complete();

        return primeFlag[0] != 0;
    }

    [BurstCompile]
    struct DivisorCheckJob : IJobParallelFor
    {
        public long number;
        [WriteOnly] public NativeArray<byte> results;

        public void Execute(int index)
        {
            long divisor = index + 2;
            results[index] = (number % divisor == 0) ? (byte)1 : (byte)0;
        }
    }

    [BurstCompile]
    struct ReduceJob : IJob
    {
        [ReadOnly] public NativeArray<byte> results;
        public NativeArray<byte> isPrime;

        public void Execute()
        {
            for (int i = 0; i < results.Length; i++)
                if (results[i] != 0) {
                    isPrime[0] = 0; 
                    return;
                }

            isPrime[0] = 1;
        }
    }
}
