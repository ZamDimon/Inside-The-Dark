using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    private LevelGenerator manager;
    private Material startMaterial;
    public Material highlightmaterial;
    [SerializeField] private Sprite notificationSprite;

    public GameObject bookShelf;
    public GameObject character;
    public float distanceToInteract;

    private Notification notification;

    private void Start() {
        manager = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<LevelGenerator>();
        notification = transform.GetComponent<Notification>();
        startMaterial = bookShelf.GetComponent<SpriteRenderer>().material;
    }

    private void InteractWithShelf() {
        if (Vector3.Distance(character.transform.position, bookShelf.transform.position) <= distanceToInteract && !manager.IsUsedOnRoad(manager.characterPositionX, manager.characterPositionY, manager.newX, manager.newY, InteractableObjectType.Bookshelf)) {
            bookShelf.GetComponent<SpriteRenderer>().material = highlightmaterial;

            if (Input.GetKeyDown(KeyCode.W)) {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<StoryWriter>().OpenNewParagraph();
                
                for (int i = 0; i < manager.roads.Length; ++i) {
                    if (((manager.roads[i].x1 == manager.characterPositionX && manager.roads[i].y1 == manager.characterPositionY && manager.roads[i].x2 == manager.newX && manager.roads[i].y2 == manager.newY) ||
                (manager.roads[i].x1 == manager.newX && manager.roads[i].y1 == manager.newY && manager.roads[i].x2 == manager.characterPositionX && manager.roads[i].y2 == manager.characterPositionY)))
                        manager.roads[i].roadObjects[1].IsUsed = true;
                }

                notification.ShowNotification(notificationSprite, "Page was added to your book");
                bookShelf.GetComponent<SpriteRenderer>().material = startMaterial;
            }
        } else bookShelf.GetComponent<SpriteRenderer>().material = startMaterial;
    }

    private void Update() {
        InteractWithShelf();
    }
}
