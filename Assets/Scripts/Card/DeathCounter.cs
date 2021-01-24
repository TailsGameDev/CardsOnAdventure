/// explaining deathCounter
/// the size is the number of attacks to consider in counting: Ex: let's consider how many died in the last '10' attacks
/// in this case 10 would be the size of the deathCount array.
/// it starts as 1,1,1... because it will be used to see if the game tied. They will consider the game tied if in the
/// last 10 attacks, no cards died. 1,1,1... suggests 10 cards have died in the last 10 attacks
/// but unfortunately, each time the player ask an attack, it count's 2 or 3 times here because the way
/// skills and damage ware implemented
public class DeathCounter
{
    private static int[] deathCount;
    private static int deathCountIndex = 0;

    public static void ResetDeathCount()
    {
        deathCount = new int[14];
        Clear();
    }
    public static void ResetDeathCountAndMakeItMoreTolerant()
    {
        deathCount = new int[deathCount.Length + 1];
        Clear();
    }
    private static void Clear()
    {
        for (int d = 0; d < deathCount.Length; d++)
        {
            deathCount[d] = 1;
        }
    }

    public static void RegisterDeath()
    {
        deathCountIndex = (deathCountIndex + 1) % deathCount.Length;
        deathCount[deathCountIndex] = 1;
    }
    public static void RegisterSurvived()
    {
        deathCountIndex = (deathCountIndex + 1) % deathCount.Length;
        deathCount[deathCountIndex] = 0;
    }

    public static bool AreCardsDyingToFew()
    {
        int counter = 0;
        for (int i = 0; i < deathCount.Length; i++)
        {
            counter += deathCount[i];
        }
        return counter <= 0;
    }
}
