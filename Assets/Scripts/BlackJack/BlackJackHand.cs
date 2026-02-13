public class BlackjackHand : HandBase
{
    private int[] cardValues =
    {
        0,
        11,
        2,3,4,5,6,7,8,9,
        10,10,10,10
    };

    public override int GetScore()
    {
        int score = 0;
        int aceCount = 0;

        foreach (Card c in cards)
        {
            int value = cardValues[(int)c.rank];
            score += value;

            if (c.rank == Rank.Ace)
                aceCount++;
        }

        while (score > 21 && aceCount > 0)
        {
            score -= 10;
            aceCount--;
        }

        return score;
    }
}
