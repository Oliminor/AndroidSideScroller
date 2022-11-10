using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UpdateVolumeSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UpdateSettigns();
    }

    private void UpdateSettigns()
    {
        Volume _volume = GetComponent<Volume>();
        Bloom _bloom;
        _volume.profile.TryGet(out _bloom);
        _bloom.active = PlayerSettings.instance.IsBloomOn;

        UniversalRenderPipelineAsset _pipleine = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        _pipleine.renderScale = PlayerSettings.instance.RenderScale;
        _pipleine.supportsHDR = PlayerSettings.instance.IsHDROn;
    }
}
