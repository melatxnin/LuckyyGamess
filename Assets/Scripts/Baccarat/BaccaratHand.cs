public class BaccaratHand : HandBase
{
    private int[] cardValues =
    {
        0,
        1,
        2,3,4,5,6,7,8,9,
        0,0,0,0
    };

    public override int GetScore()
    {
        int score = 0;

        foreach (Card c in cards)
        {
            score += cardValues[(int)c.rank];
        }

        return score % 10;
    }
}
