/*===============================================================================
Copyright (c) 2016-2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class SamplesMainMenuTest : MonoBehaviour
{

    #region PUBLIC_MEMBERS

    public enum MenuItem
    {

        SpacecraftAR,
        MainControlRoomAR
    }


    // initialize static enum with one of the items
    public static MenuItem menuItem = MenuItem.SpacecraftAR;

    public const string MenuScene = "1-Menu";
    public const string LoadingScene = "2-Loading";

    #endregion // PUBLIC_MEMBERS

  
    #region PUBLIC_METHODS


    public static void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }


    public void LoadSelectedScene(string itemSelected)
    {
        UpdateConfiguration(itemSelected);


        switch(itemSelected){
            case ("SpacecraftAR"):
                menuItem = MenuItem.SpacecraftAR;

                break;
            
            case  ("MainControlRoomAR"):
                menuItem = MenuItem.MainControlRoomAR;

                break;
        }

        LoadingScreen.SceneToLoad = menuItem.ToString();


    }

    void UpdateConfiguration(string scene)
    {
        VuforiaConfiguration.Instance.Vuforia.MaxSimultaneousImageTargets = scene == "VuMarks" ? 10 : 4;
    }

    #endregion // PUBLIC_METHODS

}
