using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    public Text teste;
    public InfinityScroll infinityScroll;

    // Update is called once per frame
    void Update()
    {
        if (!infinityScroll.isRunning())
        {
            teste.text = "Valor escolhido: " + infinityScroll.SnapValor();
        }
    }
}
