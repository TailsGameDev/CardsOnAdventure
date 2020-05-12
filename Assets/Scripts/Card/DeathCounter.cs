/// explaining deathCounter
/// the size is the number of attacks to consider in counting: Ex: let's consider how many died in the last '10' attacks
/// in this case 10 would be the size of the deathCount array.
/// it starts as 1,1,1... because it will be used to see if the game tied. They will consider the game tied if in the
/// last 10 attacks, no cards died. 1,1,1... suggests 10 cards have died in the last 10 attacks
/// but unfortunately, each time the player ask an attack, it count's 2 or 3 times here because the way
/// skills and damage ware implemented
public class DeathCounter
{
    private int[] deathCount = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    private int deathCountIndex = 0;

    public DeathCounter(int size)
    {
        deathCount = new int[size];

        // Make the system think that every last 'size' attacks killed one card each.
        for (int i = 0; i < size; i++)
        {
            deathCount[i] = 1;
        }
    }

    public void RegisterDeath()
    {
        deathCountIndex = (deathCountIndex + 1) % deathCount.Length;
        deathCount[deathCountIndex] = 1;
    }
    public void RegisterSurvived()
    {
        deathCountIndex = (deathCountIndex + 1) % deathCount.Length;
        deathCount[deathCountIndex] = 0;
    }

    public int GetDeathCount()
    {
        int counter = 0;
        for (int i = 0; i < deathCount.Length; i++)
        {
            counter += deathCount[i];
        }
        return counter;
    }
    public void ResetDeathCount()
    {
        for (int i = 0; i < deathCount.Length; i++)
        {
            deathCount[i] = 1;
        }
    }
}
