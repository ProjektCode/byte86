using UnityEngine;
using MoreMountains.Feedbacks;

public class FeedBackManager : MonoBehaviour
{
    public static FeedBackManager Instance { get; private set; }

    [Header("MMF_Player References")]
    [SerializeField] private MMF_Player CameraShakePlayer;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void CameraShake(float intensity = 0.2f, float duration = 0.1f, float frequency = 40) {

        foreach(MMF_Feedback feedback in CameraShakePlayer.FeedbacksList) {
            if(feedback is MMF_CameraShake shake){
                shake.CameraShakeProperties.Amplitude = intensity;
                shake.CameraShakeProperties.Duration = duration;
                shake.CameraShakeProperties.Frequency = frequency;
            }
        }

        // If you have multiple shake events, you can configure these dynamically here
        CameraShakePlayer.PlayFeedbacks();
    }

}
