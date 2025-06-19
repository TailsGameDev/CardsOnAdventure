using UnityEngine;

// TODO: Rethink this architecture
public class OpenersSuperclass : MonoBehaviour
{
    protected static ThePopUpOpenerInstance openerOfPopUpsMadeInEditor;
    protected static CustomPopUp customPopUpOpener;
    protected static SceneOpener sceneOpener;
    protected static CustomPopUp incidentPopUpOpener;
    protected static StoryPopUpOpener storyPopUpOpener;
}