using System;
using System.Collections.Generic;
using ManagedBass;

namespace AvaloniaLoudnessMeter.Services;

public class RecordingDevice : IDisposable
{
    private RecordingDevice(int Index, string name)
    {
        this.Index = Index;

        Name = name;
    }

    public string Name { get; }

    public int Index { get; }

    public void Dispose()
    {
        Bass.CurrentRecordingDevice = Index;
        Bass.RecordFree();
    }

    public static IEnumerable<RecordingDevice> Enumerate()
    {
        for (var i = 0; Bass.RecordGetDeviceInfo(i, out var info); ++i)
            yield return new RecordingDevice(i, info.Name);
    }

    public override string ToString()
    {
        return Name;
    }
}