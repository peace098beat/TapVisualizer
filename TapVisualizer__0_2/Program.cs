using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TapVisualizer__0_2
{
    class Program
    {
        static void Main(string[] args)
        {

            int DebugNumber = 1000;

            for (int d = 0; d < DebugNumber; d++)
            {

                Console.WriteLine($"[{d}] Debug ");

                try
                {
                    TestRecording();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }

            Console.WriteLine($"end");
            Console.ReadKey();
        }

        static void TestRecording()
        {

            TapVisualizer.RecodingBuffer recbuffer = new TapVisualizer.RecodingBuffer(256, 44100, 16, 1);

            int N = TapVisualizer.RecodingBuffer.GetDeveiceCount();
            for (int i = 0; i < N; i++)
            {
                string DeviceName = TapVisualizer.RecodingBuffer.GetDeveceCapabilities(i);
            }

            recbuffer.Start(0);

            for (int i = 0; i < 100; i++)
            {
                float[] b = recbuffer.Buffer;
                int L = b.Length;

                for (int j = 0; j < L; j++)
                {
                }
            }

            recbuffer.Stop();
        }
    }
}
