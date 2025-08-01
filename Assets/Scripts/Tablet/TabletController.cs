using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;
using DG.Tweening;
using System.Net;

public class TabletController : MonoBehaviour
{
    [SerializeField] private Transform Tablet;
    [SerializeField] private InventoryController inventoryRef;
    [SerializeField] private AllItemsInGame allItemsRef;
    [SerializeField] private MoneyController moneyControllerRef;
    [SerializeField] private GameFlowController FlowControllerRef;
    [SerializeField] private ClientController ClientControllerRef;
    [SerializeField] private NewsletterClass newsletterRef;
    [SerializeField] private Hand handRef;
    [SerializeField] private GameObject storeContainer;
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private GameObject storeSpacerPrefab;
    [SerializeField] private TextMeshProUGUI totalPriceFooter;
    [SerializeField] private Button orderButton;
    [SerializeField] private GameObject warningText;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject unlockScreen;
    [SerializeField] private GameObject confirmationScreen;
    [SerializeField] private Button nextArticleButton;
    [SerializeField] private Button previousArticleButton;
    [SerializeField] private Button pocketZoneButton;
    [SerializeField] private Button unpocketZoneButton;
    [SerializeField] private Transform EndPoint;
    [SerializeField] private Transform StartPoint;

    public List<Event> AllEvents = new List<Event>();
    public List<Event> possibleEvents = new List<Event>();
    public List<Event> queuedEvents = new List<Event>();

    public List<GameObject> storeContents = new List<GameObject>();
    public List<StoreItem> storeItems = new List<StoreItem>();

    private StoreSpacer lastSpacer;

    private int currentDay = 0;
    private int currentTotalPrice = 0;
    private int dailyClients = 3;
    private int currentViewedArticle = 0;

    private string controllerName = "TabletController";
    public string ControllerName
    {
        get { return controllerName; }
        set { controllerName = value; }
    }

    [System.Serializable]
    public class Event
    {
        public string title;
        public string contents;
        public string imageNameRef;
        public int clientID;
        public Event(string myTitle, string myContents, string myImageNameRef, int myClientID)
        {
            title = myTitle;
            contents = myContents;
            imageNameRef = myImageNameRef;
            clientID = myClientID;
        }
    }

    public void Awake()
    {
        totalPriceFooter.text = "0 $B";
        if (moneyControllerRef == null)
        {
            Debug.LogWarning("PODEPNIJ MONEY CONTROLLER");
        }
        if (allItemsRef == null)
        {
            Debug.LogWarning("PODEPNIJ WSZYSTKIE ITEMKI");
        }
        if (inventoryRef == null)
        {
            Debug.LogWarning("PODEPNIJ INVENTORY");
        }

        InitializeEvents();
    }

    private void InitializeEvents()
    {
        LoadEvents();
        if (possibleEvents.Count > 0)
        {
            return;
        }
        foreach (Event e in AllEvents)
        {
            possibleEvents.Add(e);
        }
    }

    public void PullOutTablet()
    {
        InitializeEvents();
        queuedEvents.Clear();
        for (int i = 0; i < dailyClients; i++)
        {
            queuedEvents.Add(GetRandomEvent());
        }
        Event defaultArticle = queuedEvents[0];
        currentViewedArticle = 0;
        newsletterRef.LoadArticle(currentDay, defaultArticle.title, defaultArticle.contents, defaultArticle.imageNameRef);
        GenerateStoreContents();
        unlockScreen.SetActive(true);
        loadingScreen.SetActive(false);
        confirmationScreen.SetActive(false);
        nextArticleButton.interactable = true;
        previousArticleButton.interactable = false;
        pocketZoneButton.gameObject.SetActive(true);

        StartCoroutine(PullOutTabletAnim());
    }

    private IEnumerator PullOutTabletAnim()
    {
        yield return new WaitForSeconds(1f);
        storeContainer.SetActive(false);
        yield return null;
        storeContainer.SetActive(true);
        Canvas.ForceUpdateCanvases();
        yield return new WaitForSeconds(2f);
        CameraController.Instance.ShowTablet();
        handRef.SetUIHandBlock(true);
    }

    public void NextArticle()
    {
        currentViewedArticle++;
        if (currentViewedArticle >= queuedEvents.Count - 1)
        {
            nextArticleButton.interactable = false;
        }
        if (currentViewedArticle > 0)
        {
            previousArticleButton.interactable = true;
        }
        Event currentEvent = queuedEvents[currentViewedArticle];
        newsletterRef.LoadArticle(currentDay, currentEvent.title, currentEvent.contents, currentEvent.imageNameRef);
    }

    public void PreviousArticle()
    {
        currentViewedArticle--;
        if (currentViewedArticle <= 0)
        {
            previousArticleButton.interactable = false;
        }
        if (currentViewedArticle < queuedEvents.Count - 1)
        {
            nextArticleButton.interactable = true;
        }
        Event currentEvent = queuedEvents[currentViewedArticle];
        newsletterRef.LoadArticle(currentDay, currentEvent.title, currentEvent.contents, currentEvent.imageNameRef);
    }

    public void UnlockScreen()
    {
        unlockScreen.SetActive(false);
        shopScreen.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        shopScreen.SetActive(true);
    }

    public void PlaceOrder()
    {
        if (currentTotalPrice != 0)
        {
            StartCoroutine(PlaceOrderAsync());
        }
    }

    private IEnumerator PlaceOrderAsync()
    {
        yield return null;
        Debug.Log("Placed order");
        loadingScreen.SetActive(true);
        loadingScreen.GetComponent<TabletLoadingScreen>().StartLoading();
        shopScreen.SetActive(false);
        moneyControllerRef.spendMoney(currentTotalPrice);

        foreach (StoreItem item in storeItems)
        {
            GameObject itemGO;
            int itemCount;
            item.GetOrderCount(out itemGO, out itemCount);
            if (itemCount <= 0)
            {
                continue;
            }
            for (int i = 0; i < itemCount; i++)
            {
                inventoryRef.AddToInventory(itemGO);
            }
            Debug.Log("Zamowiles " + itemCount.ToString() + " razy item o ID " + itemGO.name.ToString());
        }
    }

    public void ClearOrder()
    {
        foreach (StoreItem item in storeItems)
        {
            item.ResetOrderCount();
        }
    }

    public void DeliveryCompleted()
    {
        StartCoroutine(PutTabletAway());
    }

    private IEnumerator PutTabletAway()
    {
        yield return new WaitForSeconds(3f);
        CameraController.Instance.HideTablet();
        Debug.Log("Tablet Turns Off");
        handRef.SetUIHandBlock(false);
        pocketZoneButton.gameObject.SetActive(false);
        unpocketZoneButton.gameObject.SetActive(false);
        FlowControllerRef.FinishRequirement(controllerName);
    }

    private void GenerateStoreContents()
    {
        ResetStoreContent();
        StartCoroutine(GenerateContentsAsync());
    }

    private IEnumerator GenerateContentsAsync()
    {
        yield return null;
        int i = 0;
        ItemType currentType = ItemType.FRAME;

        foreach (Item item in allItemsRef.allItems)
        {
            if(i == 0 || currentType != item.itemType)
            {
                currentType = item.itemType;
                GameObject spacer = Instantiate(storeSpacerPrefab, storeContainer.transform);
                lastSpacer = spacer.GetComponent<StoreSpacer>();
                string spacerName = "Gun " + currentType.ToString().Substring(0, 1) + currentType.ToString().Substring(1).ToLower() + "s";
                lastSpacer.InitializeSpacer(spacerName);
                storeContents.Add(spacer);
            }
            currentType = item.itemType;

            GameObject newItem = Instantiate(storeItemPrefab, storeContainer.transform);
            StoreItem newStoreItem = newItem.GetComponent<StoreItem>();
            newStoreItem.InitializeItem(item.gameObject, item.iconName, item.name, item.description, item.price, this);
            storeContents.Add(newItem);
            storeItems.Add(newStoreItem);

            lastSpacer.AddChildItem(newItem);

            i++;
        }
    }

    private void ResetStoreContent()
    {
        foreach (GameObject item in storeContents)
        {
            Destroy(item);
        }
        storeContents.Clear();
        storeItems.Clear();
        currentTotalPrice = 0;
        totalPriceFooter.text = "0 $B";
        warningText.SetActive(false);
        orderButton.interactable = false;
    }

    public void UpdateTotalPrice(int priceChange)
    {
        currentTotalPrice += priceChange;
        totalPriceFooter.text = currentTotalPrice.ToString() + " $B";
        bool enoughMoney = moneyControllerRef.CheckIfPlayerHasEnough(currentTotalPrice);
        if (enoughMoney)
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


    public List<int> GetQueuedClientIDs()
    {
        List<int> clientIDs = new List<int>();
        foreach (Event ev in queuedEvents)
        {
            clientIDs.Add(ev.clientID);
        }
        return clientIDs;
    }

    private Event GetRandomEvent()
    {
        int chosenEventIndex = UnityEngine.Random.Range(0, possibleEvents.Count);
        Event currentEvent = possibleEvents[chosenEventIndex];
        possibleEvents.RemoveAt(chosenEventIndex);
        ClientControllerRef.AddNextDaysClient(currentEvent.clientID);
        return currentEvent;
    }

    private void ResetPossibleEvents()
    {
        foreach (Event e in AllEvents)
        {
            possibleEvents.Add(e);
        }
    }

    public void PocketTablet()
    {
        handRef.SetUIHandBlock(false);
        CameraController.Instance.PocketTablet();
    }

    public void UnpocketTablet()
    {
        handRef.SetUIHandBlock(true);
        CameraController.Instance.ShowTablet();
    }

    private void LoadEvents()
    {
        if(AllEvents.Count > 0)
        {
            return;
        }
        AllEvents.Add(new Event("Forests No More", "Another day, another factory! We want to gladly inform you that another forest in our region will be cut down! How cool is that? The construction of the new factory will begin short after, but don't worry, this time there will be no child labor! We ain't savages!", "FactoryArticleImage", 1));
        AllEvents.Add(new Event("Hunting Championships", "Great hunting competition begins! 'Beware all animals!' says one of the contestants. 'I am going to win this trophy!' says another. 'Why isn't meat cooking itself?!' says third, weirdly cricle shaped contestant. We wish best of luck and stay tuned for a winner annoucement.", "HuntingArticleImage", 2));
        AllEvents.Add(new Event("Sexy And Deadly", "New victims to the famous 'Black Widow'. Another one bites the dust, they say, and this week almost three guys have met their destined death! 'Black Widow' is still on the loose and no one seems to know who she really is.", "WidowArticleImage", 3));
        AllEvents.Add(new Event("Assassin For Hire", "Is that a bird? Is that a plane? NO! It's another victim of the mysterious hitman. Dude is so cool and quiet. He never misses and he always kills with style!", "HitmanArticleImage", 4));
        AllEvents.Add(new Event("This Article Will Change Your Life", "Are you lonely? Are you a sad loser? Terrible at anything you do? Don't worry! We have a solution just for you! You should... KILL YOURSELF NOW!", "SuicideArticleImage", 5));//Notatka, mo�e jednak zwi�kszony wska�nik samob�jstw?
        AllEvents.Add(new Event("Biggest Nuclear Explosion This Year", "This just in, nuclear factory explosion in the Kersk region! What an interesting day to be alive! We don't have any information about possible survivors yet, but we're hoping to see some cool mutations!", "StalkerArticleImage", 6));
    }
}
