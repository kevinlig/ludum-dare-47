using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Pixelplacement;
using UniRx;

public class AstroManager : Singleton<AstroManager>
{
    private List<string> possibleNames;
    private List<string> shuffledNames;

    public float navUnitSize = 1f;
    public int navUnitsPerDeg = 10;

    public GameObject starPrefab;
    public int starCount = 32;
    public List<StarData> starData;
    public List<StarController> starControllers;

    void Awake()
    {
        // get star info
        possibleNames = LoadStarMeta.GetStarNames();
        // shuffle the name list so we get random names each time and there are no repeats
        shuffledNames = ShuffleNames();

        navUnitsPerDeg = UnityEngine.Random.Range(8, 25);

        starCount = UnityEngine.Random.Range(24, 48);

        // generate a star count
        for (int i = 0; i < starCount; i++) {
            GenerateStar(i);
        }
    }

    void GenerateStar(int starId) {
        string name = shuffledNames[starId];

        float hourAngle = UnityEngine.Random.Range(-60f, 60f);  
        // spread declination out evenly so there are stars in every hemisphere
        float declination = (360 / starCount) * starId * 0.5f;
        if (starId % 2 == 0) {
            declination = declination * -1;
        }

        StarData star = new StarData(starId, name, declination, hourAngle);
        starData.Add(star);

        // create the game object
        GameObject starObject = Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
        starObject.name = string.Format("Star {0}", starId);
        starObject.transform.parent = gameObject.transform;
        starObject.layer = gameObject.layer;
        StarController controller = starObject.GetComponent<StarController>();
        controller.data = star;
        
        starControllers.Add(controller);
    }

    List<string> ShuffleNames() {
        // https://improveandrepeat.com/2018/08/a-simple-way-to-shuffle-your-lists-in-c/
        return possibleNames.OrderBy(x => Guid.NewGuid()).ToList();
    }

}
