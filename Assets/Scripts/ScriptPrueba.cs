using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPrueba : MonoBehaviour
{
    int numero1 = 10, numero2 = 60, resultado = 0;
    void Start()
    {
        Debug.Log("Este es un mensaje de prueba");
        resultado = numero1 + numero2;
        Debug.Log(resultado);
    }
}