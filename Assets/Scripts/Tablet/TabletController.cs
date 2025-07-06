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

public class TabletController : MonoBehaviour
{
    [SerializeField] private AllItemsInGame allItemsRef;
    [SerializeField] private MoneyController moneyControllerRef;
    [SerializeField] private NewsletterClass newsletterRef;
    [SerializeField] private GameObject storeContainer;
    [SerializeField] private GameObject storeItemPrefab;
    [SerializeField] private TextMeshProUGUI totalPriceFooter;
    [SerializeField] private Button orderButton;
    [SerializeField] private GameObject warningText;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Button nextArticleButton;
    [SerializeField] private Button previousArticleButton;

    public List<Event> AllEvents = new List<Event>();
    public List<Event> possibleEvents = new List<Event>();
    public List<Event> queuedEvents = new List<Event>();

    public List<StoreItem> storeItems = new List<StoreItem>();

    private int currentDay = 0;
    private int currentTotalPrice = 0;
    private int dailyClients = 3;
    private int currentViewedArticle = 0;

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
        for (int i = 0; i < dailyClients; i++)
        {
            queuedEvents.Add(GetRandomEvent());
        }
        Event defaultArticle = queuedEvents[0];
        currentViewedArticle = 0;
        newsletterRef.LoadArticle(currentDay, defaultArticle.title, defaultArticle.contents, defaultArticle.imageNameRef);
        GenerateStoreItems();
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

    public void PlaceOrder()
    {
        if (currentTotalPrice != 0)
        {
            Debug.Log("Placed order");
            loadingScreen.SetActive(true);
            loadingScreen.GetComponent<TabletLoadingScreen>().StartLoading();
            shopScreen.SetActive(false);
            moneyControllerRef.spendMoney(currentTotalPrice);

            foreach (StoreItem item in storeItems)
            {
                int itemID;
                int itemCount;
                item.GetOrderCount(out itemID, out itemCount);
                if (itemCount <= 0)
                {
                    continue;
                }
                //send list of bought items to the game controller
                //PRZY MERGOWANIU U¯YJ TEJ LISTY DO ODCZYTANIA JAKIE PRZEDMIOTY GRACZ ZAKUPI£
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
        int i = 0;
        foreach (Item item in allItemsRef.allItems)
        {
            GameObject newItem = Instantiate(storeItemPrefab, storeContainer.transform);
            StoreItem newStoreItem = newItem.GetComponent<StoreItem>();
            newStoreItem.InitializeItem(i, item.iconName, item.name, item.description, item.price, this);
            storeItems.Add(newStoreItem);
            i++;
        }
    }

    public void UpdateTotalPrice(int priceChange)
    {
        Debug.Log("Updating total price "+ currentTotalPrice + " by " + priceChange.ToString() + " $B");
        currentTotalPrice += priceChange;
        totalPriceFooter.text = currentTotalPrice.ToString() + " $B";
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

    public List<int> GetQueuedClientIDs()//PRZY MERGOWANIU U¯YJ TEJ METODY TO ODCZYTANIA JAKICH KLIENTÓW ZESPAWNOWAÆ NASTÊPNEGO DNIA
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
        return currentEvent;
    }

    private void ClearQueuedEvents()
    {
        queuedEvents.Clear();
    }

    private void ResetPossibleEvents()
    {
        foreach (Event e in AllEvents)
        {
            possibleEvents.Add(e);
        }
    }

    private void LoadEvents()
    {
        AllEvents.Add(new Event("Forests No More", "Another day, another factory! We want to gladly inform you that another forest in our region will be cut down! How cool is that? The construction of new factory will begin short after, but don't worry, this time there will be no child labor! We ain't savages!", "FactoryArticleImage", 2));
        AllEvents.Add(new Event("Hunting Competitions", "Great hunting competition begins! 'Beware all animals!' says one of the contestants. 'I am going to win this trophy!' says another. 'Why isn't meat cooking itself?!' says third, weirdly cricle shaped contestant. We wish all luck and stay tuned for a winner annoucement.", "HuntingArticleImage", 3));
        AllEvents.Add(new Event("Deadly But Sexy", "New victims to a famous 'Black Widow' Another one bites the dust, they say, and this week almost three guys have met their destined death! 'Black widow' is still on the loose and no one seems to know who she really is.", "WidowArticleImage", 4));
        AllEvents.Add(new Event("Serial Killer On The Loose", "Is that a bird? Is that a plane? NO! It's another victim of 'Misterious killer'. Dude is so cool and quiet. He never misses and he always kill with a style!", "HitmanArticleImage", 5));
        AllEvents.Add(new Event("This Article Will Change Your Life", "Are you a sad loser? Don't worry! We have a solution just for you! The solution is... JUST KILL YOURSELF", "SuicideArticleImage", 6));//Notatka, mo¿e jednak zwiêkszony wskaŸnik samobójstw?
        AllEvents.Add(new Event("Stalker", "Nuclear factory explosion!!! What an interesting day to be alive! For now we don't have any informations about possible survivors, but we hope they're going to have some cool mutations!", "StalkerArticleImage", 7));
    }
}
