using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapVisualizer
{
    /// <summary>
    /// レコーディング用クラス
    /// シングルトンにするか悩み中
    /// 
    /// </summary>
    public class RecodingBuffer : IDisposable
    {
        public float[] Buffer { get; private set; } // 32bitバッファー

        public int BufferLength { get; } = 512;
        public int FrameSamples { get; private set; } = 441000;
        public int Bits { get; private set; } = 16; // 実装の都合で16bit固定
        public int Channels { get; private set; } = 1;

        private NAudio.Wave.IWaveIn waveIn;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RecodingBuffer(int bufferLength = 512, int frameSamples = 44100, int bits = 16, int channels = 1)
        {
            this.BufferLength = bufferLength;
            this.FrameSamples = frameSamples;
            this.Bits = bits;
            this.Channels = channels;
            this.Buffer = new float[BufferLength];


        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        public void Dispose()
        {
            this.Clean();
        }

        /******************************************************/


        /// <summary>
        /// レコーディング開始
        /// </summary>
        /// <example href="https://stackoverrun.com/ja/q/3869314">
        /// デフォルトWaveInコンストラクタは、コールバックのWindowsメッセージを使用しています。
        /// ただし、コンソールアプリケーションやバックグラウンドスレッドを実行している場合、
        /// これらのWindowsメッセージは処理されません。最も簡単な解決策は、
        /// 
        /// 代わりにWaveInEventクラスを使用することです。
        /// waveInStream = new WaveInEvent(); 
        /// </example>
        public void Start(int deviceId)
        {

            // waveInインスタンスの生成
            try
            {
                //waveIn = new NAudio.Wave.WaveIn();
                waveIn = new NAudio.Wave.WaveInEvent
                {
                    DeviceNumber = deviceId,
                    WaveFormat = new NAudio.Wave.WaveFormat(FrameSamples, Bits, Channels),
                    NumberOfBuffers = 2,
                    BufferMilliseconds = (int)(BufferLength * 1f / FrameSamples * 1000f)
                };
            }
            catch (Exception ex)
            {
                this.Clean();
                throw ex;
            }

            // コールバックを追加
            waveIn.DataAvailable += OnDataAvailable;
            // 録音開始
            waveIn.StartRecording();

        }

        /// <summary>
        /// レコーディング停止
        /// </summary>
        public void Stop()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                this.Clean();
            }
        }

        /// <summary>
        /// インスタンスの開放
        /// </summary>
        private void Clean()
        {
            if (waveIn != null)
            {
                waveIn.DataAvailable -= OnDataAvailable;
            }
            waveIn?.StopRecording();
            waveIn?.Dispose();
            waveIn = null;
        }

        /// <summary>
        /// バッファリング
        /// </summary>
        /// <example>
        /// バイトから浮動少数に変換するのに16bitが楽なので、ビットレートは16bit固定の仕様とする.
        /// </example>
        private void OnDataAvailable(object s, NAudio.Wave.WaveInEventArgs e)
        {
            // 録音バッファ長さが0の場合は返却
            if (e.BytesRecorded == 0) return;


            // ビットレートは16bitを仕様とする.
            if (this.Bits != 16)
                throw new ArgumentException($"ビットレートは16bitにしてください. {Bits}bitになっています.");

            // 総サンプル数を計算
            int nSample = e.BytesRecorded / 2; // 1サンプル 2バイト

            // 整数から符号付浮動小数点に変換
            for (int i = 0; i < nSample; i++)
            {
                // 16bit用の演算
                short vShort = BitConverter.ToInt16(e.Buffer, 2 * i); // 2バイト限定
                float vFloat = (float)vShort / short.MaxValue;
                Buffer[i] = vFloat;
            }

        }

        /******************************************************/

        /// <summary>
        /// デバイス数を返却
        /// </summary>
        /// <returns></returns>
        public static int GetDeveiceCount()
        {
            return NAudio.Wave.WaveIn.DeviceCount;
        }


        /// <summary>
        /// デバイス情報を返却
        /// </summary>
        /// <see href="https://github.com/naudio/NAudio/blob/master/NAudio/Wave/MmeInterop/WaveInCapabilities.cs"/>
        /// <example>
        /// string name = capabilities.productName;
        /// </example>
        public static string GetDeveceCapabilities(int deviceId)
        {
            NAudio.Wave.WaveInCapabilities capabilities = NAudio.Wave.WaveIn.GetCapabilities(deviceId);

            return capabilities.ProductName;
        }

        /******************************************************/

    }
}
