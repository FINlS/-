using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectedWeapon = 0; // Индекс текущего оружия
    public List<GameObject> weapons = new List<GameObject>(); // Список всех пушек в руке

    void Start()
    {
        SelectWeapon();
    }

    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        // Переключение колесиком мыши
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapon >= weapons.Count - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = weapons.Count - 1;
            else
                selectedWeapon--;
        }

        // Переключение цифрами 1, 2, 3...
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Count >= 2) selectedWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Count >= 3) selectedWeapon = 2;

        // Если выбор изменился, обновляем видимость
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (GameObject weapon in weapons)
        {
            if (i == selectedWeapon)
                weapon.SetActive(true); // Включаем выбранное
            else
                weapon.SetActive(false); // Выключаем остальные
            i++;
        }
    }
}