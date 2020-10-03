using System.Collections;
using System.Collections.Generic;
using UniRx;

[System.Serializable]
public class StarData
{
    public int id = 0;
    public string name = "";
    // public float declination = 0f;
    // public float hourAngle = 0f;
    public FloatReactiveProperty declination = new FloatReactiveProperty(0);
    public FloatReactiveProperty hourAngle = new FloatReactiveProperty(0);

    public StarData(int starId, string starName, float dec, float ha) {
        id = starId;
        name = starName;
        declination = new FloatReactiveProperty(dec);
        hourAngle = new FloatReactiveProperty(ha);
    }
}
