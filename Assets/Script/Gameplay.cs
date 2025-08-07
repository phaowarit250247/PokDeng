using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public enum Rank
    {
        Ace = 1, Two, Three, Four, Five, Six, Seven, Eight,
        Nine, Ten, Jack, Queen, King
    }

    public Suit suit;
    public Rank rank;
    public Sprite cardSprite;

    public Card(Suit s, Rank r)
    {
        suit = s;
        rank = r;
    }

    public int GetPokDengValue()
    {
        if (rank >= Rank.Jack) return 0;
        if (rank == Rank.Ace) return 1;
        return (int)rank;
    }

    public string GetDisplayName()
    {
        return rank.ToString() + " of " + suit.ToString();
    }
}

public class Gameplay : MonoBehaviour
{
    [Header("UI References")]
    public Button butDeal;
    public Image[] imgCard = new Image[4]; // 0,1: player | 2,3: AI
    public Text resultText; // ลาก Text จาก Canvas มาใส่ตรงนี้ใน Inspector

    [Header("Card Sprites")]
    public Sprite cardBackSprite;
    public Sprite[] cardSprites = new Sprite[52];

    private List<Card> deck = new List<Card>();
    private List<Card> playerHand = new List<Card>();
    private List<Card> aiHand = new List<Card>();

    void Start()
    {
        butDeal.onClick.AddListener(DealCards);
        CreateDeck();
        ShuffleDeck();
    }

    void CreateDeck()
    {
        deck.Clear();
        for (int suit = 0; suit < 4; suit++)
        {
            for (int rank = 1; rank <= 13; rank++)
            {
                Card newCard = new Card((Card.Suit)suit, (Card.Rank)rank);
                int spriteIndex = suit * 13 + (rank - 1);
                if (spriteIndex < cardSprites.Length && cardSprites[spriteIndex] != null)
                {
                    newCard.cardSprite = cardSprites[spriteIndex];
                }
                deck.Add(newCard);
            }
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void ClearHands()
    {
        playerHand.Clear();
        aiHand.Clear();
    }

    void DealCards()
    {
        ClearHands();

        if (deck.Count < 4)
        {
            CreateDeck();
            ShuffleDeck();
        }

        for (int i = 0; i < 2; i++)
        {
            playerHand.Add(DrawCard());
            aiHand.Add(DrawCard());
        }

        UpdateImages();
        DetermineWinner();
    }

    Card DrawCard()
    {
        if (deck.Count == 0)
        {
            CreateDeck();
            ShuffleDeck();
        }

        Card drawCard = deck[0];
        deck.RemoveAt(0);
        return drawCard;
    }

    void UpdateImages()
    {
        // Player cards: index 0,1
        imgCard[0].sprite = playerHand[0].cardSprite;
        imgCard[1].sprite = playerHand[1].cardSprite;

        // AI cards: index 2,3
        imgCard[2].sprite = aiHand[0].cardSprite;
        imgCard[3].sprite = aiHand[1].cardSprite;
    }

    int CalculateHandValue(List<Card> hand)
    {
        int sum = 0;
        foreach (Card card in hand)
        {
            sum += card.GetPokDengValue();
        }
        return sum % 10;
    }

    void DetermineWinner()
    {
        int playerScore = CalculateHandValue(playerHand);
        int aiScore = CalculateHandValue(aiHand);

        string result;

        if (playerScore > aiScore)
        {
            result = $"🧑 Player wins! ({playerScore} vs {aiScore})";
        }
        else if (playerScore < aiScore)
        {
            result = $"🤖 AI wins! ({aiScore} vs {playerScore})";
        }
        else
        {
            result = $"🤝 It's a tie! ({playerScore} vs {aiScore})";
        }

        resultText.text = result;
        Debug.Log(result);
    }
}
