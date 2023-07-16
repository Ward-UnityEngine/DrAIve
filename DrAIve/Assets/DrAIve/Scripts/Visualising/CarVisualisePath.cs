using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarVisualisePath : MonoBehaviour, IPathVisualiseBehaviour
{

    private NavRequester navRequester;
    public GameObject GreenSphere;
    public GameObject BlueSphere;
    public GameObject PurpleSphere;

    private List<GameObject> visualizing;
    private RoadNode[] path = null;

    private Coroutine coroutine = null;


    private void Start()
    {
        navRequester = GetComponent<NavRequester>();
        visualizing = new();
    }

    IEnumerator CheckForNewSolution()
    {
        bool keep_running = true;
        while (keep_running) {
            RoadNode[] newPath = navRequester.getSolution();
            if (newPath != path)
            {
                //new path
                path = newPath;
                Invoke("RefreshVisualization", 0.5f);
                keep_running = false;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    private void RefreshVisualization()
    {
        stopVisualising();
        visualise();
    }

    public void visualise()
    {
        path = navRequester.getSolution();
        if (path != null)
        {
            int index = 0;
            foreach (RoadNode node in path)
            {
                if (index == 0)
                {
                    visualizing.Add(Instantiate(GreenSphere, node.road.transform.Find("Nav").transform));
                }
                else if (index == path.Length - 1)
                {
                    visualizing.Add(Instantiate(BlueSphere, node.road.transform.Find("Nav").transform));
                }
                else
                {
                    visualizing.Add(Instantiate(PurpleSphere, node.road.transform.Find("Nav").transform));
                }
                index++;
            }
        }
        coroutine = StartCoroutine(CheckForNewSolution());
    }
    public void stopVisualising()
    {
        foreach (GameObject visualized in visualizing)
        {
            Destroy(visualized);
        }
        visualizing = new();
        StopCoroutine(coroutine);
    }

    public bool isVisualising()
    {
        return (visualizing.Count > 0);
    }
}
