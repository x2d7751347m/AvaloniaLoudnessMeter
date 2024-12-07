using System;
using System.Runtime.InteropServices;
using ManagedBass;

namespace AvaloniaLoudnessMeter.Services;

public class AudioCaptureService : IDisposable
{
    public delegate void DataAvailableHandler(byte[] buffer, int length);

    #region Default Constructor

    public AudioCaptureService(int deviceId)
    {
        mDevice = deviceId;

        Bass.Init();
        Bass.RecordInit(mDevice);

        mHandle = Bass.RecordStart(44100, 2, BassFlags.RecordPause, Procedure);
    }

    #endregion

    public void Dispose()
    {
        Bass.CurrentRecordingDevice = mDevice;

        Bass.RecordFree();
    }

    #region Capture Audio Methods

    private bool Procedure(int handle, IntPtr buffer, int length, IntPtr user)
    {
        if (mBuffer == null || mBuffer.Length < length)
            mBuffer = new byte[length];

        Marshal.Copy(buffer, mBuffer, 0, length);

        DataAvailable?.Invoke(mBuffer, length);

        return true;
    }

    #endregion

    public event DataAvailableHandler DataAvailable;

    public void Start()
    {
        Bass.ChannelPlay(mHandle);
    }

    public void Stop()
    {
        Bass.ChannelStop(mHandle);
    }

    public void CaptureDefaultInput()
    {
    }

    #region MyRegion

    private readonly int mDevice;
    private readonly int mHandle;

    private byte[] mBuffer;

    #endregion
}