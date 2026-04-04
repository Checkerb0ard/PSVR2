using EyeTracking;
using EyeTracking.TrackingProviders;
using PSVR2Toolkit.Utilities;

namespace PSVR2.Features;

public class PSVR2EyeTracking : TrackingProvider
{
    public override string Name => "PSVR2";
    public override bool SupportsEye => true;
    public override bool SupportsFace => false;
    public override bool IsLoaded => Core.Instance?.ToolkitManager?.Loaded ?? false;

    private const int noiseFilterSamples = 8;
    private LowPassFilter leftEyeFilter;
    private LowPassFilter rightEyeFilter;

    public override void Initialize()
    {
        leftEyeFilter = new LowPassFilter(noiseFilterSamples);
        rightEyeFilter = new LowPassFilter(noiseFilterSamples);
        
        Tracking.EyeData.MinDilation = 0f;
        Tracking.EyeData.MaxDilation = 10f;
    }

    public override void Update()
    {
        if (Core.Instance?.ToolkitManager?.Loaded != true)
            return;

        if (!Core.Instance.ToolkitManager.IpcClient.IsRunning)
            return;

        var eyeTrackingData = Core.Instance.ToolkitManager.IpcClient.RequestEyeTrackingData();

        if (eyeTrackingData.leftEye.isBlinkValid)
        {
            float leftOpenness;

            if (eyeTrackingData.leftEye.isOpenEnabled && Core.Instance.PreferencesManager.EyeLidEstimation.Value)
            {
                leftOpenness = eyeTrackingData.leftEye.open;
            }
            else
            {
                leftOpenness = eyeTrackingData.leftEye.blink ? 0 : 1;
            }
            
            if (leftEyeFilter != null)
            {
                leftOpenness = leftEyeFilter.FilterValue(leftOpenness);
            }

            Tracking.EyeData.Left.Openness = leftOpenness;
        }

        if (eyeTrackingData.rightEye.isBlinkValid)
        {
            float rightOpenness;

            if (eyeTrackingData.rightEye.isOpenEnabled && Core.Instance.PreferencesManager.EyeLidEstimation.Value)
            {
                rightOpenness = eyeTrackingData.rightEye.open;
            }
            else
            {
                rightOpenness = eyeTrackingData.rightEye.blink ? 0 : 1;
            }
            
            if (rightEyeFilter != null)
            {
                rightOpenness = rightEyeFilter.FilterValue(rightOpenness);
            }

            Tracking.EyeData.Right.Openness = rightOpenness;
        }

        if (eyeTrackingData.leftEye.isGazeDirValid)
        {
            Tracking.EyeData.Left.GazeX = -eyeTrackingData.leftEye.gazeDirNorm.x;
            Tracking.EyeData.Left.GazeY =  eyeTrackingData.leftEye.gazeDirNorm.y;
        }

        if (eyeTrackingData.rightEye.isGazeDirValid)
        {
            Tracking.EyeData.Right.GazeX = -eyeTrackingData.rightEye.gazeDirNorm.x;
            Tracking.EyeData.Right.GazeY =  eyeTrackingData.rightEye.gazeDirNorm.y;
        }

        if (eyeTrackingData.leftEye.isPupilDiaValid)
        {
            Tracking.EyeData.Left.PupilDiameterMm = eyeTrackingData.leftEye.pupilDiaMm;
        }

        if (eyeTrackingData.rightEye.isPupilDiaValid)
        {
            Tracking.EyeData.Right.PupilDiameterMm = eyeTrackingData.rightEye.pupilDiaMm;
        }
    }
}