using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DatosCultivos
{
    public int id;
    public string nombre;
    public float velocidadDeCrecimiento;
    public string temporada;

}

[Serializable]
public class ContenedorCultivos 
{
    
    public List<DatosCultivos> listaCultivos;
}
