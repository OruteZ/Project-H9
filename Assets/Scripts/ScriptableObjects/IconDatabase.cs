using UnityEngine;

[CreateAssetMenu(fileName = "IconDatabase", menuName = "ScriptableObjects/IconDatabase", order = 1)]
public class IconDatabase : ScriptableObject
{
    [SerializeField]
    private IconData[] icons;
    
    public Sprite GetIcon(string name)
    {
        foreach (IconData icon in icons)
        {
            if (icon.name == name)
            {
                return icon.icon;
            }
        }

        return null;
    }
    
    //GetIconByIndex
    public Sprite GetIcon(int index)
    {
        foreach (IconData icon in icons)
        {
            if (icon.index == index)
            {
                return icon.icon;
            }
        }

        return null;
    }
}

[System.Serializable] 
public class IconData
{
    public string name;
    public int index;
    public Sprite icon;
}