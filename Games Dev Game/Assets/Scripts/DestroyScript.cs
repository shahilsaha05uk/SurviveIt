using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestroyScript : MonoBehaviour
{
    GameManager manager;
    public static NavMeshSurface surface;




    private void Start()
    {
        manager = GetComponent<GameManager>();
        //StartCoroutine(RebuildNavMeshSurface());
    }
    public static void UpdateNavmeshSurface()
    {
        surface = GameObject.Find("Map").GetComponent<NavMeshSurface>();

        surface.UpdateNavMesh(surface.navMeshData);

    }

    public static void DestroyElement(GameObject obj)
    {
        Destroy(obj,3);
    }


/*    public bool DestroyObject(GameObject obj)
    {
        if (obj != null)
        {
            Animator anim;
            obj.TryGetComponent<Animator>(out anim);

            if (anim != null)
            {
                if (obj.CompareTag("Enemy"))
                {
                    if (obj.name == "Monster")
                    {
                        obj.GetComponent<MonsterScript>().MonsterBehaviour(MonsterMove.DEAD);
                        Destroy(obj, 4f);
                    }
                    else if (obj.name == "wizard")
                    {
                        obj.GetComponent<WizardScript>().WizardBehaviour(WizardMove.DEAD);
                        Destroy(obj, 4f);
                    }
                    else if (obj.name == "Ghoul")
                    {
                        obj.GetComponent<GhoulScript>().GhoulBehaviour(GhoulMove.DEAD);
                        Destroy(obj, 4f);
                    }
                    manager.playerGold += 4000;
                    GetComponent<GamePlayUIScript>().UpdateGold(manager.playerGold);

                    return true;
                }
            }
            else
            {
                Destroy(obj);
                //rebuildSurface = true;
                Debug.Log("Rebuild Surface is true");
                return true;
            }
            return false;
        }
        return false;
    }
*/}
