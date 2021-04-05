using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesList
{
    public Upgrade[] upgrades;

    public UpgradesList() {
        upgrades = new Upgrade[]
        {
        new Upgrade(0, "Chain reaction - Cough", UpgradeType.Intensity, "Causes victims cough to up to 3 times in a row.", 3f, new int[1]{2}), // 1 represents the amount of times it will happen in sequence
        new Upgrade(1, "Chain reaction - Sneeze", UpgradeType.Intensity, "Causes victims sneeze to up to 3 times in a row.", 3f, new int[1]{3}), // 1 represents the amount of times it will happen in sequence
        new Upgrade(2, "Increased range - Cough", UpgradeType.Range, "Increases cough contamination/infection range.", 5f, new int[1]{2}), // 1 represents the amount of times it will happen in sequence
        new Upgrade(3, "Increased range - Sneeze", UpgradeType.Range, "Increases sneeze contamination/infection range", 5f, new int[1]{3}), // 1 represents the amount of times it will happen in sequence
        new Upgrade(4, "Increased frequency - Sneeze", "Causes victims to cough more often.", UpgradeType.Frequency, new Vector2(5f, 10f), new int[1]{2}), // 1 represents the amount of times it will happen in sequence
        new Upgrade(5, "Increased frequency - Cough", "Causes victims to cough more frequently.", UpgradeType.Frequency, new Vector2(5f, 10f), new int[1]{3}) // 1 represents the amount of times it will happen in sequence
    };
    }
}
