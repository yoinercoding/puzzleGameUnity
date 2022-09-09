using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //private List<GameObject> fichas = new List<GameObject>();
    private List<Transform> posicionOrig=new List<Transform>();
    private List<int> posicion=new List<int> ();
    private List<int> posicionNewOrder = new List<int>();
    public List<GameObject> fichas = new List<GameObject> ();
    private tablero tab=new tablero ();
    public Canvas canvas;

    public Text segCount;
    public Text moveCount;
    public Button playButton;
    private Button btn;

    private int cantMovimientos = 0;
    private float segundosDeJuego=0;
    private int estadoJuego = 0;
    private int cargar = 1;
    public TextMeshProUGUI TMPtext;


    // Start is called before the first frame update
    void Start()
    {
        btn = playButton.GetComponent<Button>();
        btn.onClick.AddListener(play);


        canvas.enabled = true;
        llenarMatrizOrden();

        
    }

    // Update is called once per frame
    void Update()
    {
        switch (estadoJuego)
        {
            //estadoJuego=0; esperando el boton play
            case 0:
                break;
            case 1:
                mezclarMatriz();
                estadoJuego = 2;
                segCount.GetComponent<Text>().text ="0.0000";
                segundosDeJuego = 0.0f;
                cantMovimientos = 0;
                moveCount.GetComponent<Text>().text = cantMovimientos.ToString();
                TMPtext.text = "Reset";
                break;
            case 2:
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit raycastHit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity))
                    {
                        if (raycastHit.transform != null)
                        {
                            mover(raycastHit.transform.gameObject.tag);
                        }
                    }
                }

                if (!tab.win())
                {
                    segundosDeJuego = (segundosDeJuego + Time.deltaTime);
                    segCount.GetComponent<Text>().text = Math.Round(segundosDeJuego,2).ToString();
                }
                break;
        }
       
        int index=0;
        foreach (GameObject go in fichas)
        {
            int reti = 0, retj = 0;
            tab.getIndex(index, ref reti, ref retj);
            go.transform.position = tab.getPosition(reti, retj);
            index++;
        }

    }
    void llenarMatrizOrden()
    {
        cargar = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                posicion.Add(cargar);
                tab.setFicha(i,j,cargar);
                cargar++;
            }
        }
    }
    void mezclarMatriz()
    {
        List<int> temp=new List<int>();
        temp.AddRange(posicion.ToArray());
        posicionNewOrder.Clear();

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int value = UnityEngine.Random.Range(0, temp.Count);
                tab.setFicha(i,j, temp[value]);
                posicionNewOrder.Add(temp[value]);
                temp.Remove(temp[value]);
            }
        }
        int index=0;
        foreach (GameObject go in fichas)
        {
            int reti=0, retj=0;
            tab.getIndex(index,ref reti,ref retj);
            go.transform.position = tab.getPosition(reti, retj);
            index++;
        }
    }

    public void play()
    {
        estadoJuego = 1;
    }

    private void mover(string number)
    {
        if (tab.moverFicha(Int32.Parse(number) - 1))
        {
            cantMovimientos++;
            moveCount.GetComponent<Text>().text = cantMovimientos.ToString();
        }
    }

}
public class tablero
{
    private int[,] numeroFicha = new int[4,4];
    private Vector3[] posicionesOrigninales = new[] {
        new Vector3(-3.12f,3.05f,4f),
        new Vector3(-1.04f,3.05f,4f),
        new Vector3(1.04f,3.05f,4f),
        new Vector3(3.1f,3.05f,4f),

        new Vector3(-3.12f,1.01f,4f),
        new Vector3(-1.04f,1.01f,4f),
        new Vector3(1.04f,1.01f,4f),
        new Vector3(3.1f,1.01f,4f),

        new Vector3(-3.12f,-1.04f,4f),
        new Vector3(-1.04f,-1.04f,4f),
        new Vector3(1.04f,-1.04f,4f),
        new Vector3(3.1f,-1.04f,4f),

        new Vector3(-3.12f,-3.08f,4f),
        new Vector3(-1.04f,-3.08f,4f),
        new Vector3(1.04f,-3.08f,4f),
        new Vector3(3.1f,-3.08f,4f)
    };

    public void setFicha(int i,int j, int number)
    {
        numeroFicha[i,j]=number;
    }

    public int getFicha(int i, int j)
    {
        return numeroFicha[i,j];
    }



    public Vector3 getPosition(int i,int j)
    {
        return posicionesOrigninales[i*4+j];
    }

    public void getIndex(int index,ref int reti,ref int retj)
    {
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                if (numeroFicha[i, j] == index)
                {
                    reti = i; retj= j;
                }
            }
        }
    }

    public Boolean moverFicha(int ficha)
    {
        Boolean ret = false;
        int posx=0, posy=0;
        for (int i=0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (ficha == numeroFicha[i, j])
                {
                    posx = i;  posy = j;
                }
            }
        }
        if (posx >= 1)
        {
            if (numeroFicha[posx - 1, posy] == 15)
            {
                cambiarfichas(posx - 1, posy, posx, posy);
                return true;
            }
        }
        if (posx <= 2)
        {
            if (numeroFicha[posx + 1, posy] == 15)
            {
                cambiarfichas(posx + 1, posy, posx, posy);
                return true;
            }
        }

        if (posy >= 1)
        {
            if (numeroFicha[posx, posy - 1] == 15)
            {
                cambiarfichas(posx, posy - 1, posx, posy);
                return true;
            }
        } 
        if (posy <=2)
        {
            if (numeroFicha[posx, posy + 1] == 15)
            {
                cambiarfichas(posx, posy + 1, posx, posy);
                return true;
            }
        }

        return false;
    }

    public void cambiarfichas(int posxorig, int posyorig,int posxnew,int posynew)
    {
        int aux = numeroFicha[posxorig, posyorig];
        setFicha(posxorig, posyorig, numeroFicha[posxnew, posynew]);
        setFicha(posxnew, posynew,aux);
    }

    public Boolean win()
    {
        Boolean ret = true;
        int val = 0;
        for(int i = 0; i < 4; i++)
        {
            for (int j=0;j<4; j++)
            {
                if (numeroFicha[i, j] != val)
                {
                    ret = false;
                }
                val++;
            }
        }

        return ret;
    }
}