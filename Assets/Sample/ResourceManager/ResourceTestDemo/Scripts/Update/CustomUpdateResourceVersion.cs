
using ResourceMgr.Runtime.ResourceUpdate;
using UnityEngine;
using UnityEngine.UI;

public class CustomUpdateResourceVersion : UpdateResourceVersion
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text speedText;

    protected override void OnProgressChanged()
    {
        if (speedText != null)
            speedText.text = CurrentSpeed.ToString();

        if (progressSlider != null)
            progressSlider.value = ProgressTotal;
    }
    
    
    private string GetByteLengthString(long byteLength)
    {
        if (byteLength < 1024L) // 2 ^ 10
        {
            return $"{byteLength} Bytes";
        }

        if (byteLength < 1048576L) // 2 ^ 20
        {
            return $"{(byteLength / 1024f):F2} KB";
        }

        if (byteLength < 1073741824L) // 2 ^ 30
        {
            return $"{(byteLength / 1048576f):F2} MB";
        }

        if (byteLength < 1099511627776L) // 2 ^ 40
        {
            return $"{(byteLength / 1073741824f):F2} GB";
        }

        if (byteLength < 1125899906842624L) // 2 ^ 50
        {
            return $"{(byteLength / 1099511627776f):F2} TB";
        }

        if (byteLength < 1152921504606846976L) // 2 ^ 60
        {
            return $"{(byteLength / 1125899906842624f):F2} PB";
        }

        return $"{(byteLength / 1152921504606846976f):F2} EB";
    }
}


