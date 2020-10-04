using System.Collections;
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
    public IntReactiveProperty fuelAvailable = new IntReactiveProperty(3);
    public StringReactiveProperty currentView = new StringReactiveProperty("cockpit");
    // keep all observers in sync using subject instead of individual subscriptions
    public BehaviorSubject<Vector2> playerLocation;

    public BehaviorSubject<StarController> activeStar;
    public BehaviorSubject<Dictionary<int, int>> deliveryDestinations;

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
        deliveryDestinations = new BehaviorSubject<Dictionary<int, int>>(GenerateDeliveries());
    }

    Dictionary<int, int> GenerateDeliveries() {
        int count = UnityEngine.Random.Range(3, 6);
        // we are only doing northeastern hemisphere
        int maxNavUnit = 90 * AstroManager.Instance.navUnitsPerDeg;

        Dictionary<int, int> destinations = new Dictionary<int, int>();
        while (destinations.Count < count) {
            int destination = UnityEngine.Random.Range(1, maxNavUnit + 1);
            if (!destinations.ContainsValue(destination)) {
                destinations.Add(destination, 1);
            }
        }

        return destinations;
    }

    public void SetActiveStar(StarController active) {
        activeStar.OnNext(active);
    }

    public void MoveByOne(int direction) {
        currentNavUnit.SetValueAndForceNotify(currentNavUnit.Value + (1 * direction));
    }

    public void DeliverTo(int boxNumber) {
        Dictionary<int, int> deliveries = deliveryDestinations.Value;
        deliveries[boxNumber] = 0;
        deliveryDestinations.OnNext(deliveries);
    }

    public void UpdateFuel(int newFuel) {
        fuelAvailable.SetValueAndForceNotify(newFuel);
    }
}
