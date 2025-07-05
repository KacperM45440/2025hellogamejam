using System;
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
    [SerializeField] private TextMeshProUGUI totalPriceHeader;
    [SerializeField] private TextMeshProUGUI totalPriceFooter;
    [SerializeField] private Button orderButton;
    [SerializeField] private GameObject warningText;

    public List<Event> AllEvents = new List<Event>();
    public List<Event> possibleEvents = new List<Event>();

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
        LoadEvents();
        foreach (Event e in AllEvents)
        {
            possibleEvents.Add(e);
        }
    }

    private void Start()
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
        Debug.Log("Placed order");
    }

    private void GenerateStoreItems()
    {
        //pêtla foreach, przechodz¹ca przez wszystkie przedmioty
        GameObject newItem = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample Item", "This is a sample item description.", 100, this);
        GameObject newItem2 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem2.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample2 Item", "This is a sample item description.", 100, this);
        GameObject newItem3 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem3.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample 3Item", "This is a sample item description.", 100, this);
        GameObject newItem4 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem4.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample Item", "This is a sample item description.", 100, this);
        GameObject newItem5 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem5.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample2 Item", "This is a sample item description.", 100, this);
        GameObject newItem6 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem6.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample 3Item", "This is a sample item description.", 100, this);
        GameObject newItem7 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem7.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample Item", "This is a sample item description.", 100, this);
        GameObject newItem8 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem8.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample2 Item", "This is a sample item description.", 100, this);
        GameObject newItem9 = Instantiate(storeItemPrefab, storeContainer.transform);
        newItem9.GetComponent<StoreItem>().InitializeItem("store_item_image", "Sample 3Item", "This is a sample item description.", 100, this);
    }

    public void UpdateTotalPrice(int priceChange)
    {
        currentTotalPrice += priceChange;
        totalPriceHeader.text = "Total Price: " + currentTotalPrice.ToString();
        totalPriceFooter.text = "Total Price: " + currentTotalPrice.ToString();
        bool enoughMoney = moneyControllerRef.CheckIfPlayerHasEnough(currentTotalPrice);
        if(enoughMoney)
        {
            warningText.SetActive(false);
            orderButton.interactable = true;
        }
        else
        {
            warningText.SetActive(true);
            orderButton.interactable = true;
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
