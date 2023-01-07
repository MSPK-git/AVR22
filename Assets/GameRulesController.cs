using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameRulesController : MonoBehaviour
{
    enum BikeCheckResult
    {
        OK,
        WrongComponents,
        WrongColor
    }

    [Serializable]
    public class Mission
    {
        public Material material;
        public string name;
    }

    [SerializeField]
    public List<Mission> missions = new List<Mission>();

    [SerializeField]
    public SocketInteractorWithFilter bikeSocket;

    [SerializeField]
    public TextMeshProUGUI moneyText;

    [SerializeField]
    public TextMeshProUGUI missionText;

    [SerializeField]
    public TextMeshProUGUI errorText;

    [SerializeField]
    public Button button;

    [SerializeField]
    public AudioSource missionCompleteAudio;

    [Tooltip("For testing: accept incomplete bike frames")]
    [SerializeField]
    public bool acceptIncompleteBikes;

    int money;

    int missionIndex;

    DateTime completionTime;


    public void Start()
    {
        money = 0;
        missionIndex = 0;

        UpdateMissionText();
    }

    // UI callback when the Button on the monitor is pressed
    public void CheckBicycleComplete()
    {
        if (missionIndex < missions.Count)
        {
            if (bikeSocket.interactablesSelected.Count != 0)
            {
                foreach (var interactable in bikeSocket.interactablesSelected)
                {
                    var gameObject = interactable.transform.gameObject;
                    var result = CheckBicycleAttachments(gameObject);
                    if (result == BikeCheckResult.OK)
                    {
                        DestroyBicycle(gameObject);

                        missionIndex++;
                        AddMoney(1000);
                        UpdateMissionText();
                        completionTime = DateTime.UtcNow;

                        missionCompleteAudio.Play();
                    }
                    else if (result == BikeCheckResult.WrongComponents)
                    {
                        ShowError("Fahrrad ist nicht vollständig!");
                    }
                    else if (result == BikeCheckResult.WrongColor)
                    {
                        ShowError("Rahmen hat die falsche Farbe!");
                    }
                }
            } else
            {
                if ((DateTime.UtcNow - completionTime).Seconds > 3)
                {
                    ShowError("Kein Fahrrad im Austellungsbereich");
                }
            }
        }
    }

    void AddMoney(int newMoney)
    {
        money += newMoney;
        moneyText.SetText(money + " €");
    }

    void UpdateMissionText()
    {
        // < color =#FF0000>
        errorText.SetText(string.Empty);
        if (missionIndex < missions.Count) {
            var mission = missions[missionIndex];
            string text = string.Format("Bauen Sie ein Rad mit einem {0} Rahmen", mission.name);
            missionText.SetText(text);
        } else
        {
            missionText.SetText("Keine weiteren Kundenaufträge verfügbar - Feierabend!");
            button.gameObject.SetActive(false);
        }
    }

    void ShowError(string errorMessage)
    {
        errorText.SetText(errorMessage);
        Invoke(nameof(ClearError), 5.0f);
    }

    void ClearError()
    {
        errorText.SetText("");
    }

    BikeCheckResult CheckBicycleAttachments(GameObject gameObject)
    {
        var mission = missions[missionIndex];
        var sockets = gameObject.GetComponentsInChildren<SocketInteractorWithFilter>();

        // 1. check if all sockets are populated
        int sumAttachments = 0;
        foreach (var socket in sockets)
        {
            if (socket.interactablesSelected.Count > 0)
            {
                sumAttachments++;
            }
        }

        if (sumAttachments == sockets.Length || acceptIncompleteBikes)
        {
            var meshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();

            // 2. check frame color
            bool isCorrectFrameColor = false;
            var bikeFrameMask = LayerMask.NameToLayer("Bike Frame");
            foreach (var meshRender in meshRenderer)
            {
                if (meshRender.gameObject.layer == bikeFrameMask)
                {
                    foreach (var material in meshRender.materials)
                    {
                        if (material.color == mission.material.color)
                        {
                            isCorrectFrameColor = true;
                        }
                    }

                    break;
                }
            }

            return isCorrectFrameColor ? BikeCheckResult.OK : BikeCheckResult.WrongColor;
        } else
        {
            return BikeCheckResult.WrongComponents;
        }
    }

    // Remove bicycle and all of its attachments
    void DestroyBicycle(GameObject gameObject)
    {
        var sockets = gameObject.GetComponentsInChildren<SocketInteractorWithFilter>();

        foreach (var socket in sockets)
        {
            // Get all gameobjects and put them into a separate list so we can delete all of them.
            // We can't delete them while iterating interactablesSelected (deleting while iterating
            List<GameObject> gameObjects = new List<GameObject>(socket.interactablesSelected.Count);
            foreach (var interactable in socket.interactablesSelected)
            {
                gameObjects.Add(interactable.transform.gameObject);
            }

            // Delete all gameobjects
            foreach (var obj in gameObjects)
            {
                Destroy(obj);
            }
        }
        Destroy(gameObject);
    }
}
