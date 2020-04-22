using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpotPrototypeFactory : MonoBehaviour
{

    [SerializeField]
    private Button[] masterPrototypes = null;

    [SerializeField]
    private Button simpleBattlePrototype = null;

    [SerializeField]
    private Button difficultBattlePrototype = null;

    public Button GetCopyOfRandomMaster()
    {
        int randomIndex = Random.Range(0, masterPrototypes.Length);

        return Instantiate(masterPrototypes[randomIndex]);
    }

    public Button GetCopyOfRandomSpot()
    {
        int random = Random.Range(0, 3);

        Button spot = simpleBattlePrototype;

        switch (random)
        {
            case 0:
                spot = GetCopyOfRandomMaster();
                break;
            case 1:
                spot = Instantiate(difficultBattlePrototype);
                break;
            case 2:
                spot = Instantiate(difficultBattlePrototype);
                break;
        }

        return spot;
    }
}
