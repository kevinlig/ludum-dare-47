using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Pixelplacement;
using UniRx;
public class GameManager : Singleton<GameManager>
{
    [Serializable]
    public class GameView {
        public GameObject viewContainer;
        public string viewName = "";
    }
    public GameView[] gameViews;
    public FloatReactiveProperty latitude = new FloatReactiveProperty(0);
    public FloatReactiveProperty longitude = new FloatReactiveProperty(0);
    public IntReactiveProperty currentNavUnit = new IntReactiveProperty(0);
    public IntReactiveProperty fuelAvailable = new IntReactiveProperty(3);
    public StringReactiveProperty currentView = new StringReactiveProperty("cockpit");
    public LongReactiveProperty timeRemaining = new LongReactiveProperty(60 * 8);
    public LongReactiveProperty fuelTimeRemaining = new LongReactiveProperty(0);
    // keep all observers in sync using subject instead of individual subscriptions
    public BehaviorSubject<Vector2> playerLocation;

    public BehaviorSubject<StarController> activeStar;
    public BehaviorSubject<Dictionary<int, int>> deliveryDestinations;

    void Awake() {
        CreateObservables();
        SubscribeToData();
        StartGameTimer();
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

        fuelAvailable
            .Select((x) => x < 3 && fuelTimeRemaining.Value == 0)
            .Subscribe((x) => {
                StartFuelTimer();
            });
    }

    void SubscribeToData() {
        currentView
            .DistinctUntilChanged()
            .Subscribe(SwitchToView);

        currentNavUnit
            .Subscribe(CalculateLatitudeFromNavUnit);
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

    void SwitchToView(string view) {
        foreach(GameView gameView in gameViews) {
            bool activeState = false;
            if (gameView.viewName.Equals(view)) {
                activeState = true;
            }
            gameView.viewContainer.SetActive(activeState);
        }
    }

    void CalculateLatitudeFromNavUnit(int navUnit) {
        float degrees = (float) navUnit / AstroManager.Instance.navUnitsPerDeg;
        latitude.SetValueAndForceNotify(degrees);
    }

    public void SetActiveStar(StarController active) {
        activeStar.OnNext(active);
    }

    public void MoveByOne(int direction) {
        currentNavUnit.SetValueAndForceNotify(currentNavUnit.Value + (1 * direction));
    }

    public void DeliverTo(int boxNumber) {
        Dictionary<int, int> deliveries = deliveryDestinations.Value;

        // check if there is a delivery for this box
        if (!deliveries.ContainsKey(boxNumber)) {
            // not deliverable!
            GlobalUI.Instance.SetAlert("[ERROR]: You don't have a delivery for this dropbox.");
            return;
        }

        deliveries[boxNumber] = 0;
        GlobalUI.Instance.SetAlert(string.Format("[SUCCESS]: Delivered to dropbox {0}!", boxNumber));
        deliveryDestinations.OnNext(deliveries);
    }

    public void UpdateFuel(int newFuel) {
        fuelAvailable.SetValueAndForceNotify(newFuel);
    }

    public void StartGameTimer() {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeWhile((x) => timeRemaining.Value - 1 >= 0)
            .Subscribe((x) => {
                timeRemaining.SetValueAndForceNotify(timeRemaining.Value - 1);
            });
    }

    public void StartFuelTimer() {
        long fuelTime = 20 * (3 - fuelAvailable.Value);
        fuelTimeRemaining.SetValueAndForceNotify(fuelTime);
        Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeWhile((x) => fuelTimeRemaining.Value - 1 >= 0)
            .Subscribe((x) => {
                long newTime = fuelTimeRemaining.Value - 1;
                fuelTimeRemaining.SetValueAndForceNotify(newTime);
                if (newTime % 20 == 0) {
                    fuelAvailable.SetValueAndForceNotify(fuelAvailable.Value + 1);
                }
            });
    }

    public void FastTravel(float degrees) {
        // determine how many nav units this is
        int distance = Mathf.FloorToInt(AstroManager.Instance.navUnitsPerDeg * degrees);
        int newNavPos = currentNavUnit.Value + distance;
        int newFuel = fuelAvailable.Value - Mathf.CeilToInt(degrees / 20f);
        currentNavUnit.SetValueAndForceNotify(newNavPos);
        fuelAvailable.SetValueAndForceNotify(newFuel);
    }
}
