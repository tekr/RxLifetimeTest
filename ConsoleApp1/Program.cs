
// See https://aka.ms/new-console-template for more information

using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ConsoleApp1;

public class Program
{
    private static WeakReference<IDisposable> Sub1WeakRef;
    private static WeakReference<IDisposable> Sub2WeakRef;
    private static WeakReference<IDisposable> Sub3WeakRef;
    private static WeakReference<IDisposable> Sub4WeakRef;

    private static IDisposable sub2Ref;
    
    public static int Main(string[] args)
    {
        Subscribe();
        var i = 0;

        while (true)
        {
            Console.WriteLine(
                $"** Liveness - Sub 1: {Sub1WeakRef.TryGetTarget(out _)} Sub 2: {Sub2WeakRef.TryGetTarget(out _)} " +
                $"Sub 3: {Sub3WeakRef.TryGetTarget(out _)} Sub 4: {Sub4WeakRef.TryGetTarget(out _)}");

            GC.Collect(2);
            Thread.Sleep(1000);

            if (i++ == 6)
            {
                Console.WriteLine("** Disposing of Sub1");
                
                if (Sub1WeakRef.TryGetTarget(out var sub1))
                {
                    sub1.Dispose();
                }
            }
        }
    }

    private static void Subscribe()
    {
        var sub1 = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(o => Console.WriteLine("Sub 1"));

        sub2Ref = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(o => Console.WriteLine("Sub 2"));

        var sub3 = Observable.Timer(TimeSpan.FromSeconds(10))
            .Subscribe(o => Console.WriteLine("Sub 3"));

        var sub4 = Observable.Range(0, int.MaxValue)
            .SubscribeOn(NewThreadScheduler.Default).Subscribe( o =>
            {
                Console.WriteLine("Sub 4");
                Thread.Sleep(1000);
            });

        Sub1WeakRef = new WeakReference<IDisposable>(sub1);
        Sub2WeakRef = new WeakReference<IDisposable>(sub2Ref);
        Sub3WeakRef = new WeakReference<IDisposable>(sub3);
        Sub4WeakRef = new WeakReference<IDisposable>(sub4);
    }
}