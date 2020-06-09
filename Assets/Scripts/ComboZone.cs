using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboZone : MonoBehaviour
{
    private List<Collider> colliders;
    private int multiplierCombo = 0;
    private float comboLevel = 0;
    float alphaFloorCombo ;
    private Image[] images;
    private const float maxAlphaFloor = 0.35f;

    // Start is called before the first frame update
    void Start()
    {
        colliders = new List<Collider>();
        images = GetComponentsInChildren<Image>();
    }

    //OnTriggerExit is not safe if object is destroyed
    void checkDestroyedColliders()
    {
        colliders.RemoveAll(x => !x);
    }


    public void ResetCombo()
    {
        multiplierCombo = 0;
        comboLevel = 0;
        alphaFloorCombo = 0;
        images[0].fillAmount = 0;
        images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b, 0);
        colliders.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        checkDestroyedColliders();
        if (colliders.Count > 0)
        {
            comboLevel += 0.5f * Time.deltaTime;
            if (comboLevel > 1)
                comboLevel = 1;

            alphaFloorCombo += 1f * Time.deltaTime;
            if (alphaFloorCombo > maxAlphaFloor)
                alphaFloorCombo = maxAlphaFloor;
        }
        else
        {
            comboLevel -= 0.3f * Time.deltaTime;
            if (comboLevel < 0f)
                comboLevel = 0f;
            
            alphaFloorCombo -= 1f * Time.deltaTime;
            if (alphaFloorCombo < 0f)
                alphaFloorCombo = 0f;
        }

        images[0].fillAmount = comboLevel;
        images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b, alphaFloorCombo);

        if (comboLevel == 0)
        {
            multiplierCombo = 0;
        }
    }

    public void TryAddMultiplierCombo()
    {
        if (comboLevel > 0)
            multiplierCombo++;
    }

    public int GetMultiplierCombo()
    {
        return multiplierCombo;
    }

    public float GetComboLevelAmount()
    {
        return comboLevel;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("TruckEnemy"))
            colliders.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("TruckEnemy"))
            colliders.Remove(other);
    }
}
