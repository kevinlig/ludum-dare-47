﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Pixelplacement;
using UniRx;
public class GameManager : Singleton<GameManager>
{
    public FloatReactiveProperty latitude = new FloatReactiveProperty(0);
    public FloatReactiveProperty longitude = new FloatReactiveProperty(0);
    public IntReactiveProperty currentNavUnit = new IntReactiveProperty(0);
    // keep all observers in sync using subject instead of individual subscriptions
    public BehaviorSubject<Vector2> playerLocation;

    public BehaviorSubject<StarController> activeStar;

    void Awake() {
        CreateObservables();
    }

    void CreateObservables() {
        // trigger an observable based on when the player's position changes
        playerLocation = new BehaviorSubject<Vector2>(Vector2.zero);
        latitude.Merge(longitude)
            .Select((float _) => {
                // value could be either lat or lng, so just cheat and grab the current property values
                return new Vector2(longitude.Value, latitude.Value);
            })
            .Subscribe((Vector2 coords) => {
                playerLocation.OnNext(coords);
            });

        activeStar = new BehaviorSubject<StarController>(null);
    }

    public void SetActiveStar(StarController active) {
        activeStar.OnNext(active);
    }
}
