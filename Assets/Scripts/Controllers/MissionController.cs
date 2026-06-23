using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;

public class MissionController : MonoBehaviour
{
    [SerializeField] private Transform inventory;
    [SerializeField] private Transform hotbar;
    [SerializeField] private int requiredIdItem;
    [SerializeField] private int requiredQuantity;
    private bool missionFinished = false;

    [SerializeField] private Dialogue disableObject;
    [SerializeField] private Dialogue enableObject;


    //Para otorgar objetos al jugador.
    [SerializeField] Inventory playerIntentory;
    

    //referencia al script de inventory, usar un metodo CheckConditionForMision que llame y pase parametros al metodo CheckInventoryForItems del script Inventory y en base a eso
    //que devuelva true o falso. Tal vez usar EVENTOS, suscribir al script este a una señal que se manda cada vez que PickUpItem
  

    private int totalItemQuantity; //Este valor se puede usar para mostrar en HUD en caso de que queramos

    private void OnEnable()
    {
        Inventory.pickUpItem += CheckMissionCondition;

    }
    private void OnDisable()
    {
        Inventory.pickUpItem -= CheckMissionCondition;
        
    }

    private void CheckMissionCondition()
    {
        Debug.Log("Chequeando mision");
        if (CheckInventoryForItems(inventory, hotbar, requiredIdItem))
        {
            missionFinished = true;

            //Esto se podria hacer mucho mas eficiente y modular, pero por ahora pongo los cambios que produce la mision acá en codigo
            disableObject.enabled = false;
            enableObject.enabled = true;

            Debug.Log("Mision cumplida!");

        }
    }
    private bool CheckInventoryForItems(Transform inventory,Transform hotbar, int id)
    {
        int quantity = new int();

        //Me fijo si en cada Slot existe un child Item
        foreach (Transform slot in inventory)
        {
            //Si existe ese child Item, tomo su referencia y analizo si se cumplen las condiciones de ID y cantidad que recibe la funcion
            if (slot.childCount > 0)
            {
                ItemUI itemInSlot = slot.GetChild(0).GetComponent<ItemUI>();
                if (itemInSlot != null && itemInSlot.id == id)
                {
                    quantity += itemInSlot.quantity;
                    Debug.Log(quantity);
                }



            }

            //Chequeo por cada Slot si ya se registró el item en su cantidad necesaria para cumplir la mision
            if (quantity >= requiredQuantity | quantity > requiredQuantity)
            {
                

                return true;
            }
                       
        
       

        }
        foreach (Transform slot in hotbar)
        {
            //Si existe ese child Item, tomo su referencia y analizo si se cumplen las condiciones de ID y cantidad que recibe la funcion
            if (slot.childCount > 0)
            {
                ItemUI itemInSlot = slot.GetChild(0).GetComponent<ItemUI>();
                if (itemInSlot != null && itemInSlot.id == id)
                {
                    quantity += itemInSlot.quantity;
                    
                }



            }

            //Chequeo por cada Slot si ya se registró el item en su cantidad necesaria para cumplir la mision
            if (quantity == requiredQuantity | quantity > requiredQuantity)
            {
                

                return true;
            }
           



        }

        Debug.Log("No hay suficientes items");
        return false;
    }

    public void Mission1()
    {
        if (missionFinished)
        {
            //RECOMPENSA AL JUGADOR, ID DEL ITEM Y CANTIDAD
            playerIntentory.PickUpItem(1, 15);
            Destroy(this);
        }
    }
    
}
