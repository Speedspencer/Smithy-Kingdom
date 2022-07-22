
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Ore _ore;
    private OreDatabase _oreDatabase;
    private OreStats[] _premiumOres;

    private void Awake()
    {
        _ore = Ore.Instance;
    }

    private void Start()
    {
        _oreDatabase = _ore.oreDatabase;
        int i = 0;
        foreach (var VARIABLE in _oreDatabase.ores)
        {
            if (VARIABLE.isPremium)
            {
                _premiumOres[i] = VARIABLE;
            }
        }
    }

    public void RandomGacha()
    {
        if (_premiumOres.Length != 0)
        {
            var randomNumber = Random.Range(1f,6f);
            switch (randomNumber)
            {
                case 1:
                    Debug.Log("You got a " + _premiumOres[0].oreName);
                    _ore.ModifyOreAmount(_premiumOres[0], 1);
                    break;
                case 2:
                    Debug.Log("You got a " + _premiumOres[1].oreName);
                    _ore.ModifyOreAmount(_premiumOres[1], 1);
                    break;
                case 3:
                    Debug.Log("You got a " + _premiumOres[2].oreName);
                    _ore.ModifyOreAmount(_premiumOres[2], 1);
                    break;
                case 4:
                    Debug.Log("You got a " + _premiumOres[3].oreName);
                    _ore.ModifyOreAmount(_premiumOres[3], 1);
                    break;
                case 5:
                    Debug.Log("You got a " + _premiumOres[4].oreName);
                    _ore.ModifyOreAmount(_premiumOres[4], 1);
                    break;
                case 6:
                    Debug.Log("You got a " + _premiumOres[5].oreName);
                    _ore.ModifyOreAmount(_premiumOres[5], 1);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("There are no Premium Ores in the database");
        }
    }
    
}
