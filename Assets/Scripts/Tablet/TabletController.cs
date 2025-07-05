using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class TabletController : MonoBehaviour
{
    [SerializeField] private MoneyController moneyControllerRef;
    [SerializeField] private NewsletterClass newsletterRef;
    [SerializeField] private GameObject storeContainer;
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalPriceFooter;
    [SerializeField] private Button orderButton;
    [SerializeField] private GameObject warningText;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject loadingScreen;

    public List<Event> AllEvents = new List<Event>();
    public List<Event> possibleEvents = new List<Event>();

    public List<StoreItem> storeItems = new List<StoreItem>();

    private int currentDay = 0;
    private int currentTotalPrice = 0;

    [System.Serializable]
    public class Event
    {
        public string title;
        public string contents;
        public string imageNameRef;
        public List<int> clients = new List<int>();
        public Event(string myTitle, string myContents, string myImageNameRef, List<int> myClients)
        {
            title = myTitle;
            contents = myContents;
            imageNameRef = myImageNameRef;
            clients = myClients;
        }
    }

    public void Awake()
    {
        totalPriceFooter.text = "0 $B";
        if (moneyControllerRef == null)
        {
            Debug.LogWarning("PODEPNIJ MONEY CONTROLLER");
        }

        LoadEvents();
        foreach (Event e in AllEvents)
        {
            possibleEvents.Add(e);
        }
    }

    private void Start()//DO USUNIÊCIA
    {
        PullOutTablet();
    }

    public void PullOutTablet()
    {
        currentDay++;
        Event currentEvent = GetRandomEvent();
        newsletterRef.InitializeArticle(currentDay, currentEvent.title, currentEvent.contents, currentEvent.imageNameRef);
        GenerateStoreItems();
    }

    public void PlaceOrder()
    {
        if (currentTotalPrice != 0)
        {
            Debug.Log("Placed order");
            loadingScreen.SetActive(true);
            loadingScreen.GetComponent<TabletLoadingScreen>().StartLoading();
            shopScreen.SetActive(false);

            //send list of bought items to the game controller
            foreach (StoreItem item in storeItems)
            {
                int itemID;
                int itemCount;
                item.GetOrderCount(out itemID, out itemCount);
                if (itemCount <= 0)
                {
                    continue;
                }
                Debug.Log("Zamowiles " + itemCount.ToString() + " razy item o ID " + itemID.ToString());
            }
        }
    }

    public void DeliveryCompleted()
    {
        StartCoroutine(PutTabletAway());
    }

    private IEnumerator PutTabletAway()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Tablet Turns Off");
    }

    private void GenerateStoreItems()
    {
        //Pêtla for jest tymczasowa - zostanie zast¹piona wszystkimi rzeczami
        for (int i = 0; i < 8; i++)
        {
            GameObject newItem = Instantiate(storeItemPrefab, storeContainer.transform);
            StoreItem newStoreItem = newItem.GetComponent<StoreItem>();
            newStoreItem.InitializeItem(i, "store_item_image "+ i, "Sample Item", "This is a sample item description.", 100, this);
            storeItems.Add(newStoreItem);
        }
    }

    public void UpdateTotalPrice(int priceChange)
    {
        currentTotalPrice += priceChange;
        totalPriceFooter.text = currentTotalPrice.ToString() + "$B";
        bool enoughMoney = moneyControllerRef.CheckIfPlayerHasEnough(currentTotalPrice);
        if(enoughMoney)
        {
            warningText.SetActive(false);
            orderButton.interactable = true;
        }
        else
        {
            warningText.SetActive(true);
            orderButton.interactable = false;
        }
    }

    private Event GetRandomEvent()
    {
        int chosenEventIndex = UnityEngine.Random.Range(0, possibleEvents.Count);
        Event currentEvent = possibleEvents[chosenEventIndex];
        possibleEvents.RemoveAt(chosenEventIndex);
        return currentEvent;
    }

    private void ResetPossibleEvents()
    {
        foreach (Event e in AllEvents)
        {
            possibleEvents.Add(e);
        }
    }

    private void LoadEvents()//Temporary articles - place articles here
    {
        AllEvents.Add(new Event("Day 1: The Beginning", "Today marks the start of our journey. We are excited to explore new horizons and share our experiences with you.", "day1_image", new List<int> { 1, 2, 3 }));
        AllEvents.Add(new Event("Day 2: Discoveries", "On the second day, we made some fascinating discoveries that will shape our future endeavors.", "day2_image", new List<int> { 1, 2, 3 }));
        AllEvents.Add(new Event("Day 3: Challenges", "Every journey has its challenges. Today, we faced some obstacles but learned valuable lessons.", "day3_image", new List<int> { 1, 2, 3 }));
        AllEvents.Add(new Event("Day 4: Triumphs", "Despite the challenges, we celebrated our triumphs and looked forward to what lies ahead.", "day4_image", new List<int> { 1, 2, 3 }));
    }
}
