using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Permutations
{
    class Program
    {
        private const bool OutputEnabled = false;

        static void Main(string[] args)
        {
            Console.Write("Input:");

            var input = Console.ReadLine();

            var cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Press ESC to cancel");

            var keyTask = StartKeyboardTask(cancellationTokenSource);

            var processTask = StartProcessTask(input, cancellationTokenSource);

            try
            {
                Task.WaitAll(processTask, keyTask);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }

        private static Task StartKeyboardTask(CancellationTokenSource cancellationTokenSource)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) cancellationTokenSource.Cancel();
                    }
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Keyboard task ended");
                }
            });
        }

        private static Task StartProcessTask(string input, CancellationTokenSource cancellationTokenSource)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    Permutate(input, cancellationTokenSource.Token);

                    cancellationTokenSource.Cancel();
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        private static void Permutate(string input, CancellationToken cancellationToken)
        {
            var boundary = input.Length;

            long loops = Factorial(boundary) / boundary;

            var indexes = new List<int>(Enumerable.Range(0, boundary));

            Console.WriteLine($"{Factorial(boundary):##,###} permutations");
            Console.WriteLine($"Executing {loops:##,###} loops");

            var sw = new Stopwatch();

            sw.Start();

            for (long j = 1; j <= loops; j++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                for (var i = 0; i < boundary; i++)
                {
                    OutputIndexes(indexes);

                    OutputPermutation(input, indexes);

                    indexes = UpdateIndexes(boundary - 1, indexes, i);
                }
            }

            Console.WriteLine($"{sw.ElapsedMilliseconds}ms elapsed");
        }

        private static List<int> UpdateIndexes(long boundary, List<int> indexes, int index)
        {
            var swapfrom = 0;

            if (index < boundary) swapfrom = index + 1;

            var tmp = indexes[index];

            indexes[index] = indexes[swapfrom];

            indexes[swapfrom] = tmp;

            return indexes;
        }

        private static void OutputIndexes(IEnumerable<int> indexes)
        {
            if(!OutputEnabled) return;

            foreach (var index in indexes)
            {
                Console.Write($"{index}\t");
            }

            Console.WriteLine();
        }

        private static void OutputPermutation(string input, IEnumerable<int> indexes)
        {
            if (!OutputEnabled) return;

            Console.WriteLine(string.Join("", indexes.Select(index => input[index])));
        }

        private static long Factorial(long i)
        {
            if (i <= 1) return 1;

            return i * Factorial(i - 1);
        }
    }
}

//  a   b   c
//  0   1   2   abc
//  1   0   2   bac
//  1   2   0   bca
//  0   2   1   acb
//  2   0   1   cab
//  2   1   0   cba


//  a       b       c       d
//  0       1       2       3
//  1       0       2       3
//  1       2       0       3
//  1       2       3       0
//  0       2       3       1
//  2       0       3       1
//  2       3       0       1
//  2       3       1       0
//  0       3       1       2
//  3       0       1       2
//  3       1       0       2
//  3       1       2       0
//  0       1       2       3
//  1       0       2       3
//  1       2       0       3
//  1       2       3       0
//  0       2       3       1
//  2       0       3       1
//  2       3       0       1
//  2       3       1       0
//  0       3       1       2
//  3       0       1       2
//  3       1       0       2
//  3       1       2       0
