using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexHolder : MonoBehaviour
{
    private int selectedIndex = -1;

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public void SetSelectedIndex(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;
    }

    public void ClearSelection()
    {
        selectedIndex = -1;
    }

    public bool SomeIndexWasSelectedSinceLastClear()
    {
        return selectedIndex != -1;
    }
}
