using EyeTracking;
using EyeTracking.TrackingProviders;
using PSVR2Toolkit;
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

        hmd2_gaze_status_t gazeStatus = new hmd2_gaze_status_t();
        
        if (!PSVR2ToolkitCAPI.GetGazeStatus(ref gazeStatus, 1000))
        {
            Core.Instance.LoggerInstance.Error("Failed to get gaze status from PSVR2Toolkit.");
            return;
        }

        if (gazeStatus.wearable.left.is_blink_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE)
        {
            float leftOpenness = gazeStatus.wearable.left.blink == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ? 0 : 1;
            
            if (leftEyeFilter != null)
            {
                leftOpenness = leftEyeFilter.FilterValue(leftOpenness);
            }

            Tracking.EyeData.Left.Openness = leftOpenness;
        }

        if (gazeStatus.wearable.right.is_blink_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE)
        {
            float rightOpenness = gazeStatus.wearable.right.blink == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE ? 0 : 1;
            
            if (rightEyeFilter != null)
            {
                rightOpenness = rightEyeFilter.FilterValue(rightOpenness);
            }

            Tracking.EyeData.Right.Openness = rightOpenness;
        }

        if (gazeStatus.wearable.left.is_gaze_dir_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE)
        {
            Tracking.EyeData.Left.GazeX = -gazeStatus.wearable.left.gaze_dir_norm.x;
            Tracking.EyeData.Left.GazeY =  gazeStatus.wearable.left.gaze_dir_norm.y;
        }

        if (gazeStatus.wearable.right.is_gaze_dir_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE)
        {
            Tracking.EyeData.Right.GazeX = -gazeStatus.wearable.right.gaze_dir_norm.x;
            Tracking.EyeData.Right.GazeY =  gazeStatus.wearable.right.gaze_dir_norm.y;
        }

        if (gazeStatus.wearable.left.is_pupil_dia_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE)
        {
            Tracking.EyeData.Left.PupilDiameterMm = gazeStatus.wearable.left.pupil_dia_mm;
        }

        if (gazeStatus.wearable.right.is_pupil_dia_valid == hmd2_gaze_bool_t.HMD2_GAZE_BOOL_TRUE)
        {
            Tracking.EyeData.Right.PupilDiameterMm = gazeStatus.wearable.right.pupil_dia_mm;
        }
    }
}