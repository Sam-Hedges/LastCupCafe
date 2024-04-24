
public class TrashCan : Workstation
{
    private void Update()
    {
        if (currentlyStoredItem)
        {
            Destroy(currentlyStoredItem.gameObject);
            currentlyStoredItem = null;
        }
    }
}
