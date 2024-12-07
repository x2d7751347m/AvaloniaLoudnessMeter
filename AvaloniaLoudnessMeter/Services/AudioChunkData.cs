namespace AvaloniaLoudnessMeter.Services;

public record AudioChunkData(
    double Loudness,
    double ShortTermLUFS, 
    double IntegratedLUFS,
    double LoudnessRange,
    double RealtimeDynamics,
    double AverageRealtimeDynamics,
    double MomentaryMaxLUFS,
    double ShortTermMaxLUFS,
    double TruePeakMax
    );